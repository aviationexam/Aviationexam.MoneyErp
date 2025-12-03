using System;
using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void IntLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.IntProperty, 42);

        Assert.Equal("IntProperty~lte~42", filter);
    }

    [Fact]
    public void LongLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.LongProperty, 42);

        Assert.Equal("LongProperty~lte~42", filter);
    }

    [Fact]
    public void DateTimeOffsetLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.DateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeOffsetProperty~lte~2023-05-15", filter);
    }

    [Fact]
    public void DateTimeLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.DateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeProperty~lte~2023-05-15", filter);
    }

    [Fact]
    public void DateOnlyLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.DateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateOnlyProperty~lte~2023-05-15", filter);
    }

    [Fact]
    public void NullableIntLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.NullableIntProperty, 42);

        Assert.Equal("NullableIntProperty~lte~42", filter);
    }

    [Fact]
    public void NullableLongLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.NullableLongProperty, 42L);

        Assert.Equal("NullableLongProperty~lte~42", filter);
    }

    [Fact]
    public void NullableDateTimeOffsetLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeOffsetProperty~lte~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateTimeLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.NullableDateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeProperty~lte~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateOnlyLessThanOrEqualWorks()
    {
        var filter = FilterFor<ApiModel>.LessThanOrEqual(x => x.NullableDateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateOnlyProperty~lte~2023-05-15", filter);
    }
}
