using System;
using System.Windows;


namespace Mailer
{
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var loginControl = new Login();
            loginControl.OnLoginSuccess += OnLoginSuccess;
            MainGrid.Children.Add(loginControl);
        }

        /// <summary>
        /// Ивент на успешную аутентификацию. Переключает на окно приложения с основной логикой
        /// </summary>
        public void OnLoginSuccess(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new MailTab((MailController)sender));
            });
        }
    }
}
