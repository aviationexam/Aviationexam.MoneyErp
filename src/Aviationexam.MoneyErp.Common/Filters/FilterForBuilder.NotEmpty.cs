using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> NotEmpty(
        Expression<Func<T, string?>> property
    ) => FilterFor<T>.NotEmpty(
        property
    );
}
