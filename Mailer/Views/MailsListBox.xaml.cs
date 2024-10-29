using System.Collections.Generic;
using System.Windows.Controls;


namespace Mailer
{
    /// <summary>
    /// Список сообщений
    /// </summary>
    public partial class MailsListBox : UserControl
    {
        public MailsListBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обновляет список сообщений по переданному списку сообщений
        /// </summary>
        public void UpdateMailsListBox(List<Email> emails)
        {
            MailsScrollList.Items.Clear();
            foreach (Email email in emails) 
            {
                MailsScrollList.Items.Add(new MailListItem(email));
            }
        }
    }
}
