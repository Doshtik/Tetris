using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        protected class Difficulty
        {
            public int Difficult { get; set; }
            public Difficulty(int value) => Difficult = value;
        }
        public static int DifficultyModificator
        {
            get;
            /*{
                using (FileStream fr = new FileStream("config.json", FileMode.OpenOrCreate))
                {
                    Difficulty? difficulty = JsonSerializer.Deserialize<Difficulty>(fr);
                    return difficulty?.Difficult ?? 1;
                }
            }*/
            private set;
            /*{
                using (FileStream fs = new FileStream("config.json", FileMode.OpenOrCreate))
                {
                    Difficulty difficulty = new Difficulty(value);
                    JsonSerializer.SerializeAsync<Difficulty>(fs, difficulty);
                }
            }*/
        } = 1;

        private static List<string> _difficultyDict;
        #endregion

        #region Конструктор
        public SettingsMenu()
        {
            InitializeComponent();
            _difficultyDict = new List<string>();
            switch (MainWindow.DictLanguage)
            {
                case "rus":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryRus.xaml") };
                    _difficultyDict.Add("Легко");
                    _difficultyDict.Add("Сложно");
                    break;
                case "eng":
                    this.Resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryEng.xaml") };
                    _difficultyDict.Add("Easy");
                    _difficultyDict.Add("Hard");
                    break;
            }

            difficultyComboBox.ItemsSource = _difficultyDict.GetRange(0, _difficultyDict.Count);

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
                    DifficultyModificator = 1;
                    break;
                case "Легко":
                    DifficultyModificator = 1;
                    break;
                case "Hard":
                    DifficultyModificator = 2;
                    break;
                case "Сложно":
                    DifficultyModificator = 2;
                    break;
            }
        }

        private void bttnExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
