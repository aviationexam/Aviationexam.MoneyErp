using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using ZLinq;

namespace Aviationexam.MoneyErp.RestApi.Extensions;

public static class RequestBuilderExtensions
{
    public static RequestInformation AddCustomQueryParameters<TClient>(
        this TClient requestBuilder,
        RequestInformation requestInformation,
        Action<ISet<KeyValuePair<string, string?>>> queryParameterBuilder
    ) where TClient : BaseRequestBuilder
    {
        requestBuilder.SetBaseUrlForRequestInformation(requestInformation);

        var query = QueryHelpers.ParseQuery(requestInformation.URI.Query)
            .AsValueEnumerable()
            .SelectMany(x => x.Value.AsValueEnumerable().Select(v => KeyValuePair.Create(x.Key, v)))
            .ToHashSet();
        queryParameterBuilder(query);

        var pathParameters = requestInformation.PathParameters.AsValueEnumerable().ToDictionary();
        var queryParameters = requestInformation.QueryParameters.AsValueEnumerable().ToDictionary();

        requestInformation.URI = new Uri($"{requestInformation.URI.GetLeftPart(UriPartial.Path)}{query.SerializeQuery()}", UriKind.Absolute);

        requestInformation.PathParameters = pathParameters;
        requestInformation.QueryParameters = queryParameters;

        return requestInformation;
    }

    private static string SerializeQuery(
        this ISet<KeyValuePair<string, string?>> query
    )
    {
        var queryString = query
            .AsValueEnumerable()
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}")
            .JoinToString('&');

        if (!string.IsNullOrEmpty(queryString))
        {
            return $"?{queryString}";
        }

        return queryString;
    }

    public static void SetBaseUrlForRequestInformation<TClient>(
        this TClient requestBuilder,
        RequestInformation requestInformation
    ) where TClient : BaseRequestBuilder
    {
        requestInformation.PathParameters.AddOrReplace("baseurl", BaseRequestBuilderExtensions.GetRequestAdapter(requestBuilder).BaseUrl!);
    }
}
