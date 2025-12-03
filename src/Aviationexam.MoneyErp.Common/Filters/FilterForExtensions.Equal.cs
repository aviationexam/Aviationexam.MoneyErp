using System;
using System.Linq.Expressions;
using System.Numerics;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterForExtensions
{
    extension<T>(FilterFor<T> filterFor) where T : class
    {
        public static string Equal(
            Expression<Func<T, string>> property, string value
        ) => FilterFor<T>.GetFilterClause(EFilterOperator.Equal, FilterFor<T>.GetPropertyName(property), value);

        public static string Equal<TP>(
            Expression<Func<T, TP>> property, TP value, string? format = null, IFormatProvider? provider = null
        ) where TP : INumberBase<TP> => FilterFor<T>.GetFilterClause(
            EFilterOperator.Equal, FilterFor<T>.GetPropertyName(property), value, format, provider
        );

        public static string Equal(
            Expression<Func<T, bool>> property, bool value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.Equal, FilterFor<T>.GetPropertyName(property), value
        );

        public static string Equal(
            Expression<Func<T, DateTimeOffset>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.Equal, FilterFor<T>.GetPropertyName(property), value
        );

        public static string Equal(
            Expression<Func<T, DateTime>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.Equal, FilterFor<T>.GetPropertyName(property), value
        );

        public static string Equal(
            Expression<Func<T, DateOnly>> property, DateOnly value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.Equal, FilterFor<T>.GetPropertyName(property), value
        );
    }
}
