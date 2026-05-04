using System.Text.Json.Serialization;

namespace uni_chat_backend.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageType
{
    TEXT,
    IMAGE,
    FILE,
    VIDEO,
    AUDIO
}