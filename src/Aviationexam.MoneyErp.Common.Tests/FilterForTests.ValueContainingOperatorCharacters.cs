using Aviationexam.MoneyErp.Common.Filters;
using System;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    /// <summary>
    /// Reproduces a real-world failure where a street address value contains '#',
    /// which is also the AND operator in the filter DSL.
    ///
    /// Original (anonymized) request had "Street 80A # 17-85" as FaktUlice value.
    /// Without escaping, the '#' was misinterpreted as an AND operator.
    /// </summary>
    [Fact]
    public void AndWithValueContainingHashIsEscaped()
    {
        var filter = FilterFor<ApiModel>.And(
            x => x.StartWith(m => m.AProperty, "ABC123"),
            x => x.Equal(m => m.BProperty, "John Doe"),
            x => x.Equal(m => m.CProperty, "Street 80A # 17-85"),
            x => x.Equal(m => m.DProperty, "Prague"),
            x => x.Equal(m => m.EProperty, "11000"),
            x => x.Equal(m => m.BoolProperty, false),
            x => x.Equal(m => m.AProperty, "b679009e-4d3c-40a0-9031-bc00b24ec9d6"),
            x => x.Empty(m => m.AProperty)
        );

        Assert.Equal(
            @"AProperty~sw~ABC123#BProperty~eq~John Doe#CProperty~eq~Street 80A \# 17-85#DProperty~eq~Prague#EProperty~eq~11000#BoolProperty~eq~false#AProperty~eq~b679009e-4d3c-40a0-9031-bc00b24ec9d6#AProperty~eq~",
            filter
        );
    }

    [Fact]
    public void EqualWithValueContainingHash()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "Street 80A # 17-85");

        Assert.Equal(@"AProperty~eq~Street 80A \# 17-85", filter);
    }

    [Fact]
    public void EqualWithValueContainingTilde()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "value~with~tildes");

        Assert.Equal(@"AProperty~eq~value\~with\~tildes", filter);
    }

    [Fact]
    public void EqualWithValueContainingPipe()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "option A|option B");

        Assert.Equal(@"AProperty~eq~option A\|option B", filter);
    }

    [Fact]
    public void EqualWithValueContainingBackslash()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, @"path\to\file");

        Assert.Equal(@"AProperty~eq~path\\to\\file", filter);
    }

    [Fact]
    public void EqualWithValueContainingMultipleSpecialCharacters()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "a#b|c~d");

        Assert.Equal(@"AProperty~eq~a\#b\|c\~d", filter);
    }

    [Fact]
    public void EqualWithValueWithoutSpecialCharactersIsUnchanged()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "normal value 123");

        Assert.Equal("AProperty~eq~normal value 123", filter);
    }

    [Fact]
    public void EscapeFilterValueReturnsInputWhenNoSpecialChars()
    {
        ReadOnlySpan<char> input = "hello world";
        var result = FilterFor<ApiModel>.EscapeFilterValue(input);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void EscapeFilterValueEscapesAllOperatorCharacters()
    {
        ReadOnlySpan<char> input = @"a#b|c~d\e";
        var result = FilterFor<ApiModel>.EscapeFilterValue(input);

        Assert.Equal(@"a\#b\|c\~d\\e", result);
    }
}
