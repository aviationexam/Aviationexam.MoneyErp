using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void StringEmptyWorks()
    {
        var filter = FilterFor<ApiModel>.Empty(x => x.AProperty);

        Assert.Equal("AProperty~eq~", filter);
    }
}
