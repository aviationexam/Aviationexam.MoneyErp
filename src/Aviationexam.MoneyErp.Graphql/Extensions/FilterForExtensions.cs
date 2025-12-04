using Aviationexam.MoneyErp.Common.Filters;
using System;
using System.Linq.Expressions;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class FilterForExtensions
{
    extension<T>(FilterFor<T> builder) where T : class
    {
        public static ReadOnlySpan<char> GetFilterClause(
            EFilterOperator filterOperator,
            ReadOnlySpan<char> property,
            Guid value
        ) => FilterFor<T>.GetFilterClause(
            filterOperator, property, value.ToString()
        );

        public static ReadOnlySpan<char> Equal(
            Expression<Func<T, ID?>> property, Guid value
        ) => FilterFor<T>.GetFilterClause(EFilterOperator.Equal, FilterFor<T>.GetPropertyName(property.Body), value);

        public static ReadOnlySpan<char> NotEqual(
            Expression<Func<T, ID?>> property, Guid value
        ) => FilterFor<T>.GetFilterClause(EFilterOperator.NotEqual, FilterFor<T>.GetPropertyName(property.Body), value);
    }
}
