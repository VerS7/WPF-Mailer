using System.Collections.Generic;


namespace Mailer
{
    /// <summary>
    /// Email-сервис по типу gmail и т.п.
    /// </summary>
    public class EmailService
    {
        public string Domain;
        public int SMTPport;
        public int IMAPport;

        public EmailService(string domain, int smtpport, int imapport) 
        {
            Domain = domain;
            SMTPport = smtpport;
            IMAPport = imapport;
        }
    }

    static class EmailServices
    {
        /// <summary>
        /// Словарь email-сервисов
        /// </summary>
        public static readonly Dictionary<string, EmailService> services = new Dictionary<string, EmailService> 
        {
            { "yandex.ru", new EmailService("yandex.ru", 465, 993 ) },
            { "mail.ru", new EmailService("mail.ru", 465, 993) },
            { "gmail.com", new EmailService("gmail.com", 465, 993) }
        };
    }
}
