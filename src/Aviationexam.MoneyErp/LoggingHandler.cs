using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.MoneyErp;

public class LoggingHandler(
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

        var stopwatch = Stopwatch.StartNew();

        var response = await base.SendAsync(request, cancellationToken);

        stopwatch.Stop();

        logger.LogInformation(
            "HTTP {Method} {Uri} responded {StatusCode} ({StatusCodeNumber}) in {ElapsedMilliseconds}ms",
            request.Method, request.RequestUri, response.StatusCode, (int) response.StatusCode, stopwatch.ElapsedMilliseconds
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
