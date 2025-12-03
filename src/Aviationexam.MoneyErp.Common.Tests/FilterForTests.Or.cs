using System;
using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void OrAgregateWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            EFilterOperator.Equal,
            m => m.AProperty,
            [
                "AProperty",
                "BProperty",
            ]
        );

        Assert.Equal(
            "AProperty~eq~AProperty|AProperty~eq~BProperty",
            filter
        );
    }

    [Fact]
    public void OrWithEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            x => x.Equal(m => m.AProperty, "AValue"),
            x => x.Equal(m => m.IntProperty, 42),
            x => x.Equal(m => m.LongProperty, 42L),
            x => x.Equal(m => m.BoolProperty, true),
            x => x.Equal(m => m.DateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.Equal(m => m.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.Equal(m => m.DateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.Equal(m => m.NullableDateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.Equal(m => m.DateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.Equal(m => m.NullableDateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.Equal(m => m.NullableIntProperty, 10),
            x => x.Equal(m => m.NullableLongProperty, 10L),
            x => x.Equal(m => m.NullableBoolProperty, false)
        );

        Assert.Equal(
            "AProperty~eq~AValue|IntProperty~eq~42|LongProperty~eq~42|BoolProperty~eq~true|DateTimeOffsetProperty~eq~2023-05-15|NullableDateTimeOffsetProperty~eq~2023-05-15|DateTimeProperty~eq~2023-05-15|NullableDateTimeProperty~eq~2023-05-15|DateOnlyProperty~eq~2023-05-15|NullableDateOnlyProperty~eq~2023-05-15|NullableIntProperty~eq~10|NullableLongProperty~eq~10|NullableBoolProperty~eq~false",
            filter
        );
    }

    [Fact]
    public void OrWithNotEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            x => x.NotEqual(m => m.AProperty, "AValue"),
            x => x.NotEqual(m => m.IntProperty, 42),
            x => x.NotEqual(m => m.LongProperty, 42L),
            x => x.NotEqual(m => m.BoolProperty, true),
            x => x.NotEqual(m => m.DateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.NotEqual(m => m.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.NotEqual(m => m.DateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.NotEqual(m => m.NullableDateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.NotEqual(m => m.DateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.NotEqual(m => m.NullableDateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.NotEqual(m => m.NullableIntProperty, 10),
            x => x.NotEqual(m => m.NullableLongProperty, 10L),
            x => x.NotEqual(m => m.NullableBoolProperty, false)
        );

        Assert.Equal(
            "AProperty~ne~AValue|IntProperty~ne~42|LongProperty~ne~42|BoolProperty~ne~true|DateTimeOffsetProperty~ne~2023-05-15|NullableDateTimeOffsetProperty~ne~2023-05-15|DateTimeProperty~ne~2023-05-15|NullableDateTimeProperty~ne~2023-05-15|DateOnlyProperty~ne~2023-05-15|NullableDateOnlyProperty~ne~2023-05-15|NullableIntProperty~ne~10|NullableLongProperty~ne~10|NullableBoolProperty~ne~false",
            filter
        );
    }

    [Fact]
    public void OrWithLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            x => x.LessThan(m => m.IntProperty, 100),
            x => x.LessThan(m => m.LongProperty, 200L),
            x => x.LessThan(m => m.DateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThan(m => m.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThan(m => m.DateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThan(m => m.NullableDateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThan(m => m.DateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThan(m => m.NullableDateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThan(m => m.NullableIntProperty, 10),
            x => x.LessThan(m => m.NullableLongProperty, 10L)
        );

        Assert.Equal(
            "IntProperty~lt~100|LongProperty~lt~200|DateTimeOffsetProperty~lt~2023-05-15|NullableDateTimeOffsetProperty~lt~2023-05-15|DateTimeProperty~lt~2023-05-15|NullableDateTimeProperty~lt~2023-05-15|DateOnlyProperty~lt~2023-05-15|NullableDateOnlyProperty~lt~2023-05-15|NullableIntProperty~lt~10|NullableLongProperty~lt~10",
            filter
        );
    }

    [Fact]
    public void OrWithLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            x => x.LessThanOrEqual(m => m.IntProperty, 100),
            x => x.LessThanOrEqual(m => m.LongProperty, 200L),
            x => x.LessThanOrEqual(m => m.DateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThanOrEqual(m => m.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThanOrEqual(m => m.DateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThanOrEqual(m => m.NullableDateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThanOrEqual(m => m.DateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThanOrEqual(m => m.NullableDateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.LessThanOrEqual(m => m.NullableIntProperty, 10),
            x => x.LessThanOrEqual(m => m.NullableLongProperty, 10L)
        );

        Assert.Equal(
            "IntProperty~lte~100|LongProperty~lte~200|DateTimeOffsetProperty~lte~2023-05-15|NullableDateTimeOffsetProperty~lte~2023-05-15|DateTimeProperty~lte~2023-05-15|NullableDateTimeProperty~lte~2023-05-15|DateOnlyProperty~lte~2023-05-15|NullableDateOnlyProperty~lte~2023-05-15|NullableIntProperty~lte~10|NullableLongProperty~lte~10",
            filter
        );
    }

    [Fact]
    public void OrWithGreaterThanWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            x => x.GreaterThan(m => m.IntProperty, 100),
            x => x.GreaterThan(m => m.LongProperty, 200L),
            x => x.GreaterThan(m => m.DateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThan(m => m.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThan(m => m.DateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThan(m => m.NullableDateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThan(m => m.DateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThan(m => m.NullableDateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThan(m => m.NullableIntProperty, 10),
            x => x.GreaterThan(m => m.NullableLongProperty, 10L)
        );

        Assert.Equal(
            "IntProperty~gt~100|LongProperty~gt~200|DateTimeOffsetProperty~gt~2023-05-15|NullableDateTimeOffsetProperty~gt~2023-05-15|DateTimeProperty~gt~2023-05-15|NullableDateTimeProperty~gt~2023-05-15|DateOnlyProperty~gt~2023-05-15|NullableDateOnlyProperty~gt~2023-05-15|NullableIntProperty~gt~10|NullableLongProperty~gt~10",
            filter
        );
    }

    [Fact]
    public void OrWithGreaterThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            x => x.GreaterThanOrEqual(m => m.IntProperty, 100),
            x => x.GreaterThanOrEqual(m => m.LongProperty, 200L),
            x => x.GreaterThanOrEqual(m => m.DateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThanOrEqual(m => m.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThanOrEqual(m => m.DateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThanOrEqual(m => m.NullableDateTimeProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThanOrEqual(m => m.DateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThanOrEqual(m => m.NullableDateOnlyProperty, new DateOnly(2023, 5, 15)),
            x => x.GreaterThanOrEqual(m => m.NullableIntProperty, 10),
            x => x.GreaterThanOrEqual(m => m.NullableLongProperty, 10L)
        );

        Assert.Equal(
            "IntProperty~gte~100|LongProperty~gte~200|DateTimeOffsetProperty~gte~2023-05-15|NullableDateTimeOffsetProperty~gte~2023-05-15|DateTimeProperty~gte~2023-05-15|NullableDateTimeProperty~gte~2023-05-15|DateOnlyProperty~gte~2023-05-15|NullableDateOnlyProperty~gte~2023-05-15|NullableIntProperty~gte~10|NullableLongProperty~gte~10",
            filter
        );
    }

    [Fact]
    public void OrWithStringOperatorsWorks()
    {
        var filter = FilterFor<ApiModel>.Or(
            x => x.StartWith(m => m.AProperty, "A"),
            x => x.Contains(m => m.AProperty, "Val"),
            x => x.EndWith(m => m.AProperty, "ue"),
            x => x.Empty(m => m.AProperty),
            x => x.NotEmpty(m => m.AProperty)
        );

        Assert.Equal("AProperty~sw~A|AProperty~ct~Val|AProperty~ew~ue|AProperty~eq~|AProperty~ne~", filter);
    }
}
