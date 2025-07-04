using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aviationexam.MoneyErp;

public sealed class MoneyErpAuthenticationOptions
{
    [Required]
    public string ClientId { get; set; } = null!;

    [Required]
    public string ClientSecret { get; set; } = null!;

    [Required]
    public TimeSpan JwtEarlyExpirationOffset { get; set; }

    public string TokenPath { get; } = "/connect/token";

    public string GraphQlPath { get; } = "/graphql";

    [Required]
    public Uri Endpoint { get; set; } = null!;

    [Required]
    public Uri TokenEndpoint { get; set; } = null!;

    [Required]
    public Uri GraphQlEndpoint { get; set; } = null!;

    [Required]
    public string TokenScope { get; set; } = "S5Api";

    [Required]
    public IReadOnlyCollection<string> AllowedHosts { get; set; } = null!;
}
