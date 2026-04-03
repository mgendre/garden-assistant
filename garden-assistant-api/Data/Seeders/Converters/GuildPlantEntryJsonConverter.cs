using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Records;

namespace GardenAssistant.Data.Seeders.Converters;

public sealed class GuildPlantEntryJsonConverter : JsonConverter<GuildPlantEntry>
{
    public override GuildPlantEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var key = reader.GetString()!;
            return new GuildPlantEntry(key, null);
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            string? key = null;
            GuildPlantRole? role = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString()!;
                    reader.Read();

                    if (string.Equals(propertyName, "key", StringComparison.OrdinalIgnoreCase))
                    {
                        key = reader.GetString();
                    }
                    else if (string.Equals(propertyName, "role", StringComparison.OrdinalIgnoreCase))
                    {
                        var roleStr = reader.GetString();
                        if (roleStr is not null && Enum.TryParse<GuildPlantRole>(roleStr, true, out var parsed))
                        {
                            role = parsed;
                        }
                    }
                }
            }

            return new GuildPlantEntry(
                key ?? throw new JsonException("GuildPlantEntry object must have a 'key' property."),
                role
            );
        }

        throw new JsonException($"Unexpected token type '{reader.TokenType}' when parsing GuildPlantEntry.");
    }

    public override void Write(Utf8JsonWriter writer, GuildPlantEntry value, JsonSerializerOptions options)
    {
        if (value.Role is null)
        {
            writer.WriteStringValue(value.Key);
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteString("key", value.Key);
            writer.WriteString("role", value.Role.Value.ToString());
            writer.WriteEndObject();
        }
    }
}
