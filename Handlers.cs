using System.Timers;
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

        private const string ADMIN_PANEL_COMMAND = "/admin";
        private const string ADD_MOVIE_COMMAND = "/add_movie";
        private const string DELETE_MOVIE_COMMAND = "/delete_movie";

        private static bool _isWaitingForMovieCode = false;
        private static bool _isWaitingForAddMovieData = false;
        private static bool _isWaitingForDeleteMovieData = false;

        internal static async Task Update(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            var msg = update.Message;
            var chat = msg.Chat;

            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        if (msg.Text == START_COMMAND) StartHandler(bot, chat);
                        if (msg.Text == SEARCH_COMMAND) WaitForCodeHandler(bot, chat);
                        if (msg.Text == ADMIN_PANEL_COMMAND) ShowAdminPanelHandler(bot, chat);

                        if (_isWaitingForMovieCode)
                        {
                            SearchHandler(bot, chat, Convert.ToInt32(msg.Text));
                            _isWaitingForMovieCode = false;
                        }

                        if (msg.Text == ADD_MOVIE_COMMAND) WaitForAddMovieDataHandler(bot, chat);

                        if (_isWaitingForAddMovieData)
                        {
                            AddMovieHandler(bot, chat, msg.Text);
                            _isWaitingForAddMovieData = false;
                        }

                        if(msg.Text == DELETE_MOVIE_COMMAND) WaitForDeleteMovieDataHandler(bot, chat);

                        if(_isWaitingForDeleteMovieData)
                        {
                            DeleteMovieHandler(bot, chat, Convert.ToInt32(msg.Text));
                            _isWaitingForDeleteMovieData = false;
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
            Logger.Print(new Log(ex.Message, LogLevel.Error));
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
                photo: InputFile.FromString($"https://espanarusa.com/files/autoupload/59/8/53/3wi1lz5h406343.jpg"),
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

            if (member.Status == ChatMemberStatus.Left || member.Status == ChatMemberStatus.Kicked || member.Status == ChatMemberStatus.Restricted)
            {
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Для поиска по коду необходимо быть подписанным на наш телеграм канал (https://t.me/movieskis)!");
                return;
            }

            var movie = Program.Ctx.Movies.FindAsync(code).Result;

            if (movie == null)
            {
                Logger.Print(new Log($"User {chat.Id} was trying to get the movie from the code {code}. " +
                    $"It is possible that a movie with this code was deleted in the database for some reason", LogLevel.Warn));
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Фильм с таким кодом не найден!");
                return;
            }

            Logger.Print(new Log($"User {chat.Id} got the movie with the code {code}.", LogLevel.Info));

            await bot.SendPhotoAsync(chat.Id,
                photo: InputFile.FromString(movie.Cover.ToString()),
                caption: $"📽️ {movie.Name} ({movie.Year})\n\n✏️ {movie.Description}\n\n🔗 Ссылка для просмотра: {movie.Link}");
        }

        private static async void ShowAdminPanelHandler(ITelegramBotClient bot, Chat chat)
        {
            if (!IsCreatorOrAdministrator(bot, chat).Result)
            {
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Вы не являетесь создателем для получения доступа к админ-панели.");
                return;
            }

            Logger.Print(new Log($"User {chat.Id} accessed the admin panel", LogLevel.Warn));

            var keyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton(ADD_MOVIE_COMMAND),
                    new KeyboardButton(DELETE_MOVIE_COMMAND)
                }
            })
            {
                ResizeKeyboard = true
            };

            await bot.SendTextMessageAsync(chat.Id, "Доступные комманды:", replyMarkup: keyboard);
        }

        private static async void WaitForAddMovieDataHandler(ITelegramBotClient bot, Chat chat)
        {
            if (!IsCreatorOrAdministrator(bot, chat).Result)
            {
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Вы не являетесь создателем для получения доступа к админ-панели.");
                return;
            }

            await bot.SendTextMessageAsync(chat.Id, "Данные в формате: [Код фильма]\n[название фильма]\n[год выхода]\n[описание]\n[ссылка на просмотр]\n[ссылка на обложку]");
            _isWaitingForAddMovieData = true;
        }

        public static async void AddMovieHandler(ITelegramBotClient bot, Chat chat, string raw)
        {
            if (!IsCreatorOrAdministrator(bot, chat).Result)
            {
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Вы не являетесь создателем для получения доступа к админ-панели.");
                return;
            }

            string[] data = raw.Split('\n');

            Movie movie = new Movie(Convert.ToInt32(data[0]), data[1], data[3], Convert.ToInt32(data[2]), new Uri(data[4]), new Uri(data[5]));
            await movie.Add();
            await bot.SendTextMessageAsync(chat.Id, "Фильм добавлен.");
        }

        private static async void WaitForDeleteMovieDataHandler(ITelegramBotClient bot, Chat chat)
        {
            if (!IsCreatorOrAdministrator(bot, chat).Result)
            {
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Вы не являетесь создателем для получения доступа к админ-панели.");
                return;
            }

            await bot.SendTextMessageAsync(chat.Id, "Данные в формате: [Код фильма]");
            _isWaitingForDeleteMovieData = true;
        }

        public static async void DeleteMovieHandler(ITelegramBotClient bot, Chat chat, int id)
        {
            if (!IsCreatorOrAdministrator(bot, chat).Result)
            {
                await bot.SendTextMessageAsync(chat.Id, "⚠️ Вы не являетесь создателем для получения доступа к админ-панели.");
                return;
            }

            await (await Program.Ctx.Movies.FindAsync(id)).Remove();
            await bot.SendTextMessageAsync(chat.Id, "Фильм удален.");
        }

        private static async Task<bool> IsCreatorOrAdministrator(ITelegramBotClient bot, Chat chat)
        {
            var member = await bot.GetChatMemberAsync(Program.Secrets.ChannelId, chat.Id);

            if (member.Status != ChatMemberStatus.Creator)
                return false;
            else
                return true;
        }
    }
}
