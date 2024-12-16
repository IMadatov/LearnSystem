using System.Text.Json;
using System.Text.Json.Serialization;

namespace LearnSystem.Extensions.Converters;

public class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<Guid>(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        var valueCopy = value.ToString().ToLower();
        writer.WriteStringValue(valueCopy);
    }
}