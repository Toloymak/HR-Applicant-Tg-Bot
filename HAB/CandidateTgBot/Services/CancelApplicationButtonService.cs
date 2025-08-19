using CandidateTgBot.Types.Callbacks;
using DataLayer.Dals;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Services;

public class CancelApplicationButtonService
{
    private readonly ApplicationService _applicationService;
    private readonly ILogger<CancelApplicationButtonService> _logger;

    public CancelApplicationButtonService(
        ApplicationService applicationService,
        ILogger<CancelApplicationButtonService> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Adds cancel application button to existing keyboard if user has active applications
    /// </summary>
    /// <param name="botUserId">Bot user ID</param>
    /// <param name="existingKeyboard">Existing keyboard markup (can be null)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated keyboard markup with cancel button if applicable</returns>
    public async Task<InlineKeyboardMarkup?> AddCancelButtonIfNeededAsync(
        Guid botUserId,
        InlineKeyboardMarkup? existingKeyboard,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user has active applications
            var activeApplication = await _applicationService.GetUserMostRecentActiveApplicationAsync(
                botUserId, cancellationToken);

            if (activeApplication == null)
            {
                return existingKeyboard;
            }

            var cancelButton = InlineKeyboardButton.WithCallbackData(
                text: "🚫 Cancel Application",
                callbackData: new CancelApplicationCallback
                {
                    ApplicationId = activeApplication.Id
                }.ToTgString().ToString()
            );

            // If there's no existing keyboard, create a new one with just the cancel button
            if (existingKeyboard == null)
            {
                return new InlineKeyboardMarkup(new[] { new[] { cancelButton } });
            }

            // Add the cancel button to the existing keyboard
            var existingButtons = existingKeyboard.InlineKeyboard.ToList();
            existingButtons.Add(new[] { cancelButton });
            return new InlineKeyboardMarkup(existingButtons);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding cancel button for user {BotUserId}", botUserId);
            return existingKeyboard;
        }
    }

    /// <summary>
    /// Creates a keyboard with only the cancel application button
    /// </summary>
    /// <param name="botUserId">Bot user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Keyboard with cancel button or null if no active applications</returns>
    public async Task<InlineKeyboardMarkup?> CreateCancelButtonKeyboardAsync(
        Guid botUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var activeApplication = await _applicationService.GetUserMostRecentActiveApplicationAsync(
                botUserId, cancellationToken);

            if (activeApplication == null)
            {
                return null;
            }

            var cancelButton = InlineKeyboardButton.WithCallbackData(
                text: "🚫 Cancel Application",
                callbackData: new CancelApplicationCallback
                {
                    ApplicationId = activeApplication.Id
                }.ToTgString().ToString()
            );

            return new InlineKeyboardMarkup(new[] { new[] { cancelButton } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cancel button keyboard for user {BotUserId}", botUserId);
            return null;
        }
    }

    /// <summary>
    /// Checks if user has active applications that can be canceled
    /// </summary>
    /// <param name="botUserId">Bot user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has cancelable applications</returns>
    public async Task<bool> HasCancelableApplicationsAsync(
        Guid botUserId,
        CancellationToken cancellationToken = default)
    {
        return await _applicationService.HasUserActiveApplicationsAsync(botUserId, cancellationToken);
    }
}
