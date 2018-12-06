using NotificationProvider.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NotificationProvider.Services
{
    internal class Mailer
    {
        private readonly HttpClient client;

        public Mailer()
        {
            this.client = new HttpClient();
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");
        }

        public async Task<bool> Send(SendGridMail mail)
        {
            string jsonRequest = mail.ToString();

            var result = await this.client.PostAsync("https://api.sendgrid.com/v3/mail/send",
                new StringContent(jsonRequest, System.Text.Encoding.UTF8,
                "application/json"));

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}