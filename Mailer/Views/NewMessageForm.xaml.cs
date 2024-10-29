using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;


namespace Mailer
{
    /// <summary>
    /// Форма отправки сообщения
    /// </summary>
    public partial class NewMessageForm : UserControl
    {
        private MailController _mailController;
        public NewMessageForm(MailController mailController)
        {
            InitializeComponent();

            _mailController = mailController;
            _mailController.OnMessageSend += OnMessageSend;
            _mailController.OnMessageSendError += OnMessageSendError;
        }

        /// <summary>
        /// Ивент кнопки отправки сообщения
        /// </summary>
        private async void SendMessageButton_Click(object sender, EventArgs e)
        {
            await SendMessage(sender, e);
        }

        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        private async Task SendMessage(object sender, EventArgs e)
        {
            string to = To.Text;
            string subject = Subject.Text;
            string body = Body.Text;

            Status.Foreground = Brushes.Black;

            if (string.IsNullOrEmpty(to) || !IsValidEmail(to))
            {
                Status.Foreground = Brushes.Red;
                Status.Text = "Некорректный Email получателя";
                return;
            }

            if (string.IsNullOrEmpty (subject))
            {
                Status.Foreground = Brushes.Red;
                Status.Text = "Тема сообщения пустая";
                return;
            }

            if (string.IsNullOrEmpty(body))
            {
                Status.Foreground = Brushes.Red;
                Status.Text = "Сообщение пустое";
                return;
            }

            await _mailController.SendMessage(to, subject, body);
        }

        /// <summary>
        /// Ивент на успешную отправку сообщения
        /// </summary>
        private void OnMessageSend(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                To.Clear();
                Subject.Clear();
                Body.Clear();

                Status.Foreground = Brushes.Green;
                Status.Text = "Сообщение успешно отправлено!";
            });
        }

        /// <summary>
        /// Ивент на проваленную отправку сообщения
        /// </summary>
        private void OnMessageSendError(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Status.Foreground = Brushes.Red;
                Status.Text = "Не удалось отправить сообщение";
            });
        }

        /// <summary>
        /// Валидация Email'ов
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])");
        }
    }
}
