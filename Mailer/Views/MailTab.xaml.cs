using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;


namespace Mailer
{
    /// <summary>
    /// Главное окно приложения
    /// </summary>
    public partial class MailTab : UserControl
    {
        private MailController _mailController;
        private MailsListBox _inboxListBox;
        private MailsListBox _sentListBox;
        private NewMessageForm _newMessageForm;

        private bool _isLoading = false;

        public MailTab(MailController mailController)
        {
            InitializeComponent();
            _mailController = mailController;
            _inboxListBox = new MailsListBox();
            _sentListBox = new MailsListBox();
            _newMessageForm = new NewMessageForm(_mailController);

            ImapStatus.Text = $"imap.{_mailController.Domain}:{_mailController.Imapport}";
            SmtpStatus.Text = $"smtp.{_mailController.Domain}:{_mailController.Smtpport}";
            UserEmail.Text = _mailController.Email;

            Loaded += OnMailTabLoaded;
        }

        /// <summary>
        /// Ивент на завершение загрузки MailTab
        /// </summary>
        private void OnMailTabLoaded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(async () => { await LoadInbox(); });
        }

        /// <summary>
        /// Ивент кнопки WriteMailButton, вызывающей окно отправки сообщения
        /// </summary>
        private void WriteMailButton_Click(object sender, EventArgs e)
        {
            if (MailListBoxContainer.Child != _newMessageForm)
            {
                _newMessageForm.Status.Text = "";
                LoadMessageForm();
            }
        }

        /// <summary>
        /// Ивент кнопки SentButton, вызывающей окно отправленных сообщений
        /// </summary>
        private async void SentButton_Click(object sender, EventArgs e)
        {
            if (MailListBoxContainer.Child != _sentListBox && !_isLoading) 
            {
                await LoadSent();
            }
        }

        /// <summary>
        /// Ивент кнопки InboxButton, вызывающей окно полученных сообщений
        /// </summary>
        private async void InboxButton_Click(object sender, EventArgs e)
        {
            if (MailListBoxContainer.Child != _inboxListBox && !_isLoading)
            {
                await LoadInbox();
            }
        }

        /// <summary>
        /// Загружает полученные сообщения с email и отображает их в MailsListBox
        /// </summary>
        private async Task LoadInbox()
        {
            _isLoading = true;
            var LoadingStatus = new LoadingElement();
            LoadingStatus.LoadingText.FontSize = 24;
            LoadingStatus.Margin = new Thickness(0, 25, 0, 0);
            LoadingStatus.onStart(this, EventArgs.Empty);
            MailListBoxContainer.Child = LoadingStatus;

            try
            {
                var emails = await _mailController.GetInboxMessages(ApplicationData.GetConfig().MaxMessages);

                Dispatcher.Invoke(() =>
                {
                    _inboxListBox.UpdateMailsListBox(emails.Item2);
                    MailListBoxContainer.Child = _inboxListBox;
                    _isLoading = false;
                });
            } catch { LoadingStatus.onError(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Загружает отправленные сообщения с email и отображает их в MailsListBox
        /// </summary>
        private async Task LoadSent()
        {
            _isLoading = true;
            var LoadingStatus = new LoadingElement();
            LoadingStatus.LoadingText.FontSize = 24;
            LoadingStatus.Margin = new Thickness(0, 25, 0, 0);
            LoadingStatus.onStart(this, EventArgs.Empty);
            MailListBoxContainer.Child = LoadingStatus;

            try
            {
                var emails = await _mailController.GetSentMessages(ApplicationData.GetConfig().MaxMessages);

                Dispatcher.Invoke(() =>
                {
                    _sentListBox.UpdateMailsListBox(emails.Item2);
                    MailListBoxContainer.Child = _sentListBox;
                    _isLoading = false;
                });
            }
            catch { LoadingStatus.onError(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Загружает форму отправки сообщения
        /// </summary>
        private void LoadMessageForm()
        {
            Dispatcher.Invoke(() =>
            {
                MailListBoxContainer.Child = _newMessageForm;
            });
        }
    }
}
