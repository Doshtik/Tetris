using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            //Высчитывание результатов и внесение в таблицу
            User player = new User(_name, _score);
            try
            {
                //Список рекордсменов (Если нет - сработает обработка исключений)
                LeaderBoard.UpdateLeaderBoardList();
                //Позиция игрока в списке (Если игрока в списке нет - сработает отработка исключений)
                LeaderBoard.GetCurrentUser(out User playerInList1, out int index1);
                if (player.Score > playerInList1.Score)
                {
                    LeaderBoard.RewriteLineInList(index1, player);
                }
            }
            catch
            {
                LeaderBoard.AddLineInList(player);
            }
            LeaderBoard.UpdateLeaderBoardList();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Высчитывание результатов и внесение в таблицу
            User player = new User(SettingsMenu.Name, _score);
            try
            {
                //Список рекордсменов (Если нет - сработает обработка исключений)
                LeaderBoard.UpdateLeaderBoardList();
                //Позиция игрока в списке (Если игрока в списке нет - сработает отработка исключений)
                LeaderBoard.GetCurrentUser(out User playerInList1, out int index1);
                if (player.Score > playerInList1.Score)
                {
                    LeaderBoard.RewriteLineInList(index1, player);
                }
            }
            catch
            {
                LeaderBoard.AddLineInList(player);
            }
            LeaderBoard.UpdateLeaderBoardList();
        }
    }
}
