using System.Windows;
using System.Windows.Controls;
using Tetris.Frames;

namespace Tetris
{
    /// <summary>
    /// Логика взаимодействия для WindowRegistration.xaml
    /// </summary>
    public partial class WindowRegistration : Window
    {
        private string _name;
        private int _score;

        public WindowRegistration(int score)
        {
            InitializeComponent();
            switch (MainWindow.DictLanguage)
            {
                case "rus":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryRus.xaml") };
                    break;
                case "eng":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryEng.xaml") };
                    break;
            }
            _score = score;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _name = NameChange_TextBox.Text;
        }

        private void bttn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.Name = _name;
            LeaderBoard.UpdateTable(_name, _score);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LeaderBoard.UpdateTable(SettingsMenu.Name, _score);
        }
    }
}
