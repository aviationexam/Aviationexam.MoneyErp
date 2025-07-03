using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aviationexam.MoneyErp.Extensions;

public static class RequestBuilderExtensions
{
    public static RequestInformation AddCustomQueryParameters<TClient>(
        this TClient requestBuilder,
        RequestInformation requestInformation,
        Action<Dictionary<string, StringValues>> queryParameterBuilder
    ) where TClient : BaseRequestBuilder
    {
        requestBuilder.SetBaseUrlForRequestInformation(requestInformation);

        var query = QueryHelpers.ParseQuery(requestInformation.URI.Query);
        queryParameterBuilder(query);

        var pathParameters = requestInformation.PathParameters.ToDictionary();
        var queryParameters = requestInformation.QueryParameters.ToDictionary();

        requestInformation.URI = new Uri(QueryHelpers.AddQueryString(requestInformation.URI.GetLeftPart(UriPartial.Path), query));

        requestInformation.PathParameters = pathParameters;
        requestInformation.QueryParameters = queryParameters;

        return requestInformation;
    }

    public static void SetBaseUrlForRequestInformation<TClient>(
        this TClient requestBuilder,
        RequestInformation requestInformation
    ) where TClient : BaseRequestBuilder
    {
        requestInformation.PathParameters.AddOrReplace("baseurl", BaseRequestBuilderExtensions.GetRequestAdapter(requestBuilder).BaseUrl!);
    }
}
