using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void StringEndWithWorks()
    {
        var filter = FilterFor<ApiModel>.EndWith(x => x.AProperty, "AValue");

        Assert.Equal("AProperty~ew~AValue", filter);
    }

    [Fact]
    public void StringEndWithEmptyWorks()
    {
        var filter = FilterFor<ApiModel>.EndWith(x => x.AProperty, "");

        Assert.Equal("AProperty~ew~", filter);
    }

    [Fact]
    public void StringEndWithSuffixWorks()
    {
        var filter = FilterFor<ApiModel>.EndWith(x => x.AProperty, "Value");

        Assert.Equal("AProperty~ew~Value", filter);
    }
}
