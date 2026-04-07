using Aviationexam.MoneyErp.Common.Filters;
using Xunit;

namespace Aviationexam.MoneyErp.Common.Tests;

public partial class FilterForTests
{
    /// <summary>
    /// Reproduces a real-world failure where a street address value contains '#',
    /// which is also the AND operator in the filter DSL.
    /// The filter string becomes ambiguous without proper escaping.
    ///
    /// Original (anonymized) request:
    /// AProperty~sw~ABC123#BProperty~eq~John Doe#CProperty~eq~Street 80A # 17-85#DProperty~eq~Prague#EProperty~eq~11000#BoolProperty~eq~false#AProperty~eq~b679009e-4d3c-40a0-9031-bc00b24ec9d6#AProperty~eq~
    ///
    /// The '#' inside "Street 80A # 17-85" is incorrectly parsed as an AND operator.
    /// </summary>
    [Fact]
    public void AndWithValueContainingHashIsAmbiguous()
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

        // TODO: This assert documents the BROKEN output - the '#' inside the street address
        // is indistinguishable from the AND operator. The server will parse 9 clauses instead of 8,
        // splitting "Street 80A # 17-85" into "Street 80A " and an invalid " 17-85".
        Assert.Equal(
            "AProperty~sw~ABC123#BProperty~eq~John Doe#CProperty~eq~Street 80A # 17-85#DProperty~eq~Prague#EProperty~eq~11000#BoolProperty~eq~false#AProperty~eq~b679009e-4d3c-40a0-9031-bc00b24ec9d6#AProperty~eq~",
            filter
        );
    }

    [Fact]
    public void EqualWithValueContainingHash()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "Street 80A # 17-85");

        // Currently produces unescaped output - the '#' will be misinterpreted as AND operator
        // when combined with other filters
        Assert.Equal("AProperty~eq~Street 80A # 17-85", filter);
    }

    [Fact]
    public void EqualWithValueContainingTilde()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "value~with~tildes");

        // The '~' in the value will be misinterpreted as operator delimiter
        Assert.Equal("AProperty~eq~value~with~tildes", filter);
    }

    [Fact]
    public void EqualWithValueContainingPipe()
    {
        var filter = FilterFor<ApiModel>.Equal(x => x.AProperty, "option A|option B");

        // The '|' in the value will be misinterpreted as OR operator
        Assert.Equal("AProperty~eq~option A|option B", filter);
    }
}
