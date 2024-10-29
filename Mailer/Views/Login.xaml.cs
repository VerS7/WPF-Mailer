using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace Mailer
{
    /// <summary>
    /// Стартовое окно входа и аутентификации
    /// </summary>
    public partial class Login : UserControl
    {
        public event EventHandler OnLoginSuccess;
        private MailController _mailController;
        private ApplicationConfig _config;

        private bool _isConnecting;
        private string _email;
        private string _password;

        public Login()
        {
            InitializeComponent();
            _config = ApplicationData.GetConfig();

            EmailTextBox.Text = _config.Email;
            PasswordTextBox.Text = _config.Password;

            LoadingStatus.Error = "Ошибка подключения";
        }

        /// <summary>
        /// Ивент кнопки входа
        /// </summary>
        private async void ConfirmButton_Click(object sender, EventArgs e)
        {
            _email = EmailTextBox.Text;
            _password = PasswordTextBox.Text;

            if (_email == null || !(IsValidEmail(_email)))
            {
                EmailError.Visibility = Visibility.Visible;
                EmailError.Text = "Некорректный email.";
                return;
            }

            if (_password.Length <= 0 || _email.Length <= 0 || _isConnecting)
            {
                return;
            }

            _isConnecting = true;
            LoadingStatus.onStart(this, EventArgs.Empty);

            EmailService service = EmailServices.services[_email.Split('@')[1]];

            _mailController = new MailController(
                    _email,
                    _password,
                    service.Domain,
                    service.IMAPport,
                    service.SMTPport
                    );

            _mailController.OnSuccess += OnSuccessAuth;
            _mailController.OnError += OnErrorAuth;

            await _mailController.Connect();
        }

        /// <summary>
        /// Ивент на успешную аутентификацию
        /// </summary>
        private void OnSuccessAuth(object sender, EventArgs e)
        {
            _config.Email = _email;
            _config.Password = _password;
            ApplicationData.Save();
            OnLoginSuccess?.Invoke(_mailController, EventArgs.Empty);
        }

        /// <summary>
        /// Ивент на проваленную аутентификацию
        /// </summary>
        private void OnErrorAuth(object sender, EventArgs e) 
        {
            Dispatcher.Invoke(() => { LoadingStatus.onError(this, EventArgs.Empty); });
            _isConnecting = false;
        }

        /// <summary>
        /// Валидация Email'ов. Подходят только gmail, mail.ru и yandex почты
        /// </summary>
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@(?:gmail\.com|mail\.ru|yandex\.ru)$");
        }
    }
}
