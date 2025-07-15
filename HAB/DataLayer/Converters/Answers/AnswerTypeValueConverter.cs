using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DataLayer.Convertes;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.Converters;

public class AnswerTypeValueConverter 
    : ValueConverter<IAnswerTypeDal, string>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        Converters =
        {
            new AnswerTypeConverter()
        }
    };

    public AnswerTypeValueConverter() 
        : base(
            v => JsonSerializer.Serialize(v, Options),
            v => JsonSerializer.Deserialize<IAnswerTypeDal>(v, Options)
                 ?? new TextAnswerTypeDal())
    {
    }
}