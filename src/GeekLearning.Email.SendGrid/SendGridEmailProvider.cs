namespace GeekLearning.Email.SendGrid
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class SendGridEmailProvider : IEmailProvider
    {
        private readonly string apiUser;
        private readonly string apiKey;

        public SendGridEmailProvider(IEmailProviderOptions options)
        {
            this.apiUser = options.Parameters["User"];
            this.apiKey = options.Parameters["Key"];

            if (string.IsNullOrWhiteSpace(this.apiUser))
            {
                throw new ArgumentNullException("apiUser");
            }

            if (string.IsNullOrWhiteSpace(this.apiKey))
            {
                throw new ArgumentNullException("apiKey");
            }
        }

        public async Task SendEmailAsync(
            IEmailAddress from, 
            IEnumerable<IEmailAddress> recipients, 
            string subject, 
            string text, 
            string html)
        {
            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/api/mail.send.json");

            var variables = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_user", this.apiUser),
                new KeyValuePair<string, string>("api_key", this.apiKey),
                new KeyValuePair<string, string>("from", from.Email),
                new KeyValuePair<string, string>("fromname", from.DisplayName),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("html", html),
            };

            if (recipients.Count() == 1)
            {
                variables.Add(new KeyValuePair<string, string>("to", recipients.First().Email));
                variables.Add(new KeyValuePair<string, string>("toname", recipients.First().DisplayName));
            }
            else
            {
                foreach (var recipient in recipients)
                {
                    variables.Add(new KeyValuePair<string, string>("to[]", recipient.Email));
                    variables.Add(new KeyValuePair<string, string>("toname[]", recipient.DisplayName));
                }
            }

            message.Content = new FormUrlEncodedContent(variables);

            var response = await client.SendAsync(message);
            var responseBody = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Cannot Send Email");
            }
        }
    }
}
