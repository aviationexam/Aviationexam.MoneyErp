using Aviationexam.MoneyErp.Graphql.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using Xunit;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public sealed class MoneyErpAuthenticationsClassData() : TheoryData<MoneyErpAuthenticationsClassData.AuthenticationData?>(
    GetData()
)
{
    public static IEnumerable<TheoryDataRow<AuthenticationData?>> GetData()
    {
        Loader.LoadEnvFile(".env.local");

        var clientId = Environment.GetEnvironmentVariable("MONEYERP_CLIENT_ID")?.Trim();
        var clientSecret = Environment.GetEnvironmentVariable("MONEYERP_CLIENT_SECRET")?.Trim();
        var endpoint = Environment.GetEnvironmentVariable("MONEYERP_ENDPOINT")?.Trim();
        var endpointCertificatePath = Environment.GetEnvironmentVariable("MONEYERP_ENDPOINT_CERTIFICATE")?.Trim();

        if (
            clientId is null
            || clientSecret is null
            || endpoint is null
        )
        {
            yield return new TheoryDataRow<AuthenticationData?>(null)
            {
                Skip = "Authentication data is not set. Please set MONEYERP_CLIENT_ID, MONEYERP_CLIENT_SECRET, and MONEYERP_ENDPOINT environment variables.",
            };
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
        ));
    }

    public sealed record AuthenticationData(
        string ClientId,
        string ClientSecret,
        string ServerAddress,
        string? EndpointCertificatePem
    ) : IFormattable, IParsable<AuthenticationData>
    {
        public string ToString(string? format, IFormatProvider? formatProvider) => new JsonArray(
            ClientId,
            ClientSecret,
            ServerAddress,
            EndpointCertificatePem
        ).ToString();

        public static AuthenticationData Parse(string s, IFormatProvider? provider)
        {
            if (JsonNode.Parse(s) is not JsonArray { Count: 3 } arr)
            {
                throw new FormatException("Input string is not a valid AuthenticationData JSON array.");
            }

            return new AuthenticationData(
                arr[0]?.GetValue<string>() ?? throw new FormatException("ClientId missing."),
                arr[1]?.GetValue<string>() ?? throw new FormatException("ClientSecret missing."),
                arr[2]?.GetValue<string>() ?? throw new FormatException("ServerAddress missing."),
                arr[3]?.GetValue<string>()
            );
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out AuthenticationData result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
