using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Aviationexam.MoneyErp.Common;

public sealed class MoneyErpAuthenticationOptions
{
    [Required]
    public string ClientId { get; set; } = null!;

    [Required]
    public string ClientSecret { get; set; } = null!;

    [Required]
    public TimeSpan JwtEarlyExpirationOffset { get; set; }

    public string TokenPath { get; } = "/connect/token";

    [Required]
    public Uri Endpoint { get; set; } = null!;

    public X509Certificate2? EndpointCertificate { get; set; } = null!;

    [Required]
    public Uri TokenEndpoint { get; set; } = null!;

    [Required]
    public string TokenScope { get; set; } = "S5Api";
}
