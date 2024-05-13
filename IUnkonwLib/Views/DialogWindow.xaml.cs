using IUnkonwLib.Models;
using System.Windows;

namespace IUnkonwLib.Views
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public Status StatusValue { get; set; }
        public DialogWindow()
        {
            InitializeComponent();
        }

        public Status ShowPauseWindow(string text)
        {
            MessageTbk.Text = text;
            this.ShowDialog();
            return StatusValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StatusValue = Status.Сontinue;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StatusValue = Status.Close;
            Close();
        }
    }
}
