using Aviationexam.MoneyErp.Common.Filters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class FilterForExtensions
{
    extension<T>(FilterFor<T>) where T : class
    {
        private static ReadOnlySpan<char> GetPropertyName(
            Expression<Func<T, ID?>> property
        ) => FilterFor<T>.GetPropertyName(property.Body);

        public static ReadOnlySpan<char> Equal(
            Expression<Func<T, ID?>> property, Guid value
        ) => FilterFor<T>.GetFilterClause(EFilterOperator.Equal, GetPropertyName(property), value);

        public static ReadOnlySpan<char> NotEqual(
            Expression<Func<T, ID?>> property, Guid value
        ) => FilterFor<T>.GetFilterClause(EFilterOperator.NotEqual, GetPropertyName(property), value);

        public static ReadOnlySpan<char> Or(
            EFilterOperator filterOperator,
            Expression<Func<T, ID?>> property,
            IReadOnlyCollection<Guid> values
        ) => FilterFor<T>.CombineExpressions(
            FilterFor<T>.OrOperator,
            values,
            value => FilterFor<T>.GetFilterClause(filterOperator, GetPropertyName(property), value)
        );
    }
}
