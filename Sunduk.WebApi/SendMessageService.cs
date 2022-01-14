using System.Net.Mail;

namespace Sunduk.WebApi
{
    public class SendMessageService
    {
        public static bool Send(string Name, string Message, string From, string To, string Pass)
        {
            var result = false;
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(From);
                mail.To.Add(To);
                mail.Subject = $"Обратная связь от: {Name}";
                mail.Body = Message;
                mail.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient("smtp.office365.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(From, Pass);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    result = true;
                }
            }
            return result;
        }
    }
}
