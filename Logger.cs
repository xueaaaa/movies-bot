namespace MoviesBot
{
    /// <summary>
    /// Static class for outputting messages to the console
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Displays a message in the console using formatting
        /// </summary>
        /// <param name="log">Message Instance</param>
        public static void Print(Log log)
        {
            switch (log.Type)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.Write($"[{DateTime.UtcNow} UTC] ");
            Console.ForegroundColor = ConsoleColor.White;

            switch (log.Type)
            {
                case LogLevel.Info:
                    Console.Write("INFO: ");
                    break;
                case LogLevel.Warn:
                    Console.Write("WARNING: ");
                    break;
                case LogLevel.Error:
                    Console.Write("ERROR: ");
                    break;
            }

            Console.WriteLine(log.Message);
        }
    }

    /// <summary>
    /// The message class used by the Logger class to output messages
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Main text of the message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Message type: informational, warning, or error message
        /// </summary>
        public LogLevel Type { get; set; }

        public Log(string message, LogLevel type)
        {
            Message = message;
            Type = type;
        }
    }

    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }
}
