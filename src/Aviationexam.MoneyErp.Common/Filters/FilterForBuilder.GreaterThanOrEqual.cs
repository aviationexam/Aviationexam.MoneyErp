using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> GreaterThanOrEqual<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => FilterFor<T>.GreaterThanOrEqual(
        property, value, format, provider
    );

    public ReadOnlySpan<char> GreaterThanOrEqual<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => FilterFor<T>.GreaterThanOrEqual(
        property, value, format, provider
    );

    public ReadOnlySpan<char> GreaterThanOrEqual(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => FilterFor<T>.GreaterThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> GreaterThanOrEqual(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => FilterFor<T>.GreaterThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> GreaterThanOrEqual(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => FilterFor<T>.GreaterThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> GreaterThanOrEqual(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => FilterFor<T>.GreaterThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> GreaterThanOrEqual(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => FilterFor<T>.GreaterThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> GreaterThanOrEqual(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => FilterFor<T>.GreaterThanOrEqual(
        property, value
    );
}
