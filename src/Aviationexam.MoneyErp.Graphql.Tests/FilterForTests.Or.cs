using Aviationexam.MoneyErp.Common.Filters;
using Aviationexam.MoneyErp.Graphql.Extensions;
using System;
using Xunit;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public partial class FilterForTests
{
    [Fact]
    public void OrWorks()
    {
        var filter = FilterFor<GraphqlApiModel>.Or(
            EFilterOperator.Equal,
            m => m.IdProperty,
            [Guid.Parse("12345678-1234-1234-1234-123456789abc"), Guid.Parse("12345678-1234-1234-1234-123456789abd")]
        );

        Assert.Equal("IdProperty~eq~12345678-1234-1234-1234-123456789abc|IdProperty~eq~12345678-1234-1234-1234-123456789abd", filter.ToString());
    }
}
