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
        public static int DifficultyModificator { get; private set; }

        private Dictionary<string, double> _difficulty;
        #endregion

        #region Конструктор
        public SettingsMenu()
        {
            InitializeComponent();
            InitDictOfDifficulty();
            switch (MainWindow.DictLanguage)
            {
                case "rus":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryRus.xaml") };
                    break;
                case "eng":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryEng.xaml") };
                    break;
            }

            difficultyComboBox.ItemsSource = _difficulty.Keys;
            DifficultyModificator = 1;

            masterSlider.Value = MasterVolume;
            soundSlider.Value = SoundVolume;
            musicSlider.Value = MusicVolume;
        }
        #endregion

        #region Метод инициализации
        private void InitDictOfDifficulty()
        {
            _difficulty = new Dictionary<string, double>();
            _difficulty.Add("Easy", 1);
            _difficulty.Add("Hard", 2);
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
                    Debug.Print($"Выбран легкий уровень: {_difficulty["Easy"]}");
                    break;
                case "Medium":
                    Debug.Print($"Выбран средний уровень: {_difficulty["Medium"]}");
                    break;
                case "Hard":
                    Debug.Print($"Выбран сложный уровень: {_difficulty["Hard"]}");
                    break;
                case "VeryHard":
                    Debug.Print($"Выбран очень сложный уровень: {_difficulty["VeryHard"]}");
                    break;
            }
        }

        private void bttnExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
