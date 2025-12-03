using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterForExtensions
{
    extension<T>(FilterFor<T> filterFor) where T : class
    {
        public static string LessThanOrEqual<TP>(
            Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
        ) where TP : INumberBase<TP> => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value, format, provider
        );

        public static string LessThanOrEqual<TP>(
            Expression<Func<T, TP?>> property, TP value, string? format = null, IFormatProvider? provider = null
        ) where TP : struct, INumberBase<TP> => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value, format, provider
        );

        public static string LessThanOrEqual(
            Expression<Func<T, DateTimeOffset>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );

        public static string LessThanOrEqual(
            Expression<Func<T, DateTimeOffset?>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );

        public static string LessThanOrEqual(
            Expression<Func<T, DateTime>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );

        public static string LessThanOrEqual(
            Expression<Func<T, DateTime?>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );

        public static string LessThanOrEqual(
            Expression<Func<T, DateOnly>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );

        public static string LessThanOrEqual(
            Expression<Func<T, DateOnly?>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.LessThanOrEqual, FilterFor<T>.GetPropertyName(property), value
        );
    }
}
