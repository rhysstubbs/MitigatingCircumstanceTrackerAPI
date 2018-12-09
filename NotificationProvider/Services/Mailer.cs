using NotificationProvider.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NotificationProvider.Services
{
    internal class Mailer
    {
        private readonly HttpClient client;
        private readonly Uri endpoint;

        public Mailer(string endpoint, string bearer)
        {
            this.client = new HttpClient();
            this.endpoint = new Uri(endpoint);
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        public async Task<bool> Send(SendGridMail mail)
        {
            string jsonRequest = mail.ToString();

            var result = await this.client.PostAsync(this.endpoint.ToString(),
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