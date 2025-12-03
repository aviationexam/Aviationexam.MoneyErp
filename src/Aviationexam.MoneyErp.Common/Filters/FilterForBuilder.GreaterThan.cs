using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> GreaterThan<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => FilterFor<T>.GreaterThan(
        property, value, format, provider
    );

    public ReadOnlySpan<char> GreaterThan<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => FilterFor<T>.GreaterThan(
        property, value, format, provider
    );

    public ReadOnlySpan<char> GreaterThan(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => FilterFor<T>.GreaterThan(
        property, value
    );

    public ReadOnlySpan<char> GreaterThan(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => FilterFor<T>.GreaterThan(
        property, value
    );

    public ReadOnlySpan<char> GreaterThan(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => FilterFor<T>.GreaterThan(
        property, value
    );

    public ReadOnlySpan<char> GreaterThan(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => FilterFor<T>.GreaterThan(
        property, value
    );

    public ReadOnlySpan<char> GreaterThan(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => FilterFor<T>.GreaterThan(
        property, value
    );

    public ReadOnlySpan<char> GreaterThan(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => FilterFor<T>.GreaterThan(
        property, value
    );
}
