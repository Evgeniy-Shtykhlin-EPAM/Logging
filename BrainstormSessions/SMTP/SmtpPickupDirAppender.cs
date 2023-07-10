using log4net.Appender;
using log4net.Core;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace BrainstormSessions.SMTP
{
    public class SMTPAppender : BufferingAppenderSkeleton
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string SmtpHost { get; set; }
        public string Port { get; set; }

        protected void SendEmail(string messageBody)
        {
            SmtpClient client = new SmtpClient(SmtpHost);
            client.UseDefaultCredentials = false;
            client.Port = int.Parse(Port);
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(From);
                mailMessage.To.Add(To);
                mailMessage.Body = messageBody;
                mailMessage.Subject = Subject;
                client.Send(mailMessage);
            }
        }

        protected override bool RequiresLayout => true;

        protected override void SendBuffer(LoggingEvent[] events)
        {
            StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);

            string t = Layout.Header;
            if (t != null)
            {
                writer.Write(t);
            }

            for (int i = 0; i < events.Length; i++)
            {
                // Render the event and append the text to the buffer
                RenderLoggingEvent(writer, events[i]);
            }

            t = Layout.Footer;
            if (t != null)
            {
                writer.Write(t);
            }

            SendEmail(writer.ToString());
        }


        private static async Task SendEmailAsync()
        {
            MailAddress from = new MailAddress("somemail@gmail.com", "Tom");
            MailAddress to = new MailAddress("somemail@yandex.ru");
            MailMessage m = new MailMessage(from, to);
            m.Attachments.Add(new Attachment("C:\\temp\\logs2.txt"));
            m.Subject = "Logs";
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("somemail@gmail.com", "mypassword");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
        }
    }
}
