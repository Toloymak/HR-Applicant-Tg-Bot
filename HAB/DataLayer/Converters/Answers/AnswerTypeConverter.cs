using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DataLayer.Dals;

namespace DataLayer.Convertes;

public class AnswerTypeConverter: JsonConverter<IAnswerTypeDal>
{
    private readonly JsonSerializerOptions _subOptions = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };
    
    public override IAnswerTypeDal? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty(nameof(IAnswerTypeDal.Type), out var mt))
        {
            var typeVal = mt.GetString();
            IAnswerTypeDal? result = typeVal switch
            {
                TextAnswerTypeDal.TypeName => Deserialize<TextAnswerTypeDal>(),
                YesNoAnswerTypeDal.TypeName => Deserialize<YesNoAnswerTypeDal>(),
                YesNoWithRequiredCorrectAnswerTypeDal.TypeName => Deserialize<YesNoWithRequiredCorrectAnswerTypeDal>(),
                _ => new TextAnswerTypeDal()
            };

            return result ?? new TextAnswerTypeDal();
			
            T? Deserialize<T>()
                => JsonSerializer.Deserialize<T>(root.GetRawText(), options);
        }

        return new TextAnswerTypeDal();
    }

    public override void Write(
        Utf8JsonWriter writer,
        IAnswerTypeDal value,
        JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), _subOptions);
}