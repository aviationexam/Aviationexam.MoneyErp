using System;
using System.Text.Json.Serialization;

namespace Aviationexam.MoneyErp.Common.Oidc;

public sealed class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;

    public TimeSpan ExpiresInTimeSpan => TimeSpan.FromSeconds(ExpiresIn);
}

