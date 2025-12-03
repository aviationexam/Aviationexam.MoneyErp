using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterForExtensions
{
    extension<T>(FilterFor<T> filterFor) where T : class
    {
        public static string StartWith(
            Expression<Func<T, string>> property, string value
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.StartWith, FilterFor<T>.GetPropertyName(property), value
        );
    }
}
