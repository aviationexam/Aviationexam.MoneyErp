using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Aviationexam.MoneyErp.Common.Filters;

public partial class FilterFor<T>
{
   public string StartWith(
        Expression<Func<T, string>> property, string value
    ) => GetFilterClause(EFilterOperator.StartWith, GetPropertyName(property), value);
}
