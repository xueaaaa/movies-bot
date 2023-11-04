using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoviesBot
{
    internal static class Handlers
    {
        internal static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    var msg = update.Message;
                    Console.WriteLine(msg.Text);
                    break;
            }
        }

        internal static async Task Error(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
