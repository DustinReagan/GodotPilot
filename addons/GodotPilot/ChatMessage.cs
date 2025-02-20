using System.Text.Json;
using System.Text.Json.Serialization;

namespace GodotPilotPlugin
{
    //[JsonConverter(typeof(ChatMessageJsonConverter))]
    public record ChatMessage
    {
        [JsonPropertyName("type")]
        public string? Type { get; init; }  // holds message role, e.g. "ai" or "tool"

        [JsonPropertyName("content")]
        public string? Content { get; init; }  // holds the message text or content

        [JsonPropertyName("tool_calls")]
        public ToolCall[]? ToolCalls { get; init; }  // holds any associated tool calls
    }

    public record ToolCall
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("args")]
        public Dictionary<string, object> Arguments { get; init; } = new();

        [JsonPropertyName("id")]
        public string Id { get; init; } = "";

        [JsonPropertyName("type")]
        public string Type { get; init; } = "";
    }

    public class ChatMessageJsonConverter : JsonConverter<ChatMessage>
    {
        public override ChatMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    // handle case where it's a plain string
                    string content = reader.GetString() ?? "";
                    return new ChatMessage { Type = "ai", Content = content };
                }

                // normal object parsing
                using JsonDocument document = JsonDocument.ParseValue(ref reader);
                return document.RootElement.Deserialize<ChatMessage>(options);
            }
            catch (JsonException)
            {
                // if parsing fails, try reading as raw string
                string rawContent = reader.GetString() ?? "";
                return new ChatMessage { Type = "ai", Content = rawContent };
            }
        }

        public override void Write(Utf8JsonWriter writer, ChatMessage value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}