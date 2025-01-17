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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris.Frames
{
    /// <summary>
    /// Логика взаимодействия для LeaderBoardMenu.xaml
    /// </summary>
    public partial class LeaderBoardMenu : Page
    {
        public LeaderBoardMenu()
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
        }

        private void bttnLeaderBoard_Close_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(null);
        }
    }
}
