using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> LessThan<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => FilterFor<T>.LessThan(
        property, value, format, provider
    );

    public ReadOnlySpan<char> LessThan<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => FilterFor<T>.LessThan(
        property, value, format, provider
    );

    public ReadOnlySpan<char> LessThan(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => FilterFor<T>.LessThan(
        property, value
    );

    public ReadOnlySpan<char> LessThan(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => FilterFor<T>.LessThan(
        property, value
    );

    public ReadOnlySpan<char> LessThan(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => FilterFor<T>.LessThan(
        property, value
    );

    public ReadOnlySpan<char> LessThan(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => FilterFor<T>.LessThan(
        property, value
    );

    public ReadOnlySpan<char> LessThan(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => FilterFor<T>.LessThan(
        property, value
    );

    public ReadOnlySpan<char> LessThan(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => FilterFor<T>.LessThan(
        property, value
    );
}
