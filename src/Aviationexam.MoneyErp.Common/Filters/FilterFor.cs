using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Aviationexam.MoneyErp.Common.Filters;

public partial class FilterFor<T> where T : class
{
    public string GetFilterClause(
        EFilterOperator filterOperator,
        string property,
        string value
    ) => filterOperator switch
    {
        EFilterOperator.Equal => $"{property}~eq~{value}",
        EFilterOperator.NotEqual => $"{property}~ne~{value}",
        EFilterOperator.LessThan => $"{property}~lt~{value}",
        EFilterOperator.LessThanOrEqual => $"{property}~lte~{value}",
        EFilterOperator.GreaterThan => $"{property}~gt~{value}",
        EFilterOperator.GreaterThanOrEqual => $"{property}~gte~{value}",
        EFilterOperator.StartWith => $"{property}~sw~{value}",
        EFilterOperator.Contains => $"{property}~ct~{value}",
        EFilterOperator.EndWith => $"{property}~ew~{value}",
        _ => throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null),
    };

    internal static string GetPropertyName(
        Expression<Func<T, string>> property
    ) => GetPropertyName(property.Body);

    private static string GetPropertyName(
        Expression expressionBody
    ) => expressionBody switch
    {
        MemberExpression { Member: { MemberType: MemberTypes.Property, Name: { } propertyName } } => propertyName,
        _ => throw new ArgumentOutOfRangeException(nameof(expressionBody), expressionBody, null),
    };
}
