using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.Converters;

public class QuestionAnswerValueConverter 
    : ValueConverter<IQuestionAnswerValue, string>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        Converters =
        {
            new QuestionAnswerValueJsonConverter()
        }
    };

    public QuestionAnswerValueConverter() 
        : base(
            v => JsonSerializer.Serialize(v, Options),
            v => JsonSerializer.Deserialize<IQuestionAnswerValue>(v, Options)
                 ?? new TextAnswerValue { Value = string.Empty })
    {
    }
}

public class QuestionAnswerValueJsonConverter : JsonConverter<IQuestionAnswerValue>
{
    private readonly JsonSerializerOptions _subOptions = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };
    
    public override IQuestionAnswerValue? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty(nameof(IQuestionAnswerValue.Type), out var mt))
        {
            var typeVal = mt.GetString();
            IQuestionAnswerValue? result = typeVal switch
            {
                BooleanAnswerValue.TypeName => Deserialize<BooleanAnswerValue>(),
                TextAnswerValue.TypeName => Deserialize<TextAnswerValue>(),
                _ => new TextAnswerValue { Value = string.Empty }
            };

            return result ?? new TextAnswerValue { Value = string.Empty };
			
            T? Deserialize<T>()
                => JsonSerializer.Deserialize<T>(root.GetRawText(), options);
        }

        return new TextAnswerValue { Value = string.Empty };
    }

    public override void Write(
        Utf8JsonWriter writer,
        IQuestionAnswerValue value,
        JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), _subOptions);
}
