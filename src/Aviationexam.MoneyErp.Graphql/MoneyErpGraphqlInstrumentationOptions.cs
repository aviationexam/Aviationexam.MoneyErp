namespace Aviationexam.MoneyErp.Graphql;

/// <summary>
/// Configures what information is included in OpenTelemetry traces for GraphQL operations.
/// Consumers must register the ActivitySource named <see cref="InstrumentationConstants.ActivitySourceName"/>
/// in their OpenTelemetry configuration to receive these traces.
/// </summary>
public class MoneyErpGraphqlInstrumentationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the GraphQL operation name is recorded
    /// as the <c>graphql.operation.name</c> tag on the Activity.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool RecordOperationName { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the GraphQL operation type (query/mutation)
    /// is recorded as the <c>graphql.operation.type</c> tag on the Activity.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool RecordOperationType { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the full GraphQL document is recorded
    /// as the <c>graphql.document</c> tag on the Activity.
    /// Defaults to <c>false</c> because documents can be large and may contain sensitive field names.
    /// </summary>
    public bool RecordDocument { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the GraphQL document hash is recorded
    /// as the <c>graphql.document.hash</c> tag on the Activity.
    /// Useful for correlating operations without storing the full document.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool RecordDocumentHash { get; set; } = true;
}
