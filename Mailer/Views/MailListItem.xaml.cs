using System.Windows;
using System.Windows.Controls;


namespace Mailer
{
    /// <summary>
    /// Карточка сообщения в списке сообщений
    /// </summary>
    public partial class MailListItem : UserControl
    {
        public MailListItem(Email email)
        {
            InitializeComponent();

            LoadMessage(email);
        }

        /// <summary>
        /// Загружает сообщение в карточку
        /// </summary>
        public void LoadMessage(Email email)
        {
            From.Text = email.From;
            Subject.Text = email.Subject;
            Date.Text = email.Date.ToString("dd.MM.yyyy HH:mm:ss");

            if (!string.IsNullOrEmpty(email.TextBody))
            {
                TextView.Visibility = Visibility.Visible;
                TextView.Text = email.TextBody;
            }
        }
    }
}
