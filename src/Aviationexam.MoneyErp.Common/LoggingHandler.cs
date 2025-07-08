using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.MoneyErp.Common;

public class LoggingHandler(
    TimeProvider timeProvider,
    ILogger logger
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken
    )
    {
        logger.LogInformation("HTTP {Method} {Uri}", request.Method, request.RequestUri);

        if (
            logger.IsEnabled(LogLevel.Trace)
            && request.Content is { } requestContent
        )
        {
            await requestContent.LoadIntoBufferAsync(
#if NET9_0_OR_GREATER
                cancellationToken
#endif
            );
            var requestBody = await requestContent.ReadAsStringAsync(cancellationToken);

            logger.LogTrace("Request Body: {RequestBody}", requestBody);
        }

        var now = timeProvider.GetTimestamp();

        var response = await base.SendAsync(request, cancellationToken);

        var requestTimespan = timeProvider.GetElapsedTime(now);

        logger.LogInformation(
            "HTTP {Method} {Uri} responded {StatusCode} ({StatusCodeNumber}) in {Elapsed}ms",
            request.Method, request.RequestUri, response.StatusCode, (int) response.StatusCode, requestTimespan
        );

        if (
            logger.IsEnabled(LogLevel.Trace)
            && response.Content is { } responseContent
        )
        {
            await responseContent.LoadIntoBufferAsync(
#if NET9_0_OR_GREATER
                cancellationToken
#endif
            );
            var responseBody = await responseContent.ReadAsStringAsync(cancellationToken);

            logger.LogTrace("Response Body: {ResponseBody}", responseBody);
        }

        return response;
    }
}
