using System;
using System.ComponentModel.DataAnnotations;

namespace Aviationexam.MoneyErp.Graphql;

public class MoneyErpGraphQlAuthenticationOptions
{
    [Required]
    public Uri GraphQlEndpoint { get; set; } = null!;
}
