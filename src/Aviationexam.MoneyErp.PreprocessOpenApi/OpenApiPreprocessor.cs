using System;
using System.Collections.Generic;
using System.IO;
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
    private const string OneOf = "oneOf";
    private static ReadOnlySpan<byte> Ref => "$ref"u8;
    private const string Get = "get";
    private const string Paths = "paths";
    private const string Properties = "properties";
    private const string Type = "type";
    private const string Enum = "enum";
    private const string Schemas = "schemas";
    private const string Components = "components";

    private static ReadOnlySpan<byte> Deprecated => "deprecated"u8;

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

                    break;

                case JsonTokenType.StartArray:
                case JsonTokenType.StartObject:
                    currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
                    lastProperty = default;

                    break;
                case JsonTokenType.EndArray:
                case JsonTokenType.EndObject:
                    currentPath.Pop();

                    lastProperty = default;

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

                    var isIgnorePath = currentPath.Count == 2
                                       && currentPath.ToArray() is
                                       [
                                           { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                           { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                                       ]
                                       && collectedMetadata.IsPathIgnored(reader.ValueSpan);

                    var isIgnorePathMethod = currentPath.Count == 3
                                             && currentPath.ToArray() is
                                             [
                                                 { JsonTokenType: JsonTokenType.StartObject, PropertyName: { } pathName },
                                                 { JsonTokenType: JsonTokenType.StartObject, PropertyName: Paths },
                                                 { JsonTokenType: JsonTokenType.StartObject, PropertyName: null },
                                             ]
                                             && collectedMetadata.IsPathMethodIgnored(pathName, reader.ValueSpan);

                    if (
                        isIgnorePath
                        || isIgnorePathMethod
                    )
                    {
                        reader.Skip();

                        break;
                    }

                    writer.WritePropertyName(reader.ValueSpan);

                    break;

                case JsonTokenType.StartArray:
                    currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
                    lastProperty = default;

                    writer.WriteStartArray();

                    break;
                case JsonTokenType.StartObject:
                    currentPath.Push(new TreeItem(reader.TokenType, lastProperty.IsEmpty ? null : Encoding.UTF8.GetString(lastProperty)));
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
