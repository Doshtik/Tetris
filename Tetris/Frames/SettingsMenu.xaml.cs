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
using System.Windows.Shell;

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
            get
            {
                using (var sr = new StreamReader("config.txt"))
                {
                    int difficulty = 0;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != String.Empty)
                        {
                            if (line.Split(" = ")[0] == "difficulty")
                            {
                                difficulty = int.Parse(line.Split(" = ")[1]);
                            }
                        }
                    }

                    if (difficulty == 0)
                        return 1;
                    else
                        return difficulty;
                }
            }
            private set
            {
                string filename = "config.txt";
                using (var sr = new StreamReader(filename))
                using (var sw = new StreamWriter(filename + ".tmp", false))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != null)
                        {
                            if (sr.EndOfStream)
                            {
                                if (line.Split(" = ")[0] == "difficulty")
                                {
                                    sw.Write("difficulty = " + value);
                                }
                                else
                                {
                                    sw.Write(line);
                                }
                            }
                            else
                            {
                                if (line.Split(" = ")[0] == "difficulty")
                                {
                                    sw.WriteLine("difficulty = " + value);
                                }
                                else
                                {
                                    sw.WriteLine(line);
                                }
                            }
                        }
                    }
                }
                File.Delete(filename);
                File.Move(filename + ".tmp", filename);
            }
        }

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
            difficultyComboBox.SelectedIndex = DifficultyModificator - 1;

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

        private void bttnExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void difficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (difficultyComboBox.SelectedIndex)
            {
                case 0:
                    DifficultyModificator = 1;
                    break;
                case 1:
                    DifficultyModificator = 2;
                    break;
            }
        }
    }
}
