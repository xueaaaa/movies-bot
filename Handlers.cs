using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoviesBot
{
    internal static class Handlers
    {
        private const string START_COMMAND = "/start";
        private const string SEARCH_COMMAND = "/search";

        internal static async Task Update(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        var msg = update.Message;
                        var chat = msg.Chat;
                        
                        if(msg.Text == START_COMMAND) StartHandler(bot, chat);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.Message, LogLevel.Error);
                Logger.Print(log);
            }
            
        }

        internal static async Task Error(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            Console.WriteLine(ex.Message);
        }

        private static async void StartHandler(ITelegramBotClient bot, Chat chat)
        {
            Logger.Print(new Log($"User {chat.Id} initiated a new dialog with the bot", LogLevel.Info));

            var keyboard = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>()
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("🔎 Поиск", SEARCH_COMMAND)
                }
            });

            await bot.SendPhotoAsync(chat.Id, 
                photo: InputFile.FromString($"https://espanarusa.com/files/autoupload/59/8/53/3wi1lz5h406343.jpg") , 
                caption: "🙌 Привет!\n\n🍿 Этот бот предназначен для поиска фильмов.",
                replyMarkup: keyboard);
        }
    }
}
