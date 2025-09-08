using CandidateTgBot.Helpers;
using CandidateTgBot.Types;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging.Abstractions;

namespace CandidateTgBot.Tests.Helpers;

public class ButtonCallbackParserTests
{
    [Fact]
    public void BuildCommandParsers_ShouldAutoRegisterAllCallbackTypes()
    {
        // Arrange
        var parser = new ButtonCallbackParser(NullLogger<ButtonCallbackParser>.Instance);
        var testGuid = Guid.NewGuid();
        var testGuidBase64 = GuidShort.ToBase64Url(testGuid);
        
        // Act & Assert - Test that all callback types with IParsableCallback<T> are registered
        var testCases = new[]
        {
            ($"vi|{testGuidBase64}", typeof(VacancyInfoCallback)),
            ("ul|", typeof(UpdateVacancyListCallback)),
            ($"ap|{testGuidBase64}", typeof(ApplyForVacancyCallback)),
            ("so|", typeof(ShowOtherVacanciesCallback)),
            ($"cancel_app|{testGuidBase64}", typeof(CancelApplicationCallback)),
            ("start_new_application|", typeof(StartNewApplicationCallback)),
            ("show_status|", typeof(ShowStatusCallback)),
            ($"answer_yes|{testGuidBase64}", typeof(AnswerYesCallback)),
            ($"answer_no|{testGuidBase64}", typeof(AnswerNoCallback)),
            ($"answer_text|{testGuidBase64}", typeof(AnswerTextCallback)),
            ($"send_one_more|{testGuidBase64}", typeof(SendOneMoreApplicationCallback)),
            ("revoke_list|", typeof(ShowRevokeListCallback))
        };
        
        foreach (var (callbackData, expectedType) in testCases)
        {
            var mockCallbackQuery = new Telegram.Bot.Types.CallbackQuery
            {
                Id = "test",
                Data = callbackData,
                From = new Telegram.Bot.Types.User { Id = 123, IsBot = false, FirstName = "Test" }
            };
            
            // Act
            var result = parser.ParseCallback(mockCallbackQuery);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType(expectedType, result);
        }
    }
    
    [Fact]
    public void ParseCallback_WithInvalidCallback_ShouldReturnNull()
    {
        // Arrange
        var parser = new ButtonCallbackParser(NullLogger<ButtonCallbackParser>.Instance);
        var mockCallbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Id = "test",
            Data = "invalid_command|data",
            From = new Telegram.Bot.Types.User { Id = 123, IsBot = false, FirstName = "Test" }
        };
        
        // Act
        var result = parser.ParseCallback(mockCallbackQuery);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void ParseCallback_WithMalformedData_ShouldReturnNull()
    {
        // Arrange
        var parser = new ButtonCallbackParser(NullLogger<ButtonCallbackParser>.Instance);
        var mockCallbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Id = "test",
            Data = "no_separator_data",
            From = new Telegram.Bot.Types.User { Id = 123, IsBot = false, FirstName = "Test" }
        };
        
        // Act
        var result = parser.ParseCallback(mockCallbackQuery);
        
        // Assert
        Assert.Null(result);
    }
}
