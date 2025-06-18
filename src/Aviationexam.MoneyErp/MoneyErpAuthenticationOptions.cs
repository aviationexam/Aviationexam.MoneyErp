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

    [Required]
    public Uri Endpoint { get; set; } = null!;

    [Required]
    public IReadOnlyCollection<string> AllowedHosts { get; set; } = null!;
}
