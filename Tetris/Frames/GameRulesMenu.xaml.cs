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
        }

        private void BttnNext_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.IsGameStarted = true;
            NavigationService.Navigate(null);
        }
    }
}
