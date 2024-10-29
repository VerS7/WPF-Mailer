using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MimeKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;


namespace Mailer
{
    /// <summary>
    /// Email-сообщение
    /// </summary>
    public class Email
    {
        public string From { get; protected set; }
        public string To { get; protected set; }
        public string Subject { get; protected set; }
        public DateTimeOffset Date { get; protected set; }
        public string TextBody { get; protected set; }
        public string HtmlBody { get; protected set; }

        public Email(string from, string to, string subject, DateTimeOffset date, string textBody, string htmlBody)
        {
            From = from;
            To = to;
            Subject = subject;
            Date = date;
            TextBody = textBody;
            HtmlBody = htmlBody;
        }
    }

    /// <summary>
    /// Email-контроллер для отправки и получения сообщений
    /// </summary>
    public class MailController
    {
        public event EventHandler OnSuccess;
        public event EventHandler OnError;

        public event EventHandler OnMessageSend;
        public event EventHandler OnMessageSendError;

        public string Email;
        public string Password;
        public string Domain;
        public int Imapport;
        public int Smtpport;

        private int _connectCounter;
        protected string _IMAPdomain;
        protected string _SMTPdomain;

        protected SmtpClient _smtpClient = new SmtpClient();
        protected ImapClient _imapClient = new ImapClient();

        public MailController(string email, string password, string domain, int IMAPport, int SMTPport)
        {
            Email = email;
            Password = password;
            Domain = domain;
            Imapport = IMAPport;
            Smtpport = SMTPport;
            _IMAPdomain = "imap." + Domain;
            _SMTPdomain = "smtp." + Domain;

            _imapClient.Timeout = 10000;
            _smtpClient.Timeout = 10000;

            _imapClient.Connected += async (sender, e) => { await IMAPAuthenticate(); };
            _smtpClient.Connected += async (sender, e) => { await SMTPAuthenticate(); };
        }

        /// <summary>
        /// Подключается к SMTP и IMAP серверам
        /// </summary>
        public async Task Connect()
        {
            try
            {
                await _imapClient.ConnectAsync(_IMAPdomain, Imapport, true);
                await _smtpClient.ConnectAsync(_SMTPdomain, Smtpport, true);
            }
            catch { OnError.Invoke(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Отправляет сообщение по SMTP протоколу
        /// </summary>
        public async Task SendMessage(string to, string subject, string message)
        {
            var msg = new MimeMessage();

            msg.From.Add(new MailboxAddress(Email, Email));
            msg.To.Add(new MailboxAddress(to, to));
            msg.Subject = subject;

            msg.Body = new TextPart("Plain")
            {
                Text = message
            };

            try
            {
                await _smtpClient.SendAsync(msg);
                OnMessageSend?.Invoke(this, EventArgs.Empty);
            } catch { OnMessageSendError?.Invoke(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Получает все сообещния из вкладки "Входящие"
        /// </summary>
        public async Task<(int, List<Email>)> GetInboxMessages(int messageCount)
        {
            List<Email> messages = new List<Email>();
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);
            int count = inbox.Count;

            for (int i = 0; i < messageCount; i++)
            {
                if ((inbox.Count - i - 1) < 0) break;

                var message = await inbox.GetMessageAsync(inbox.Count - i - 1);
                messages.Add(
                    new Email(
                        message.From.Last().ToString(),
                        message.To.Last().ToString(),
                        message.Subject.ToString(),
                        message.Date,
                        !(string.IsNullOrEmpty(message.TextBody)) ? message.TextBody : "",
                        !(string.IsNullOrEmpty(message.HtmlBody)) ? message.HtmlBody : ""
                        )
                    );
            }

            return (inbox.Count, messages);
        }

        /// <summary>
        /// Получает все сообещния из вкладки "Отправленные"
        /// </summary>
        public async Task<(int, List<Email>)> GetSentMessages(int messageCount)
        {
            List<Email> messages = new List<Email>();
            var sent = _imapClient.GetFolder(SpecialFolder.Sent);
            await sent.OpenAsync(FolderAccess.ReadOnly);
            int count = sent.Count;

            for (int i = 0; i < messageCount; i++)
            {
                if ((sent.Count - i - 1) < 0) break;

                var message = await sent.GetMessageAsync(sent.Count - i - 1);
                messages.Add(
                    new Email(
                        message.To.Last().ToString(),
                        message.From.Last().ToString(),
                        message.Subject.ToString(),
                        message.Date,
                        !(string.IsNullOrEmpty(message.TextBody)) ? message.TextBody : "",
                        !(string.IsNullOrEmpty(message.HtmlBody)) ? message.HtmlBody : ""
                        )
                    );
            }

            return (sent.Count, messages);
        }

        private async Task IMAPAuthenticate()
        {
            try
            {
                await _imapClient.AuthenticateAsync(Email, Password);
                _connectCounter++;
                if (_connectCounter == 2)
                {
                    OnSuccess?.Invoke(this, EventArgs.Empty);
                }
            }
            catch { OnError.Invoke(this, EventArgs.Empty); }
        }

        private async Task SMTPAuthenticate()
        {
            try
            {
                await _smtpClient.AuthenticateAsync(Email, Password);
                _connectCounter++;
                if (_connectCounter == 2)
                {
                    OnSuccess?.Invoke(this, EventArgs.Empty);
                }
            }
            catch { OnError.Invoke(this, EventArgs.Empty); }
        }
    }
}
