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
    /// Логика взаимодействия для StartGameMenu.xaml
    /// </summary>
    public partial class StartGameMenu : Page
    {
        #region Конструктор
        public StartGameMenu()
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
        #endregion

        #region Кнопки
        private void bttnStartGame_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.IsGameStarted = true;
            NavigationService.Navigate(null);
        }
        private void bttnChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.DictLanguage == MainWindow.Languages[MainWindow.Languages.Count - 1])
            {
                MainWindow.DictLanguage = MainWindow.Languages.First();
            }
            else
            {
                MainWindow.DictLanguage = MainWindow.Languages[MainWindow.Languages.LastIndexOf(MainWindow.DictLanguage) + 1];
            }

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
        private void bttnSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsMenu());
        }
        private void bttnHowToPlay_Click(object sender, RoutedEventArgs e)
        {
            //
        }
        private void bttnExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        #endregion
    }
}
