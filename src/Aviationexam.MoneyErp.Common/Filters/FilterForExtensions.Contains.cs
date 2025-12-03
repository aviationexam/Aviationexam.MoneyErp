using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterForExtensions
{
    extension<T>(FilterFor<T> filterFor) where T : class
    {
        public static string Contains(
            Expression<Func<T, string>> property, string value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.Contains, FilterFor<T>.GetPropertyName(property), value
        );
    }
}
