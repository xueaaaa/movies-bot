using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoviesBot
{
    internal static class Handlers
    {
        private const string START_COMMAND = "/start";
        private const string SEARCH_COMMAND = "🔎 Поиск";

        private static bool _isWaitingForMovieCode = false;

        internal static async Task Update(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            var msg = update.Message;
            var chat = msg.Chat;

            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        if(msg.Text?.ToLower() == START_COMMAND) StartHandler(bot, chat);
                        if(msg.Text == SEARCH_COMMAND) WaitForCodeHandler(bot, chat);

                        if (_isWaitingForMovieCode)
                        {
                            SearchHandler(bot, chat, Convert.ToInt32(msg.Text));
                            _isWaitingForMovieCode = false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.Message, LogLevel.Error);
                Logger.Print(log);
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Произошла ошибка. Попробуй еще раз.");
            }
            
        }

        internal static async Task Error(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            Console.WriteLine(ex.Message);
        }

        private static async void StartHandler(ITelegramBotClient bot, Chat chat)
        {
            Logger.Print(new Log($"User {chat.Id} initiated a new dialog with the bot", LogLevel.Info));

            var keyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>()
            {
                new KeyboardButton[]
                {
                    new KeyboardButton(SEARCH_COMMAND)
                }
            })
            { 
                ResizeKeyboard = true 
            };

            await bot.SendPhotoAsync(chat.Id, 
                photo: InputFile.FromString($"https://espanarusa.com/files/autoupload/59/8/53/3wi1lz5h406343.jpg") , 
                caption: "🙌 Привет!\n\n🍿 Этот бот предназначен для поиска фильмов.",
                replyMarkup: keyboard);
        }
        
        private static async void WaitForCodeHandler(ITelegramBotClient bot, Chat chat)
        {
            await bot.SendTextMessageAsync(chat.Id, "📌 Введи код фильма");
            _isWaitingForMovieCode = true;
        }

        private static async void SearchHandler(ITelegramBotClient bot, Chat chat, int code)
        {
            var member = await bot.GetChatMemberAsync(Program.Secrets.ChannelId, chat.Id);

            if(member.Status == ChatMemberStatus.Left || member.Status == ChatMemberStatus.Kicked || member.Status == ChatMemberStatus.Restricted)
            {
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Для поиска по коду необходимо быть подписаным на наш телеграм канал (https://t.me/movieskis)!");
                return;
            }

            var movie = Program.Ctx.Movies.FindAsync(code).Result;

            if(movie == null)
            {
                Logger.Print(new Log($"User {chat.Id} was trying to get the movie from the code {code}. It is possible that a movie with this code was deleted in the database for some reason", LogLevel.Warn));
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Фильм с таким кодом не найден!");
                return;
            }

            await bot.SendPhotoAsync(chat.Id, 
                photo: InputFile.FromString(movie.Cover.ToString()),
                caption: $"📽️ {movie.Name} ({movie.Year})\n\n✏️ {movie.Description}\n\n🔗 Ссылка для просмотра: {movie.Link}");
        }
    }
}
