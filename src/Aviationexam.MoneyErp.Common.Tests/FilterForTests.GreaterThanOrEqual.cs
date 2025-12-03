using System;
using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void IntGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.IntProperty, 42);

        Assert.Equal("IntProperty~gte~42", filter);
    }

    [Fact]
    public void LongGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.LongProperty, 42);

        Assert.Equal("LongProperty~gte~42", filter);
    }

    [Fact]
    public void DateTimeOffsetGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.DateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeOffsetProperty~gte~2023-05-15", filter);
    }

    [Fact]
    public void DateTimeGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.DateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeProperty~gte~2023-05-15", filter);
    }

    [Fact]
    public void DateOnlyGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.DateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateOnlyProperty~gte~2023-05-15", filter);
    }

    [Fact]
    public void NullableIntGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.NullableIntProperty, 42);

        Assert.Equal("NullableIntProperty~gte~42", filter);
    }

    [Fact]
    public void NullableLongGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.NullableLongProperty, 42L);

        Assert.Equal("NullableLongProperty~gte~42", filter);
    }

    [Fact]
    public void NullableDateTimeOffsetGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeOffsetProperty~gte~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateTimeGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.NullableDateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeProperty~gte~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateOnlyGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThanOrEqual(x => x.NullableDateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateOnlyProperty~gte~2023-05-15", filter);
    }
}
