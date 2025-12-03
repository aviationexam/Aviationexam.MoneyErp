using System;
using System.Linq.Expressions;

namespace Aviationexam.MoneyErp.Common.Filters;

public static partial class FilterForExtensions
{
    extension<T>(FilterFor<T> filterFor) where T : class
    {
        public static string NotEmpty(
            Expression<Func<T, string>> property
        ) => FilterFor<T>.GetFilterClause(
            EFilterOperator.NotEqual, FilterFor<T>.GetPropertyName(property), string.Empty
        );
    }
}
