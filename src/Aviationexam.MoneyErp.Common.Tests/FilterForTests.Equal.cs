using System;
using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void StringEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "AValue");

        Assert.Equal("AProperty~eq~AValue", filter);
    }

    [Fact]
    public void IntEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.IntProperty, 42);

        Assert.Equal("IntProperty~eq~42", filter);
    }

    [Fact]
    public void LongEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.LongProperty, 42L);

        Assert.Equal("LongProperty~eq~42", filter);
    }

    [Fact]
    public void BoolEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.BoolProperty, true);

        Assert.Equal("BoolProperty~eq~true", filter);
    }

    [Fact]
    public void BoolEqualFalseWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.BoolProperty, false);

        Assert.Equal("BoolProperty~eq~false", filter);
    }

    [Fact]
    public void DateTimeOffsetEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.DateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeOffsetProperty~eq~2023-05-15", filter);
    }

    [Fact]
    public void DateTimeEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.DateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeProperty~eq~2023-05-15", filter);
    }

    [Fact]
    public void DateOnlyEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.DateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateOnlyProperty~eq~2023-05-15", filter);
    }

    [Fact]
    public void NullableIntEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.NullableIntProperty, 42);

        Assert.Equal("NullableIntProperty~eq~42", filter);
    }

    [Fact]
    public void NullableLongEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.NullableLongProperty, 42L);

        Assert.Equal("NullableLongProperty~eq~42", filter);
    }

    [Fact]
    public void NullableBoolEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.NullableBoolProperty, true);

        Assert.Equal("NullableBoolProperty~eq~true", filter);
    }

    [Fact]
    public void NullableBoolEqualFalseWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.NullableBoolProperty, false);

        Assert.Equal("NullableBoolProperty~eq~false", filter);
    }

    [Fact]
    public void NullableDateTimeOffsetEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeOffsetProperty~eq~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateTimeEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.NullableDateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeProperty~eq~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateOnlyEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.NullableDateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateOnlyProperty~eq~2023-05-15", filter);
    }
}
