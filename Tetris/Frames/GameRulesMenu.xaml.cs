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
    /// Логика взаимодействия для HowToPlayMenu.xaml
    /// </summary>
    public partial class GameRulesMenu : Page
    {
        public GameRulesMenu()
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

        private void BttnNext_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.IsGameStarted = true;
            NavigationService.Navigate(null);
        }
    }
}
