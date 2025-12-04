using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void StringNotEmptyWorks()
    {
        var filter = FilterFor<ApiModel>.NotEmpty(x => x.AProperty);

        Assert.Equal("AProperty~ne~", filter);
    }
}
