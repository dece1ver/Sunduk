using System.Net;
using System.Net.Mail;

namespace Sunduk.WebApi
{
    public class SendMessageService
    {
        public static string Send(string Name, string Message, string From, string To, string Pass)
        {
            var result = "Ne";
            try
            {
                using MailMessage mail = new();
                mail.From = new MailAddress(From, "sunduk.one");
                mail.To.Add(To);
                mail.Subject = $"Обратная связь от: {Decode(WebUtility.UrlDecode(Name))}";
                mail.Body = Decode(WebUtility.UrlDecode(Message));
                mail.IsBodyHtml = false;

                using SmtpClient smtp = new("smtp.yandex.ru", 25);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(From, Pass);
                smtp.EnableSsl = true;
                smtp.Send(mail);
                result = "Ok";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        private static string Decode(string message)
        {
            return message
                .Replace(@"_____1_", @"@")
                .Replace(@"____1_", @".")
                .Replace(@"___1_", @"\")
                .Replace(@"__1_", @"/")
                .Replace(@"_1_", @" ");
        }

    }
}
