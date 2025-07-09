using Aviationexam.MoneyErp.RestApi.Client;
using Aviationexam.MoneyErp.RestApi.Client.Models.ApiCore.Services.Connection;
using Aviationexam.MoneyErp.RestApi.Client.Models.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZLinq;

namespace Aviationexam.MoneyErp.RestApi.Extensions;

public static class ConnectionRequestBuilderExtensions
{
    public static async Task<Guid?> GetOrCreateConnectionAsync(
        this MoneyErpApiClient requestBuilder,
        Guid connectionType,
        string value,
        CancellationToken cancellationToken
    )
    {
        var connectionRequestInformation = requestBuilder.AddCustomQueryParameters(
            requestBuilder.V10.Connection.ToGetRequestInformation(),
            queryParameterBuilder =>
            {
                queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter.LogicOperator", nameof(LogicOperator.AND)));

                queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter.Filters[0].PropertyName", "TypSpojeni_ID"));
                queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter.Filters[0].Operation", "Equal"));
                queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter.Filters[0].ExpectedValue", connectionType.ToString()));

                queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter.Filters[1].PropertyName", nameof(ConnectionOutputDto.SpojeniCislo)));
                queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter.Filters[1].Operation", "Equal"));
                queryParameterBuilder.Add(KeyValuePair.Create<string, string?>("filter.Filters[1].ExpectedValue", value));
            });

        var connections = await requestBuilder.V10.Connection.GetAsync(connectionRequestInformation, cancellationToken: cancellationToken);
        var connectionId = connections?.Data?.AsValueEnumerable().FirstOrDefault()?.ID;

        if (connectionId is null)
        {
            var response = await requestBuilder.V10.Connection.PostAsync(
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

            connectionId = response?.Data?.AsValueEnumerable().FirstOrDefault();
        }

        return connectionId;
    }
}
