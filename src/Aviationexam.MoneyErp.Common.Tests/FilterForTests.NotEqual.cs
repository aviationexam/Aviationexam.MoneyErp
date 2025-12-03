using System;
using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void StringNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.AProperty, "AValue");

        Assert.Equal("AProperty~ne~AValue", filter);
    }

    [Fact]
    public void IntNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.IntProperty, 42);

        Assert.Equal("IntProperty~ne~42", filter);
    }

    [Fact]
    public void LongNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.LongProperty, 42);

        Assert.Equal("LongProperty~ne~42", filter);
    }

    [Fact]
    public void BoolNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.BoolProperty, true);

        Assert.Equal("BoolProperty~ne~true", filter);
    }

    [Fact]
    public void BoolNotEqualFalseWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.BoolProperty, false);

        Assert.Equal("BoolProperty~ne~false", filter);
    }

    [Fact]
    public void DateTimeOffsetNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.DateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeOffsetProperty~ne~2023-05-15", filter);
    }

    [Fact]
    public void DateTimeNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.DateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeProperty~ne~2023-05-15", filter);
    }

    [Fact]
    public void DateOnlyNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.DateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateOnlyProperty~ne~2023-05-15", filter);
    }

    [Fact]
    public void NullableIntNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.NullableIntProperty, 42);

        Assert.Equal("NullableIntProperty~ne~42", filter);
    }

    [Fact]
    public void NullableLongNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.NullableLongProperty, 42L);

        Assert.Equal("NullableLongProperty~ne~42", filter);
    }

    [Fact]
    public void NullableBoolNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.NullableBoolProperty, true);

        Assert.Equal("NullableBoolProperty~ne~true", filter);
    }

    [Fact]
    public void NullableBoolNotEqualFalseWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.NullableBoolProperty, false);

        Assert.Equal("NullableBoolProperty~ne~false", filter);
    }

    [Fact]
    public void NullableDateTimeOffsetNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeOffsetProperty~ne~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateTimeNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.NullableDateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeProperty~ne~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateOnlyNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.NotEqual(x => x.NullableDateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateOnlyProperty~ne~2023-05-15", filter);
    }
}
