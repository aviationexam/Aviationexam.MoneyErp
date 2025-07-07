using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aviationexam.MoneyErp;

public sealed class MoneyErpRestApiOptions
{
    [Required]
    public IReadOnlyCollection<string> AllowedHosts { get; set; } = null!;
}
