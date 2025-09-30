using Aviationexam.MoneyErp.RestApi.ClientV2;
using Aviationexam.MoneyErp.RestApi.ClientV2.Models.ApiCore.Services.Connection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZLinq;

namespace Aviationexam.MoneyErp.RestApi.Extensions;

public static class ConnectionRequestBuilderExtensions
{
    public sealed record Connection(
        Guid Id,
        Guid? PersonId,
        Guid? CompanyId
    );

    public static async Task<IReadOnlyCollection<Connection>> GetConnectionAsync(
        this MoneyErpApiV2Client requestBuilder,
        Guid connectionType,
        string value,
        CancellationToken cancellationToken
    )
    {
        var connectionRequestInformation = requestBuilder.AddCustomQueryParameters(
            requestBuilder.V20.Connection.ToGetRequestInformation(),
            queryParameterBuilder => queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter", string.Join('#',
                $"TypSpojeni_ID~eq~{connectionType}",
                $"{nameof(ConnectionOutputDto.SpojeniCislo)}~eq~{value}"
            )))
        );

        var connections = await requestBuilder.V20.Connection.GetAsync(connectionRequestInformation, cancellationToken: cancellationToken);

        return connections?.Data?.AsValueEnumerable()
            .Select(x => new Connection(
                x.ID!.Value,
                null,
                x.FirmaID
            ))
            .ToList() ?? [];
    }

    public static async Task<Guid?> CreateConnectionAsync(
        this MoneyErpApiV2Client requestBuilder,
        Guid connectionType,
        string value,
        CancellationToken cancellationToken
    )
    {
        var response = await requestBuilder.V20.Connection.PostAsync(
            [
                new ConnectionInputDto
                {
                    FirmaID = null,
                    SpojeniCislo = value,
                    TypSpojeniID = connectionType,
                },
            ],
            cancellationToken: cancellationToken
        );

        return response?.Data?.AsValueEnumerable().FirstOrDefault();
    }
}
