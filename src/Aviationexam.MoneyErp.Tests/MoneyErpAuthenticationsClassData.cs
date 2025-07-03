using Aviationexam.MoneyErp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aviationexam.MoneyErp.Tests
{
    public class MoneyErpAuthenticationsClassData() : TheoryData<string, string, string>(
        GetData()
    )
    {
        private static IEnumerable<TheoryDataRow<string, string, string>> GetData()
        {
            Loader.LoadEnvFile(".env.local");

            var clientId = Environment.GetEnvironmentVariable("MONEYERP_CLIENT_ID")?.Trim();
            var clientSecret = Environment.GetEnvironmentVariable("MONEYERP_CLIENT_SECRET")?.Trim();
            var endpoint = Environment.GetEnvironmentVariable("MONEYERP_ENDPOINT")?.Trim();

            if (
                clientId is null
                || clientSecret is null
                || endpoint is null
            )
            {
                yield return new TheoryDataRow<string, string, string>(string.Empty, string.Empty, string.Empty);
                yield break;
            }

            yield return new TheoryDataRow<string, string, string>(clientId, clientSecret, endpoint);
        }
    }
}
