using Aviationexam.MoneyErp.Common.Filters;
using System;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
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

        public int IntProperty { get; set; }

        public int LongProperty { get; set; }

        public bool BoolProperty { get; set; }

        public DateTimeOffset DateTimeOffsetProperty { get; set; }

        public DateTime DateTimeProperty { get; set; }

        public DateOnly DateOnlyProperty { get; set; }
    }
}
