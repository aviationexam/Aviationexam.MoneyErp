using System;
using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void IntGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.IntProperty, 42);

        Assert.Equal("IntProperty~gt~42", filter);
    }

    [Fact]
    public void LongGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.LongProperty, 42L);

        Assert.Equal("LongProperty~gt~42", filter);
    }

    [Fact]
    public void DateTimeOffsetGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.DateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeOffsetProperty~gt~2023-05-15", filter);
    }

    [Fact]
    public void DateTimeGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.DateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeProperty~gt~2023-05-15", filter);
    }

    [Fact]
    public void DateOnlyGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.DateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateOnlyProperty~gt~2023-05-15", filter);
    }

    [Fact]
    public void NullableIntGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.NullableIntProperty, 42);

        Assert.Equal("NullableIntProperty~gt~42", filter);
    }

    [Fact]
    public void NullableLongGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.NullableLongProperty, 42L);

        Assert.Equal("NullableLongProperty~gt~42", filter);
    }

    [Fact]
    public void NullableDateTimeOffsetGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeOffsetProperty~gt~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateTimeGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.NullableDateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeProperty~gt~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateOnlyGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.GreaterThan(x => x.NullableDateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateOnlyProperty~gt~2023-05-15", filter);
    }
}
