using System;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class IdExtensions
{
    public static Guid? AsGuid(
        this ID? id
    ) => id is null ? null : Guid.Parse(id.Value);
}
