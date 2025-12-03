using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T>
{
    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, string>> property, string value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual<TP>(
        Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : INumberBase<TP> => FilterFor<T>.NotEqual(
        property, value, format, provider
    );

    public ReadOnlySpan<char> NotEqual<TP>(
        Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
    ) where TP : struct, INumberBase<TP> => FilterFor<T>.NotEqual(
        property, value, format, provider
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, bool>> property, bool value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, bool?>> property, bool value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, DateTimeOffset>> property, DateOnly value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, DateTimeOffset?>> property, DateOnly value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, DateTime>> property, DateOnly value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, DateTime?>> property, DateOnly value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, DateOnly>> property, DateOnly value
    ) => FilterFor<T>.NotEqual(
        property, value
    );

    public ReadOnlySpan<char> NotEqual(
        Expression<Func<T, DateOnly?>> property, DateOnly value
    ) => FilterFor<T>.NotEqual(
        property, value
    );
}
