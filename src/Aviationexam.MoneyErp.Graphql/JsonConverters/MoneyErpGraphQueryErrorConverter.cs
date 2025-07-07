using System.Collections.Generic;

namespace Aviationexam.MoneyErp.Graphql.JsonConverters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZeroQL;

public class MoneyErpGraphQueryErrorConverter : JsonConverter<GraphQueryError>
{
    public override GraphQueryError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.String)
        {
            return new GraphQueryError
            {
                Message = reader.GetString()!,
            };
        }

        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected start of object, got {reader.TokenType}");
        }

        string message = null!;
        object[] path = [];
        Dictionary<string, object>? extensions = null;

        string? rawLastPropertyValue = null;
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    rawLastPropertyValue = reader.GetString();

                    break;
                case JsonTokenType.StartArray:
                    if (rawLastPropertyValue is nameof(GraphQueryError.Path))
                    {
                        var pathType = options.GetTypeInfo(path.GetType());
                        path = (object[]) JsonSerializer.Deserialize(ref reader, pathType)!;
                    }

                    break;
                case JsonTokenType.EndArray:
                    // we will deal with array items in following cases
                    break;

                case JsonTokenType.StartObject:
                    if (rawLastPropertyValue is nameof(GraphQueryError.Extensions))
                    {
                        var pathType = options.GetTypeInfo(path.GetType());
                        extensions = (Dictionary<string, object>?) JsonSerializer.Deserialize(ref reader, pathType);
                    }

                    break;
                case JsonTokenType.EndObject:
                    break;

                case JsonTokenType.String:
                    if (rawLastPropertyValue is nameof(GraphQueryError.Message))
                    {
                        message = reader.GetString()!;
                    }

                    break;
                case JsonTokenType.Number:
                    break;
            }
        }

        return new GraphQueryError
        {
            Message = message,
            Path = path,
            Extensions = extensions,
        };
    }

    public override void Write(Utf8JsonWriter writer, GraphQueryError value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(GraphQueryError.Message));
        writer.WriteStringValue(value.Message);

        writer.WritePropertyName(nameof(GraphQueryError.Path));
        writer.WriteStartArray();
        foreach (var pathItem in value.Path)
        {
            var typeInfo = options.GetTypeInfo(pathItem.GetType());
            JsonSerializer.Serialize(writer, pathItem, typeInfo);
        }

        writer.WriteEndArray();

        if (value.Extensions is { } extensions)
        {
            writer.WritePropertyName(nameof(GraphQueryError.Extensions));
            writer.WriteStartObject();
            foreach (var (key, item) in extensions)
            {
                writer.WritePropertyName(key);

                var typeInfo = options.GetTypeInfo(item.GetType());
                JsonSerializer.Serialize(writer, item, typeInfo);
            }

            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}
