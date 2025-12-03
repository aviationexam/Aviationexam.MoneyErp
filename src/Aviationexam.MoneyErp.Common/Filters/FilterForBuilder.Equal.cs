using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> Equal(
        Expression<Func<T, string>> property, string value
    )=> FilterFor<T>.Equal(
        property, value
    );
}
