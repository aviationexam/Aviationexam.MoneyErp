using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void AndWorks()
    {
        var filter = FilterFor<ApiModel>.And(
            x => x.Equal(m => m.AProperty, "AValue"),
            x => x.GreaterThan(m => m.IntProperty, 22)
        );

        Assert.Equal("AProperty~eq~AValue#IntProperty~gt~22", filter);
    }
}
