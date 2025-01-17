using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Меню для взаимодействия с классом Settings
    /// </summary>
    public partial class SettingsMenu : Page
    {
        #region Поля и свойства
        public static double MasterVolume { get; private set; } = 5;
        public static double SoundVolume { get; private set; } = 10;
        public static double MusicVolume { get; private set; } = 10;

        public static string Difficulty { get; private set; }
        public static int DifficultyModificator { get; private set; }

        private static Dictionary<string, int> _difficultyDict = new Dictionary<string, int>();
        #endregion

        #region Конструктор
        public SettingsMenu()
        {
            InitializeComponent();
            switch (MainWindow.DictLanguage)
            {
                case "rus":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryRus.xaml") };
                    _difficultyDict.Add("Легко", 1);
                    _difficultyDict.Add("Сложно", 2);
                    break;
                case "eng":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryEng.xaml") };
                    _difficultyDict.Add("Easy", 1);
                    _difficultyDict.Add("Hard", 2);
                    break;
            }

            difficultyComboBox.ItemsSource = _difficultyDict.Keys;
            DifficultyModificator = 1;

            masterSlider.Value = MasterVolume;
            soundSlider.Value = SoundVolume;
            musicSlider.Value = MusicVolume;
        }
        #endregion

        #region Методы работы со звуком
        private void masterSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MasterVolume = Math.Round(e.NewValue / 10, 2);
        }
        private void soundSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundVolume = Math.Round(e.NewValue / 10, 2);
        }
        private void musicSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicVolume = Math.Round(e.NewValue / 10, 2);
        }
        #endregion

        private void DifficultyComboBox_Selected(object sender, RoutedEventArgs e)
        {
            switch (difficultyComboBox.SelectedItem)
            {
                case "Easy":
                    Difficulty = "Easy";
                    DifficultyModificator = _difficultyDict["Easy"];
                    break;
                case "Легко":
                    Difficulty = "Easy";
                    DifficultyModificator = _difficultyDict["Easy"];
                    break;
                case "Hard":
                    Difficulty = "Hard";
                    DifficultyModificator = _difficultyDict["Hard"];
                    break;
                case "Сложно":
                    Difficulty = "Hard";
                    DifficultyModificator = _difficultyDict["Hard"];
                    break;
            }
        }

        private void bttnExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
