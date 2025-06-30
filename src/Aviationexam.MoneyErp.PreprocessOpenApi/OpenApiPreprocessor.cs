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

    private const string Info = "info";
    private static ReadOnlySpan<byte> Version => "version"u8;
    private static ReadOnlySpan<byte> Name => "name"u8;
    private static ReadOnlySpan<byte> In => "in"u8;
    private static ReadOnlySpan<byte> SchemaU8 => "schema"u8;
    private static ReadOnlySpan<byte> Type => "type"u8;
    private static ReadOnlySpan<byte> Format => "format"u8;
    private const string Get = "get";
    private const string Paths = "paths";
    private const string Parameters = "parameters";
    public const string Path = "path";
    private const string Schema = "schema";

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
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
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
        var collectedMetadata = new CollectedMetadata();
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
                case JsonTokenType.StartObject:
                    currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
                    lastProperty = default;

                    break;
                case JsonTokenType.EndArray:
                case JsonTokenType.EndObject:
                    {
                        var treeItem = currentPath.Pop();

                        if (
                            treeItem is { JsonTokenType : JsonTokenType.StartObject, PropertyName : null }
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

                    writer.WritePropertyName(reader.ValueSpan);

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

                        writer.WritePropertyName("original-schema"u8);
                    }

                    static bool IsIgnorePath(
                        ref Utf8JsonReader reader, CollectedMetadata currentPath, Stack<TreeItem> treeItems
                    ) => treeItems.Count == 2
                         && treeItems.ToArray() is
                         [
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                         ]
                         && currentPath.IsPathIgnored(reader.ValueSpan);

                    static bool IsIgnorePathMethod(
                        ref Utf8JsonReader reader, CollectedMetadata currentPath, Stack<TreeItem> treeItems
                    ) => treeItems.Count == 3
                         && treeItems.ToArray() is
                         [
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                             { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                         ]
                         && currentPath.IsPathMethodIgnored(pathName, reader.ValueSpan);

                    break;

                case JsonTokenType.StartArray:
                    currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
                    lastProperty = default;

                    writer.WriteStartArray();

                    break;
                case JsonTokenType.StartObject:
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
                    else
                    {
                        currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
                    }

                    lastProperty = default;

                    writer.WriteStartObject();

                    break;
                case JsonTokenType.EndArray:
                    currentPath.Pop();
                    lastProperty = default;

                    writer.WriteEndArray();

                    break;
                case JsonTokenType.EndObject:
                    currentPath.Pop();
                    lastProperty = default;

                    writer.WriteEndObject();

                    break;
                case JsonTokenType.String:
                    if (
                        currentPath.Count == 6
                        && currentPath.First() is ParameterTreeItem { JsonTokenType : JsonTokenType.StartObject } parameterTreeItem
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
}
