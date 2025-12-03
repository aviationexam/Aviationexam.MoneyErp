using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public class FilterForTests
{
    [Fact]
    public void GetPropertyNameWorks()
    {
        var propertyName = FilterFor<ApiModel>.GetPropertyName(x => x.AProperty);

        Assert.Equal("AProperty", propertyName);
    }

    private sealed class ApiModel
    {
        public required string AProperty { get; set; }
    }
}
