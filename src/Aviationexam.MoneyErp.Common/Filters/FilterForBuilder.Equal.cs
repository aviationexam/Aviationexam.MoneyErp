using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> Equal(
        Expression<Func<T, string?>> property, string value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => FilterFor<T>.Equal(
        property, value, format, provider
    );

    public ReadOnlySpan<char> Equal<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => FilterFor<T>.Equal(
        property, value, format, provider
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, bool>> property, bool value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, bool?>> property, bool value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => FilterFor<T>.Equal(
        property, value
    );

    public ReadOnlySpan<char> Equal(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => FilterFor<T>.Equal(
        property, value
    );
}
