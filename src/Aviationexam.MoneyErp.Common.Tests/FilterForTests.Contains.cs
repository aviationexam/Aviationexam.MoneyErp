using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void StringContainsWorks()
    {
        var filter = FilterFor<ApiModel>.Contains(x => x.AProperty, "AValue");

        Assert.Equal("AProperty~ct~AValue", filter);
    }

    [Fact]
    public void StringContainsEmptyWorks()
    {
        var filter = FilterFor<ApiModel>.Contains(x => x.AProperty, "");

        Assert.Equal("AProperty~ct~", filter);
    }

    [Fact]
    public void StringContainsSubstringWorks()
    {
        var filter = FilterFor<ApiModel>.Contains(x => x.AProperty, "Val");

        Assert.Equal("AProperty~ct~Val", filter);
    }
}
