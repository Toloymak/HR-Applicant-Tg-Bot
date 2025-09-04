using CandidateTgBot.Types.Callbacks;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Services;

public static class TgButtonProvider
{
    public static class Applications
    {
        public static InlineKeyboardButton StartNew
            => InlineKeyboardButton.WithCallbackData(
                text: "🚀 Start New Application",
                callbackData: new StartNewApplicationCallback()
                    .ToTgString().ToString()
            );

        public static InlineKeyboardButton Status
            => InlineKeyboardButton.WithCallbackData(
                text: "📋 Status",
                callbackData: new ShowStatusCallback()
                    .ToTgString().ToString()
            );

        public static InlineKeyboardButton ShowRevokeApplicationMenu
            => InlineKeyboardButton.WithCallbackData(
                text: "🚫 Cancel Application",
                callbackData: new ShowRevokeListCallback()
                    .ToTgString().ToString()
            );

        public static InlineKeyboardButton RemoveApplication(
            string title,
            Guid applicationId)
            => InlineKeyboardButton.WithCallbackData(
                text: $"🗑️ Revoke {title}",
                callbackData: new RevokeSpecificApplicationCallback { ApplicationId = applicationId }
                    .ToTgString().ToString()
            );

        public static InlineKeyboardButton ConfirmCancel(
            Guid applicationId)
            => InlineKeyboardButton.WithCallbackData(
                text: "✅ Yes, Cancel Application",
                callbackData: new ConfirmCancelApplicationCallback
                {
                    ApplicationId = applicationId
                }.ToTgString().ToString()
            );

        public static InlineKeyboardButton CancelRevokeApplication
            => InlineKeyboardButton.WithCallbackData(
                text: "❌ No, Keep Application",
                callbackData: new CancelRevokeApplicationCallback().ToTgString().ToString()
            );
        
        
        // public static InlineKeyboardButton ContinueWorkWithApplication
        //     => InlineKeyboardButton.WithCallbackData(
        //         text: "❌ No, Keep Application",
        //         callbackData: new BackToStatusCallback().ToTgString().ToString()
        //     );
    }


    public static class Vacancy
    {
        public static InlineKeyboardButton UpdateStatus
            => InlineKeyboardButton.WithCallbackData(
                text: "Update list",
                callbackData: new UpdateVacancyListCallback()
                    .ToTgString().ToString()
            );
    }
}