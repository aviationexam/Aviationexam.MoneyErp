using System;
using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    [Fact]
    public void IntLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.IntProperty, 42);

        Assert.Equal("IntProperty~lt~42", filter);
    }

    [Fact]
    public void LongLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.LongProperty, 42);

        Assert.Equal("LongProperty~lt~42", filter);
    }

    [Fact]
    public void DateTimeOffsetLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.DateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeOffsetProperty~lt~2023-05-15", filter);
    }

    [Fact]
    public void DateTimeLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.DateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateTimeProperty~lt~2023-05-15", filter);
    }

    [Fact]
    public void DateOnlyLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.DateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("DateOnlyProperty~lt~2023-05-15", filter);
    }

    [Fact]
    public void NullableIntLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.NullableIntProperty, 42);

        Assert.Equal("NullableIntProperty~lt~42", filter);
    }

    [Fact]
    public void NullableLongLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.NullableLongProperty, 42L);

        Assert.Equal("NullableLongProperty~lt~42", filter);
    }

    [Fact]
    public void NullableDateTimeOffsetLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.NullableDateTimeOffsetProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeOffsetProperty~lt~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateTimeLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.NullableDateTimeProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateTimeProperty~lt~2023-05-15", filter);
    }

    [Fact]
    public void NullableDateOnlyLessThanWorks()
    {
        var filter = FilterFor<ApiModel>.LessThan(x => x.NullableDateOnlyProperty, new DateOnly(2023, 5, 15));

        Assert.Equal("NullableDateOnlyProperty~lt~2023-05-15", filter);
    }
}
