using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> LessThanOrEqual<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => FilterFor<T>.LessThanOrEqual(
        property, value, format, provider
    );

    public ReadOnlySpan<char> LessThanOrEqual<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => FilterFor<T>.LessThanOrEqual(
        property, value, format, provider
    );

    public ReadOnlySpan<char> LessThanOrEqual(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => FilterFor<T>.LessThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> LessThanOrEqual(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => FilterFor<T>.LessThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> LessThanOrEqual(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => FilterFor<T>.LessThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> LessThanOrEqual(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => FilterFor<T>.LessThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> LessThanOrEqual(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => FilterFor<T>.LessThanOrEqual(
        property, value
    );

    public ReadOnlySpan<char> LessThanOrEqual(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => FilterFor<T>.LessThanOrEqual(
        property, value
    );
}
