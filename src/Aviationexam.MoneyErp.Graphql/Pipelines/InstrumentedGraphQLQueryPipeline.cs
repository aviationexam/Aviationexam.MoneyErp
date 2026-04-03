using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ZeroQL;
using ZeroQL.Internal;
using ZeroQL.Pipelines;
using ZeroQL.Stores;

namespace Aviationexam.MoneyErp.Graphql.Pipelines;

public class InstrumentedGraphQLQueryPipeline(
    IGraphQLQueryPipeline innerPipeline,
    IOptions<MoneyErpGraphqlInstrumentationOptions> instrumentationOptions
) : IGraphQLQueryPipeline
{
    private static readonly AssemblyName AssemblyName = typeof(InstrumentedGraphQLQueryPipeline).Assembly.GetName();

    private static readonly ActivitySource ActivitySource = new(
        InstrumentationConstants.ActivitySourceName,
        AssemblyName.Version?.ToString()
    );

    public async Task<GraphQLResponse<TQuery>> ExecuteAsync<TQuery>(
        IHttpHandler httpHandler,
        string queryKey,
        object? variables,
        CancellationToken cancellationToken,
        Func<GraphQLRequest, HttpContent> contentCreator
    )
    {
        var options = instrumentationOptions.Value;

        QueryInfo? queryInfo = null;
        if (GraphQLQueryStore<TQuery>.Query.TryGetValue(queryKey, out var storedQueryInfo))
        {
            queryInfo = storedQueryInfo;
        }

        var activityName = BuildActivityName(queryInfo, options);

        using var activity = ActivitySource.StartActivity(activityName, ActivityKind.Client);

        if (activity is { IsAllDataRequested: true })
        {
            SetActivityTags(activity, queryInfo, options);
        }

        try
        {
            var result = await innerPipeline.ExecuteAsync<TQuery>(
                httpHandler, queryKey, variables, cancellationToken, contentCreator
            );

            if (activity is { IsAllDataRequested: true } && result.Errors is { Length: > 0 } errors)
            {
                activity.SetStatus(ActivityStatusCode.Error);
                activity.SetTag("graphql.error.count", errors.Length);
            }

            return result;
        }
        catch (Exception ex)
        {
            if (activity is { IsAllDataRequested: true })
            {
                activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            }

            throw;
        }
    }

    private static string BuildActivityName(QueryInfo? queryInfo, MoneyErpGraphqlInstrumentationOptions options)
    {
        if (queryInfo is null)
        {
            return "graphql";
        }

        var operationType = options.RecordOperationType ? queryInfo.OperationType : null;
        var operationName = options.RecordOperationName ? ExtractOperationName(queryInfo) : null;

        return (operationType, operationName) switch
        {
            (not null, not null) => $"{operationType} {operationName}",
            (not null, null) => operationType,
            _ => "graphql",
        };
    }

    private static void SetActivityTags(
        Activity activity,
        QueryInfo? queryInfo,
        MoneyErpGraphqlInstrumentationOptions options
    )
    {
        if (queryInfo is null)
        {
            return;
        }

        if (options.RecordOperationType)
        {
            activity.SetTag("graphql.operation.type", queryInfo.OperationType);
        }

        if (options.RecordOperationName)
        {
            var operationName = ExtractOperationName(queryInfo);

            if (operationName is not null)
            {
                activity.SetTag("graphql.operation.name", operationName);
            }
        }

        if (options.RecordDocument)
        {
            activity.SetTag("graphql.document", queryInfo.Query);
        }

        if (options.RecordDocumentHash)
        {
            activity.SetTag("graphql.document.hash", queryInfo.Hash);
        }
    }

    internal static string? ExtractOperationName(QueryInfo queryInfo)
    {
        var query = queryInfo.Query;

        if (string.IsNullOrEmpty(query))
        {
            return null;
        }

        var span = query.AsSpan().TrimStart();

        // Skip operation type keyword (query/mutation/subscription)
        foreach (var keyword in (ReadOnlySpan<string>) ["query", "mutation", "subscription"])
        {
            if (span.StartsWith(keyword, System.StringComparison.OrdinalIgnoreCase))
            {
                span = span[keyword.Length..].TrimStart();
                break;
            }
        }

        // Extract operation name (alphanumeric + underscore characters before '(' or '{')
        var nameLength = 0;
        foreach (var c in span)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                nameLength++;
            }
            else
            {
                break;
            }
        }

        return nameLength > 0 ? span[..nameLength].ToString() : null;
    }
}
