using System;
using ZeroQL;

namespace Aviationexam.MoneyErp.Graphql.Extensions;

public static class IdExtensions
{
    public static Guid? AsGuid(
        this ID? id
    ) => id is null ? null : Guid.Parse(id.Value);

    public static Guid? AsGuid(
        this Client.Guid? guid
    ) => guid is null ? null : Guid.Parse(guid.Value);
}
