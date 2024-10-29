using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;


namespace Mailer
{
    /// <summary>
    /// Индикатор загрузки с бегущими точками
    /// </summary>
    public partial class LoadingElement : UserControl
    {
        public string Text = "Загрузка";
        public string Error = "Ошибка";
        private bool _loading;
        public LoadingElement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Остановка
        /// </summary>
        public void Stop() 
        { 
            _loading = false;
        }

        /// <summary>
        /// Ивент запуска
        /// </summary>
        public void onStart(object sender, EventArgs e)
        {
            _loading = true;
            LoadingText.Foreground = Brushes.Black;
            Dispatcher.Invoke(() => { Start(); });
        }

        /// <summary>
        /// Ивент на ошибку
        /// </summary>
        public void onError(object sender, EventArgs e)
        {
            _loading = false;
            LoadingText.Foreground = Brushes.Red;
            LoadingText.Text = Error;
        }

        /// <summary>
        /// Запуск
        /// </summary>
        private async void Start()
        {
            while (_loading)
            {
                LoadingText.Text = Text;

                for (int i = 0; i < 3; i++)
                {
                    LoadingText.Text += ".";
                    await Task.Delay(500);
                }
            }
        }
    }
}
