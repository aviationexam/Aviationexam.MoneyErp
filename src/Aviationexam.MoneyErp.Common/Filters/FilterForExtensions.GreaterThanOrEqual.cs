using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterForExtensions
{
    extension<T>(FilterFor<T> filterFor) where T : class
    {
        public static string GreaterThanOrEqual<TP>(
            Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
        ) where TP : INumberBase<TP> => FilterFor<T>.GetFilterClause(
            EFilterOperator.GreaterThanOrEqual, FilterFor<T>.GetPropertyName(property), value, format, provider
        );

        public static string GreaterThanOrEqual(
            Expression<Func<T, DateTimeOffset>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.GreaterThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );

        public static string GreaterThanOrEqual(
            Expression<Func<T, DateTime>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.GreaterThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );

        public static string GreaterThanOrEqual(
            Expression<Func<T, DateOnly>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.GreaterThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );
    }
}
