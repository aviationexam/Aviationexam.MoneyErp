using Aviationexam.MoneyErp.Common.Filters;
using Aviationexam.MoneyErp.Graphql.Extensions;
using System;
using Xunit;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public partial class FilterForTests
{
    [Fact]
    public void IdEqualWorks()
    {
        var value = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var filter = FilterFor<GraphqlApiModel>.Equal(x => x.IdProperty, value);

        Assert.Equal("IdProperty~eq~12345678-1234-1234-1234-123456789abc", filter.ToString());
    }

    [Fact]
    public void NullableIdEqualWorks()
    {
        var value = Guid.Parse("87654321-4321-4321-4321-cba987654321");
        var filter = FilterFor<GraphqlApiModel>.Equal(x => x.NullableIdProperty, value);

        Assert.Equal("NullableIdProperty~eq~87654321-4321-4321-4321-cba987654321", filter.ToString());
    }
}
