using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoviesBot
{
    public class SecretsKeeper
    {
        /// <summary>
        /// Secrets file name
        /// </summary>
        public const string SECRETS_FILE_NAME = "secrets.json";

        /// <summary>
        /// Bot token for accessing Telegram API
        /// </summary>
        [JsonPropertyName("api_token")] public string APIToken { get; set; }

        /// <summary>
        /// Do not use the standard class constructor.
        /// Instead, use the static Create() method to properly create an instance of the secrets class
        /// </summary>
        [Obsolete]
        public SecretsKeeper() { }

        /// <summary>
        /// Creates an instance of the secrets class by reading data from a json file.
        /// </summary>
        /// <returns>SecretsKeeper or null</returns>
        public static SecretsKeeper Create()
        {
            SecretsKeeper sk = new SecretsKeeper();

            using (StreamReader sr = new StreamReader($"{Directory.GetCurrentDirectory()}\\{SECRETS_FILE_NAME}"))
            {
                string data = sr.ReadToEnd();
                sk = JsonSerializer.Deserialize<SecretsKeeper>(data);
                return sk;
            }
        }
    }
}
