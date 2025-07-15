using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DataLayer.Dals;

namespace DataLayer.Converters;

public class ConditionAnswerConverter : JsonConverter<IAnswerCondition>
{
    private readonly JsonSerializerOptions _subOptions = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };

    public override IAnswerCondition? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty("Type", out var mt))
        {
            var typeVal = mt.GetString();
            IAnswerCondition? result = typeVal switch
            {
                nameof(YesNoAnswerCondition) => Deserialize<YesNoAnswerCondition>(),
                _ => new YesNoAnswerCondition { Answer = false }
            };
            return result ?? new YesNoAnswerCondition { Answer = false };

            T? Deserialize<T>()
                => JsonSerializer.Deserialize<T>(root.GetRawText(), options);
        }
        return new YesNoAnswerCondition { Answer = false };
    }

    public override void Write(
        Utf8JsonWriter writer,
        IAnswerCondition value,
        JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), _subOptions);
} 