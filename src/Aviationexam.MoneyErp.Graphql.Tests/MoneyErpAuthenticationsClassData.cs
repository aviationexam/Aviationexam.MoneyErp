using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public sealed class MoneyErpAuthenticationsClassData() : TheoryData<MoneyErpAuthenticationsClassData.AuthenticationData?>(
    GetData()
)
{
    public static IEnumerable<TheoryDataRow<AuthenticationData?>> GetData()
    {
        Infrastructure.Loader.LoadEnvFile(".env.local");

        var clientId = System.Environment.GetEnvironmentVariable("MONEYERP_CLIENT_ID")?.Trim();
        var clientSecret = System.Environment.GetEnvironmentVariable("MONEYERP_CLIENT_SECRET")?.Trim();
        var endpoint = System.Environment.GetEnvironmentVariable("MONEYERP_ENDPOINT")?.Trim();
        var endpointCertificatePath = System.Environment.GetEnvironmentVariable("MONEYERP_ENDPOINT_CERTIFICATE")?.Trim();

        if (
            clientId is null
            || clientSecret is null
            || endpoint is null
        )
        {
            yield return new TheoryDataRow<AuthenticationData?>(null)
            {
                Skip = "Authentication data is not set. Please set MONEYERP_CLIENT_ID, MONEYERP_CLIENT_SECRET, and MONEYERP_ENDPOINT environment variables.",
            }.WithTestDisplayName("Money ERP authentication data is missing");
            yield break;
        }

        string? endpointCertificatePem = null;
        if (endpointCertificatePath is not null)
        {
            using var endpointCertificate = X509CertificateLoader.LoadCertificateFromFile(endpointCertificatePath);
            endpointCertificatePem = endpointCertificate.ExportCertificatePem();
        }

        yield return new TheoryDataRow<AuthenticationData?>(new AuthenticationData(
            clientId, clientSecret, endpoint, endpointCertificatePem
        )).WithTestDisplayName("Money ERP authentication data is configured");
    }

    public sealed record AuthenticationData(
        string ClientId,
        string ClientSecret,
        string ServerAddress,
        string? EndpointCertificatePem
    )
    {
        public override string ToString() => "Money ERP authentication data (redacted)";
    }
}
