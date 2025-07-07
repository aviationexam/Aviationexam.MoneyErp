using System;
using System.ComponentModel.DataAnnotations;

namespace Aviationexam.MoneyErp.Graphql;

public class MoneyErpGraphqlOptions
{
    public string GraphqlPath { get; set; } = "/graphql";

    [Required]
    public Uri GraphqlEndpoint { get; set; } = null!;
}
