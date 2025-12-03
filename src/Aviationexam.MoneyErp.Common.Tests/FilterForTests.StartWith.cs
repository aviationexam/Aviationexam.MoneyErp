using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void StringStartWithWorks()
    {
        var filter = FilterFor<ApiModel>.StartWith(x => x.AProperty, "AValue");

        Assert.Equal("AProperty~sw~AValue", filter);
    }

    [Fact]
    public void StringStartWithEmptyWorks()
    {
        var filter = FilterFor<ApiModel>.StartWith(x => x.AProperty, "");

        Assert.Equal("AProperty~sw~", filter);
    }

    [Fact]
    public void StringStartWithPrefixWorks()
    {
        var filter = FilterFor<ApiModel>.StartWith(x => x.AProperty, "A");

        Assert.Equal("AProperty~sw~A", filter);
    }
}
