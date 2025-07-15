using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.Converters;

public class ConditionAnswerValueConverter : ValueConverter<IAnswerCondition, string>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        Converters =
        {
            new ConditionAnswerConverter()
        }
    };

    public ConditionAnswerValueConverter()
        : base(
            v => JsonSerializer.Serialize(v, Options),
            v => JsonSerializer.Deserialize<IAnswerCondition>(v, Options)
                 ?? new YesNoAnswerCondition { Answer = false })
    {
    }
} 