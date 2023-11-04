using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace MoviesBot
{
    class Program
    {
        public static SecretsKeeper Secrets;
        private static TelegramBotClient _bot;
        private static ReceiverOptions _receiverOptions;
        public static BotContext Ctx;

        public static async Task Main(string[] args)
        { 
            Secrets = SecretsKeeper.Create();
            _bot = new TelegramBotClient(Secrets.APIToken);
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                },
                ThrowPendingUpdates = true,
            };
            Ctx = new BotContext();

            var cts = new CancellationTokenSource();

            _bot.StartReceiving(Handlers.Update, Handlers.Error, _receiverOptions, cts.Token);

            var me = await _bot.GetMeAsync();
            Logger.Print(new Log($"{me.FirstName} successfully launched", LogLevel.Info));

            Console.ReadLine();
        }
    }
}