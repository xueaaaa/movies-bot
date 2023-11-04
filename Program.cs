using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace MoviesBot
{
    class Program
    {
        private static SecretsKeeper _secrets;
        private static TelegramBotClient _bot;
        private static ReceiverOptions _receiverOptions;
        private static BotContext _ctx;

        public static async Task Main(string[] args)
        {
            _secrets = SecretsKeeper.Create();
            _bot = new TelegramBotClient(_secrets.APIToken);
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                },
                ThrowPendingUpdates = true,
            };
            _ctx = new BotContext();

            var cts = new CancellationTokenSource();

            _bot.StartReceiving(Handlers.Update, Handlers.Error, _receiverOptions, cts.Token);

            var me = await _bot.GetMeAsync();
            Logger.Print(new Log($"{me.FirstName} successfully launched", LogLevel.Info));

            Console.ReadLine();
        }
    }
}