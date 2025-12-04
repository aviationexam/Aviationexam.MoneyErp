using System.Diagnostics.CodeAnalysis;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.Tests;

public partial class FilterForTests
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed class GraphqlApiModel
    {
        public required ID? IdProperty { get; set; }

        public ID? NullableIdProperty { get; set; }
    }
}
