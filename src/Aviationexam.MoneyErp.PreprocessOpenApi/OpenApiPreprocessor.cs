using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public class OpenApiPreprocessor(
    string source,
    string target
)
{
    private static ReadOnlySpan<byte> Utf8Bom => [0xEF, 0xBB, 0xBF];
    private static ReadOnlySpan<byte> PaginationStatus => "PaginationStatus"u8;

    private const string Info = "info";
    private static ReadOnlySpan<byte> Version => "version"u8;
    private static ReadOnlySpan<byte> Name => "name"u8;
    private static ReadOnlySpan<byte> In => "in"u8;
    private static ReadOnlySpan<byte> SchemaU8 => "schema"u8;
    private static ReadOnlySpan<byte> Type => "type"u8;
    private static ReadOnlySpan<byte> Format => "format"u8;
    private static ReadOnlySpan<byte> Array => "array"u8;
    private static ReadOnlySpan<byte> ItemsU8 => "items"u8;
    private static ReadOnlySpan<byte> Ref => "$ref"u8;
    private const string Get = "get";
    private const string Paths = "paths";
    private const string Parameters = "parameters";
    public const string Path = "path";
    private const string Schema = "schema";
    private const string Content = "content";
    private const string Responses = "responses";
    private const string Items = "items";
    private const string Schemas = "schemas";
    private const string Components = "components";
    private const string Properties = "properties";

    public void Preprocess()
    {
        ReadOnlySpan<byte> jsonReadOnlySpan = File.ReadAllBytes(source);

        // Read past the UTF-8 BOM bytes if a BOM exists.
        if (jsonReadOnlySpan.StartsWith(Utf8Bom))
        {
            jsonReadOnlySpan = jsonReadOnlySpan[Utf8Bom.Length..];
        }

        var readerOptions = new JsonReaderOptions
        {
            AllowTrailingCommas = false,
            CommentHandling = JsonCommentHandling.Disallow,
        };

        var reader = new Utf8JsonReader(jsonReadOnlySpan, readerOptions);

        var writerOptions = new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.Default,
            Indented = true,
        };

        using var targetStream = new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        targetStream.SetLength(0);
        using var writer = new Utf8JsonWriter(targetStream, writerOptions);

        var collectedMetadata = Collect(reader);

        Preprocess(ref reader, collectedMetadata, writer);
    }

    private CollectedMetadata Collect(Utf8JsonReader reader)
    {
        var collectedMetadata = new CollectedMetadata(new HashSet<string>([
            "/v1.0/IssuedInvoice/{id}/Report/{reportConfigId}",
            "/v1.0/PrepaymentInvoice/{id}/Report/{reportConfigId}",
            "/v1.0/PrepaymentIssuedInvoice/{id}/Report/{reportConfigId}",
            "/v1.0/ReceivedInvoice/{id}/Report/{reportConfigId}",
        ]));

        var currentPath = new Stack<TreeItem>();
        ReadOnlySpan<byte> lastProperty = default;

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    {
                        lastProperty = reader.ValueSpan;

                        if (
                            currentPath.Count == 4
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } methodName },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.AddKnownPathMethod(pathName, methodName);

                            if (
                                lastProperty.SequenceEqual("requestBody"u8)
                                && methodName == Get
                            )
                            {
                                collectedMetadata.SkipPathMethod(pathName, methodName);
                            }
                        }
                    }

                    break;

                case JsonTokenType.StartArray:
                    currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
                    lastProperty = default;

                    break;
                case JsonTokenType.StartObject:
                    PushCurrentObjectItem(ref reader, currentPath, lastProperty);

                    lastProperty = default;

                    break;

                case JsonTokenType.EndArray:
                case JsonTokenType.EndObject:
                    {
                        var treeItem = currentPath.Pop();

                        if (
                            treeItem is { JsonTokenType: JsonTokenType.StartObject, PropertyName: null }
                            && currentPath.Count == 5
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartArray, PropertyName: Parameters },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.CommitParameter(pathName);
                        }

                        lastProperty = default;
                    }

                    break;
                case JsonTokenType.String:
                    {
                        if (
                            lastProperty.SequenceEqual(Version)
                            && currentPath.Count == 2
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Info },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.AddVersion(reader.ValueSpan);
                        }

                        if (
                            lastProperty.SequenceEqual(In)
                            && currentPath.Count == 6
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                                { JsonTokenType: JsonTokenType.StartArray, PropertyName: Parameters },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.AddParameterIn(reader.ValueSpan);
                        }

                        if (
                            lastProperty.SequenceEqual(Name)
                            && currentPath.Count == 6
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                                { JsonTokenType: JsonTokenType.StartArray, PropertyName: Parameters },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.AddParameterName(reader.ValueSpan);
                        }

                        if (
                            lastProperty.SequenceEqual(Type)
                            && currentPath.Count == 7
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Schema },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                                { JsonTokenType: JsonTokenType.StartArray, PropertyName: Parameters },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.AddParameterType(reader.ValueSpan);
                        }

                        if (
                            lastProperty.SequenceEqual(Format)
                            && currentPath.Count == 7
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Schema },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                                { JsonTokenType: JsonTokenType.StartArray, PropertyName: Parameters },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.AddParameterStringFormat(reader.ValueSpan);
                        }

                        if (
                            lastProperty.SequenceEqual(Type)
                            && reader.ValueSpan.SequenceEqual(Array)
                            && currentPath.Count == 9
                            && currentPath.ToArray() is
                            [
                                SchemaTreeItem { JsonTokenType: JsonTokenType.StartObject, PropertyName: Schema } schemaTreeItem,
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Content },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Responses },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            schemaTreeItem.IsArray = true;
                        }

                        if (
                            lastProperty.SequenceEqual(Ref)
                            && currentPath.Count == 10
                            && currentPath.ToArray() is
                            [
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Items },
                                SchemaTreeItem { JsonTokenType: JsonTokenType.StartObject, PropertyName: Schema, IsArray: true },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Content },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Responses },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } methodName },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                            ]
                        )
                        {
                            collectedMetadata.AddPaginatedResponse(pathName, methodName, reader.ValueSpan);
                        }
                    }
                    break;
                case JsonTokenType.Number:

                    break;
                case JsonTokenType.True:

                    break;
                case JsonTokenType.False:

                    break;
                case JsonTokenType.Null:

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(reader.TokenType), reader.TokenType, null);
            }
        }

        return collectedMetadata;
    }

    private void Preprocess(ref Utf8JsonReader reader, CollectedMetadata collectedMetadata, Utf8JsonWriter writer)
    {
        var currentPath = new Stack<TreeItem>();
        ReadOnlySpan<byte> lastProperty = default;

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    lastProperty = reader.ValueSpan;

                    if (
                        IsIgnorePath(ref reader, collectedMetadata, currentPath)
                        || IsIgnorePathMethod(ref reader, collectedMetadata, currentPath)
                    )
                    {
                        reader.Skip();

                        break;
                    }

                    if (
                        currentPath.Count == 5
                        && currentPath.ToArray() is
                        [
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Properties },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Schemas },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Components },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                        ]
                    )
                    {
                        var propertyNameAsString = Encoding.UTF8.GetString(lastProperty);

                        propertyNameAsString = propertyNameAsString switch
                        {
                            "id" => "ID",
                            "ic" => "IC",
                            "dic" => "DIC",
                            "icdph" => "ICDPH",
                            "IdDatum" => "IdDatum",
                            "idDopravaTuzemsko" => "IDDopravaTuzemsko",
                            "idDopravaZahranici" => "IDDopravaZahranici",
                            "idKrajPuvodu_ID" => "IDKrajPuvodu_ID",
                            "idOvlivnujeIntrastat" => "IDOvlivnujeIntrastat",
                            "idPovahaTransakce_ID" => "IDPovahaTransakce_ID",
                            _ => propertyNameAsString,
                        };

                        if (char.IsLower(propertyNameAsString[0]))
                        {
                            var propertyName = propertyNameAsString.ToCharArray().AsSpan();
                            propertyName[0] = char.ToUpperInvariant(propertyName[0]);

                            propertyNameAsString = propertyName.ToString();
                        }

                        writer.WritePropertyName(propertyNameAsString);
                    }
                    else
                    {
                        writer.WritePropertyName(reader.ValueSpan);
                    }

                {
                    if (
                        lastProperty.SequenceEqual(SchemaU8)
                        && currentPath.Count == 8
                        && currentPath.ToArray() is
                        [
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Content },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Responses },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } methodName },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                        ]
                        && collectedMetadata.IsPaginatedResponse(pathName, methodName, out var itemsRef)
                    )
                    {
                        writer.WriteStartObject();

                        writer.WritePropertyName(Type);
                        writer.WriteStringValue("object"u8);

                        writer.WritePropertyName("required"u8);
                        {
                            writer.WriteStartArray();

                            writer.WriteStringValue("RowCount"u8);
                            writer.WriteStringValue("PageCount"u8);
                            writer.WriteStringValue("Message"u8);
                            writer.WriteStringValue("StackTrace"u8);
                            writer.WriteStringValue("Status"u8);
                            writer.WriteStringValue("Data"u8);

                            writer.WriteEndArray();
                        }
                        writer.WritePropertyName("properties"u8);
                        {
                            writer.WriteStartObject();
                            WriteProperty(writer, "RowCount"u8, "integer"u8, "int32"u8);
                            WriteProperty(writer, "PageCount"u8, "integer"u8, "int32"u8);
                            WriteProperty(writer, "Message"u8, "string"u8, []);
                            WriteProperty(writer, "StackTrace"u8, "string"u8, []);

                            writer.WritePropertyName("Status"u8);
                            {
                                writer.WriteStartObject();

                                writer.WritePropertyName(Ref);
                                writer.WriteStringValue([.. "#/components/schemas/"u8, .. PaginationStatus]);
                                writer.WriteEndObject();
                            }

                            writer.WritePropertyName("Data"u8);
                            {
                                writer.WriteStartObject();

                                writer.WritePropertyName(Type);
                                writer.WriteStringValue(Array);

                                writer.WritePropertyName(ItemsU8);
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName(Ref);
                                    writer.WriteStringValue(itemsRef);

                                    writer.WriteEndObject();
                                }

                                writer.WriteEndObject();
                            }

                            writer.WriteEndObject();
                        }

                        writer.WriteEndObject();

                        reader.Skip();
                    }
                }

                {
                    if (
                        lastProperty.SequenceEqual(SchemaU8)
                        && currentPath.Count == 6
                        && currentPath.ToArray() is
                        [
                            ParameterTreeItem { JsonTokenType: JsonTokenType.StartObject, PropertyName: null, In: Path, Name: { } parameterName },
                            { JsonTokenType: JsonTokenType.StartArray, PropertyName: Parameters },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                        ]
                        && collectedMetadata.ModifyPathParameterType(
                            pathName,
                            parameterName,
                            out var parameterType
                        )
                    )
                    {
                        writer.WriteStartObject();

                        writer.WritePropertyName(Type);
                        writer.WriteStringValue(parameterType.Value.Type);

                        if (parameterType.Value.Format is not null)
                        {
                            writer.WritePropertyName(Format);
                            writer.WriteStringValue(parameterType.Value.Format);
                        }

                        writer.WriteEndObject();

                        writer.WritePropertyName("x-original-schema"u8);
                    }
                }

                    static bool IsIgnorePath(
                        ref Utf8JsonReader reader, CollectedMetadata collectedMetadata, Stack<TreeItem> currentPath
                    ) => currentPath.Count == 2
                         && currentPath.ToArray() is
                         [
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                         ]
                         && collectedMetadata.IsPathIgnored(reader.ValueSpan);

                    static bool IsIgnorePathMethod(
                        ref Utf8JsonReader reader, CollectedMetadata collectedMetadata, Stack<TreeItem> currentPath
                    ) => currentPath.Count == 3
                         && currentPath.ToArray() is
                         [
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                         ]
                         && collectedMetadata.IsPathMethodIgnored(pathName, reader.ValueSpan);

                    break;

                case JsonTokenType.StartArray:
                    currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
                    lastProperty = default;

                    writer.WriteStartArray();

                    break;
                case JsonTokenType.StartObject:
                    PushCurrentObjectItem(ref reader, currentPath, lastProperty);

                    lastProperty = default;

                    writer.WriteStartObject();

                    break;
                case JsonTokenType.EndArray:
                    currentPath.Pop();
                    lastProperty = default;

                    writer.WriteEndArray();

                    break;
                case JsonTokenType.EndObject:
                    var lastItem = currentPath.Pop();
                    lastProperty = default;

                    if (
                        lastItem is { JsonTokenType: JsonTokenType.StartObject, PropertyName: Schemas }
                        && currentPath.Count == 2
                        && currentPath.ToArray() is
                        [
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: Components },
                            { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                        ]
                    )
                    {
                        writer.WritePropertyName(PaginationStatus);
                        writer.WriteStartObject();

                        writer.WritePropertyName(Type);
                        writer.WriteStringValue("integer"u8);

                        writer.WritePropertyName(Format);
                        writer.WriteStringValue("int32"u8);

                        writer.WritePropertyName("enum"u8);
                        {
                            writer.WriteStartArray();
                            writer.WriteNumberValue(1);
                            writer.WriteNumberValue(2);
                            writer.WriteNumberValue(3);
                            writer.WriteEndArray();
                        }

                        writer.WritePropertyName("x-ms-enum"u8);
                        {
                            writer.WriteStartObject();

                            writer.WritePropertyName("name"u8);
                            writer.WriteStringValue(PaginationStatus);

                            writer.WritePropertyName("modelAsString"u8);
                            writer.WriteBooleanValue(false);

                            writer.WritePropertyName("values"u8);
                            {
                                writer.WriteStartArray();

                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("value"u8);
                                    writer.WriteNumberValue(1);

                                    writer.WritePropertyName("name"u8);
                                    writer.WriteStringValue("OK"u8);

                                    writer.WriteEndObject();
                                }
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("value"u8);
                                    writer.WriteNumberValue(2);

                                    writer.WritePropertyName("name"u8);
                                    writer.WriteStringValue("Warning"u8);

                                    writer.WriteEndObject();
                                }
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("value"u8);
                                    writer.WriteNumberValue(3);

                                    writer.WritePropertyName("name"u8);
                                    writer.WriteStringValue("Error"u8);

                                    writer.WriteEndObject();
                                }

                                writer.WriteEndArray();
                            }

                            writer.WriteEndObject();
                        }

                        writer.WriteEndObject();
                    }

                    writer.WriteEndObject();

                    break;
                case JsonTokenType.String:
                    if (
                        currentPath.Count == 6
                        && currentPath.First() is ParameterTreeItem { JsonTokenType: JsonTokenType.StartObject } parameterTreeItem
                    )
                    {
                        if (lastProperty.SequenceEqual(Name))
                        {
                            parameterTreeItem.Name = Encoding.UTF8.GetString(reader.ValueSpan);
                        }
                        else if (lastProperty.SequenceEqual(In))
                        {
                            parameterTreeItem.In = Encoding.UTF8.GetString(reader.ValueSpan);
                        }
                    }

                    if (reader.HasValueSequence)
                    {
                        writer.WriteRawValue(reader.ValueSequence, skipInputValidation: true);
                    }
                    else
                    {
                        writer.WriteStringValue(reader.ValueSpan);
                    }

                    break;
                case JsonTokenType.Number:

                    writer.WriteRawValue(reader.ValueSpan);

                    break;
                case JsonTokenType.True:
                    writer.WriteBooleanValue(true);

                    break;
                case JsonTokenType.False:
                    writer.WriteBooleanValue(false);

                    break;
                case JsonTokenType.Null:
                    writer.WriteNullValue();

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(reader.TokenType), reader.TokenType, null);
            }
        }
    }

    private void WriteProperty(
        Utf8JsonWriter writer,
        ReadOnlySpan<byte> propertyName,
        ReadOnlySpan<byte> type,
        ReadOnlySpan<byte> format
    )
    {
        writer.WritePropertyName(propertyName);
        writer.WriteStartObject();

        writer.WritePropertyName(Type);
        writer.WriteStringValue(type);

        if (format.IsEmpty is false)
        {
            writer.WritePropertyName(Format);
            writer.WriteStringValue(format);
        }

        writer.WriteEndObject();
    }

    private static void PushCurrentObjectItem(
        ref Utf8JsonReader reader, Stack<TreeItem> currentPath, ReadOnlySpan<byte> lastProperty
    )
    {
        if (
            currentPath.Count == 5
            && currentPath.ToArray() is
            [
                { JsonTokenType: JsonTokenType.StartArray, PropertyName: Parameters },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
            ])
        {
            currentPath.Push(new ParameterTreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
        }
        else if (
            lastProperty.SequenceEqual(SchemaU8)
            && currentPath.Count == 8
            && currentPath.ToArray() is
            [
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Content },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Responses },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: not null },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
            ]
        )
        {
            currentPath.Push(new SchemaTreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
        }
        else
        {
            currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
        }
    }
}
