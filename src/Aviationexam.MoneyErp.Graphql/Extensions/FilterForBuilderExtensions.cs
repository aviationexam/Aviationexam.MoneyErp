using Aviationexam.MoneyErp.Common.Filters;
using System;
using System.Linq.Expressions;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class FilterForBuilderExtensions
{
    extension<T>(FilterForBuilder<T> builder) where T : class
    {
        public ReadOnlySpan<char> Equal(
            Expression<Func<T, ID?>> property, Guid value
        ) => FilterFor<T>.Equal(
            property, value
        );

        public ReadOnlySpan<char> NotEqual(
            Expression<Func<T, ID?>> property, Guid value
        ) => FilterFor<T>.NotEqual(
            property, value
        );
    }
}
