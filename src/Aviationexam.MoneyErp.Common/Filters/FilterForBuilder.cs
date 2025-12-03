using System;

namespace Aviationexam.MoneyErp.Common.Filters;

public readonly partial struct FilterForBuilder<T> where T : class
{
    public delegate ReadOnlySpan<char> Filter(
        FilterForBuilder<T> builder
    );
}
