using Newtonsoft.Json;
using System.Net.Mail;

namespace NotificationProvider.Models
{
    internal class SendGridMail
    {
        public MailAddress To { get; set; }

        public MailAddress From { get; set; } = new MailAddress("admin@rhysstubbs.services");

        public string Subject { get; set; }

        public string Type { get; set; } = "text/plain";

        public string Content { get; set; } = "This email has no content.";

        public override string ToString() => JsonConvert.SerializeObject(new
        {
            personalizations = new[] {
                new {
                    to = new[]
                    {
                        new {email = this.To.Address.ToString()}
                    },
                    subject = this.Subject
                }
                },
            from = new
            {
                email = this.From.Address.ToString()
            },
            content = new[]
                {
                    new
                    {
                        type = this.Type,
                        value = this.Content
                    }
                }
        });
    }
}