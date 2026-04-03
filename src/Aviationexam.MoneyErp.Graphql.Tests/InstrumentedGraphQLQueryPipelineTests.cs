using Aviationexam.MoneyErp.Graphql.Pipelines;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ZeroQL;
using ZeroQL.Internal;
using ZeroQL.Pipelines;
using ZeroQL.Stores;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public class InstrumentedGraphQLQueryPipelineTests
{
    [Theory]
    [InlineData("query GetVersion { version }", "GetVersion")]
    [InlineData("query GetUser($id: ID!) { user(id: $id) { name } }", "GetUser")]
    [InlineData("mutation CreateUser($input: CreateUserInput!) { createUser(input: $input) { id } }", "CreateUser")]
    [InlineData("subscription OnUserCreated { onUserCreated { id } }", "OnUserCreated")]
    [InlineData("query { version }", null)]
    [InlineData("mutation { deleteAll }", null)]
    [InlineData("{ version }", null)]
    [InlineData("query Get_User_V2 { user { name } }", "Get_User_V2")]
    [InlineData("  query  SpacedQuery  { version }", "SpacedQuery")]
    public void ExtractOperationNameWorks(string queryDocument, string? expectedName)
    {
        var queryInfo = new QueryInfo
        {
            Query = queryDocument,
            QueryBody = queryDocument,
            OperationType = "query",
            Hash = "abc123",
        };

        var result = InstrumentedGraphQLQueryPipeline.ExtractOperationName(queryInfo);

        Assert.Equal(expectedName, result);
    }

    [Fact]
    public void ExtractOperationNameReturnsNullForEmptyQuery()
    {
        var queryInfo = new QueryInfo
        {
            Query = "",
            QueryBody = "",
            OperationType = "query",
            Hash = "abc123",
        };

        var result = InstrumentedGraphQLQueryPipeline.ExtractOperationName(queryInfo);

        Assert.Null(result);
    }

    [Fact]
    public async Task PipelineCreatesActivityWithOperationTags()
    {
        const string queryKey = "test-query-key";
        const string queryDocument = "query TestOp { test }";
        const string queryHash = "hash123";

        GraphQLQueryStore<string>.Query[queryKey] = new QueryInfo
        {
            Query = queryDocument,
            QueryBody = "{ test }",
            OperationType = "query",
            Hash = queryHash,
        };

        try
        {
            var capturedActivities = new List<Activity>();
            using var listener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == InstrumentationConstants.ActivitySourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = activity => capturedActivities.Add(activity),
            };
            ActivitySource.AddActivityListener(listener);

            var innerPipeline = Substitute.For<IGraphQLQueryPipeline>();
            innerPipeline
                .ExecuteAsync<string>(
                    Arg.Any<IHttpHandler>(),
                    Arg.Any<string>(),
                    Arg.Any<object?>(),
                    Arg.Any<CancellationToken>(),
                    Arg.Any<System.Func<GraphQLRequest, HttpContent>>()
                )
                .Returns(new GraphQLResponse<string>
                {
                    Data = "result",
                    HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                    Query = queryDocument,
                });

            var options = Options.Create(new MoneyErpGraphqlInstrumentationOptions());
            var pipeline = new InstrumentedGraphQLQueryPipeline(innerPipeline, options);

            var httpHandler = Substitute.For<IHttpHandler>();

            await pipeline.ExecuteAsync<string>(
                httpHandler,
                queryKey,
                null,
                CancellationToken.None,
                _ => new StringContent("")
            );

            Assert.Single(capturedActivities);
            var activity = capturedActivities[0];
            Assert.Equal("query TestOp", activity.DisplayName);
            Assert.Equal(ActivityKind.Client, activity.Kind);
            Assert.Equal("query", activity.GetTagItem("graphql.operation.type"));
            Assert.Equal("TestOp", activity.GetTagItem("graphql.operation.name"));
            Assert.Equal(queryHash, activity.GetTagItem("graphql.document.hash"));
            Assert.Null(activity.GetTagItem("graphql.document"));
        }
        finally
        {
            GraphQLQueryStore<string>.Query.Remove(queryKey);
        }
    }

    [Fact]
    public async Task PipelineRecordsDocumentWhenEnabled()
    {
        const string queryKey = "test-doc-key";
        const string queryDocument = "query DocTest { test }";

        GraphQLQueryStore<string>.Query[queryKey] = new QueryInfo
        {
            Query = queryDocument,
            QueryBody = "{ test }",
            OperationType = "query",
            Hash = "hash456",
        };

        try
        {
            var capturedActivities = new List<Activity>();
            using var listener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == InstrumentationConstants.ActivitySourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = activity => capturedActivities.Add(activity),
            };
            ActivitySource.AddActivityListener(listener);

            var innerPipeline = Substitute.For<IGraphQLQueryPipeline>();
            innerPipeline
                .ExecuteAsync<string>(
                    Arg.Any<IHttpHandler>(),
                    Arg.Any<string>(),
                    Arg.Any<object?>(),
                    Arg.Any<CancellationToken>(),
                    Arg.Any<System.Func<GraphQLRequest, HttpContent>>()
                )
                .Returns(new GraphQLResponse<string>
                {
                    Data = "result",
                    HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                    Query = queryDocument,
                });

            var options = Options.Create(new MoneyErpGraphqlInstrumentationOptions
            {
                RecordDocument = true,
            });
            var pipeline = new InstrumentedGraphQLQueryPipeline(innerPipeline, options);

            var httpHandler = Substitute.For<IHttpHandler>();

            await pipeline.ExecuteAsync<string>(
                httpHandler,
                queryKey,
                null,
                CancellationToken.None,
                _ => new StringContent("")
            );

            Assert.Single(capturedActivities);
            var activity = capturedActivities[0];
            Assert.Equal(queryDocument, activity.GetTagItem("graphql.document"));
        }
        finally
        {
            GraphQLQueryStore<string>.Query.Remove(queryKey);
        }
    }

    [Fact]
    public async Task PipelineSetsErrorStatusOnGraphQLErrors()
    {
        const string queryKey = "test-error-key";
        const string queryDocument = "query ErrorTest { test }";

        GraphQLQueryStore<string>.Query[queryKey] = new QueryInfo
        {
            Query = queryDocument,
            QueryBody = "{ test }",
            OperationType = "query",
            Hash = "hash789",
        };

        try
        {
            var capturedActivities = new List<Activity>();
            using var listener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == InstrumentationConstants.ActivitySourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = activity => capturedActivities.Add(activity),
            };
            ActivitySource.AddActivityListener(listener);

            var innerPipeline = Substitute.For<IGraphQLQueryPipeline>();
            innerPipeline
                .ExecuteAsync<string>(
                    Arg.Any<IHttpHandler>(),
                    Arg.Any<string>(),
                    Arg.Any<object?>(),
                    Arg.Any<CancellationToken>(),
                    Arg.Any<System.Func<GraphQLRequest, HttpContent>>()
                )
                .Returns(new GraphQLResponse<string>
                {
                    Data = null,
                    HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                    Query = queryDocument,
                    Errors = [new GraphQueryError { Message = "Something went wrong" }],
                });

            var options = Options.Create(new MoneyErpGraphqlInstrumentationOptions());
            var pipeline = new InstrumentedGraphQLQueryPipeline(innerPipeline, options);

            var httpHandler = Substitute.For<IHttpHandler>();

            await pipeline.ExecuteAsync<string>(
                httpHandler,
                queryKey,
                null,
                CancellationToken.None,
                _ => new StringContent("")
            );

            Assert.Single(capturedActivities);
            var activity = capturedActivities[0];
            Assert.Equal(ActivityStatusCode.Error, activity.Status);
            Assert.Equal(1, activity.GetTagItem("graphql.error.count"));
        }
        finally
        {
            GraphQLQueryStore<string>.Query.Remove(queryKey);
        }
    }

    [Fact]
    public async Task PipelineRespectsDisabledOptions()
    {
        const string queryKey = "test-disabled-key";
        const string queryDocument = "query DisabledTest { test }";

        GraphQLQueryStore<string>.Query[queryKey] = new QueryInfo
        {
            Query = queryDocument,
            QueryBody = "{ test }",
            OperationType = "query",
            Hash = "hash000",
        };

        try
        {
            var capturedActivities = new List<Activity>();
            using var listener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == InstrumentationConstants.ActivitySourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = activity => capturedActivities.Add(activity),
            };
            ActivitySource.AddActivityListener(listener);

            var innerPipeline = Substitute.For<IGraphQLQueryPipeline>();
            innerPipeline
                .ExecuteAsync<string>(
                    Arg.Any<IHttpHandler>(),
                    Arg.Any<string>(),
                    Arg.Any<object?>(),
                    Arg.Any<CancellationToken>(),
                    Arg.Any<System.Func<GraphQLRequest, HttpContent>>()
                )
                .Returns(new GraphQLResponse<string>
                {
                    Data = "result",
                    HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                    Query = queryDocument,
                });

            var options = Options.Create(new MoneyErpGraphqlInstrumentationOptions
            {
                RecordOperationName = false,
                RecordOperationType = false,
                RecordDocument = false,
                RecordDocumentHash = false,
            });
            var pipeline = new InstrumentedGraphQLQueryPipeline(innerPipeline, options);

            var httpHandler = Substitute.For<IHttpHandler>();

            await pipeline.ExecuteAsync<string>(
                httpHandler,
                queryKey,
                null,
                CancellationToken.None,
                _ => new StringContent("")
            );

            Assert.Single(capturedActivities);
            var activity = capturedActivities[0];
            Assert.Equal("graphql", activity.DisplayName);
            Assert.Null(activity.GetTagItem("graphql.operation.type"));
            Assert.Null(activity.GetTagItem("graphql.operation.name"));
            Assert.Null(activity.GetTagItem("graphql.document"));
            Assert.Null(activity.GetTagItem("graphql.document.hash"));
        }
        finally
        {
            GraphQLQueryStore<string>.Query.Remove(queryKey);
        }
    }
}
