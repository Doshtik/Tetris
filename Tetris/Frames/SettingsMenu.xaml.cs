using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static double MasterVolume
        {
            get
            {
                return GetValueFromConfigFile<Double>("master_volume");
            }
            private set
            {
                SetValueToConfigFile<double>("master_volume", value);
            }
        }
        public static double SoundVolume
        {
            get
            {
                return GetValueFromConfigFile<Double>("sound_volume");
            }
            private set
            {
                SetValueToConfigFile<double>("sound_volume", value);
            }
        }
        public static double MusicVolume 
        {
            get
            {
                return GetValueFromConfigFile<Double>("music_volume");
            }
            private set
            {
                SetValueToConfigFile<double>("music_volume", value);
            }
        }
        public static int DifficultyModificator
        {
            get
            {
                return GetValueFromConfigFile<Int32>("difficulty");
            }
            private set
            {
                SetValueToConfigFile<int>("difficulty", value);
            }
        }

        private static List<string> _difficultyDict;
        #endregion

        #region Конструктор
        public SettingsMenu()
        {
            if (!File.Exists("config.txt")) 
                CreateConfigFile("config.txt");

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

            if (DifficultyModificator == 1)
                difficultyComboBox.SelectedIndex = 0;
            else
                difficultyComboBox.SelectedIndex = 1;

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

        #region Методы работы с конфигом
        public static void CreateConfigFile(string filename)
        {
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.WriteLine("difficulty = 1");
                sw.WriteLine("master_volume = 0.5");
                sw.WriteLine("music_volume = 1.0");
                sw.WriteLine("sound_volume = 1.0");
            }
        }
        private static T GetValueFromConfigFile<T>(string field)
        {
            using (var sr = new StreamReader("config.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] parts = line.Split(" = ");
                    if (parts[0].Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                        {
                            return (T)converter.ConvertFromString(parts[1]);
                        }
                    }
                }
            }
            return default(T);
        }
        private static void SetValueToConfigFile<T>(string field, T value)
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
                            if (line.Split(" = ")[0] == field)
                            {
                                sw.Write(field + " = " + value);
                            }
                            else
                            {
                                sw.Write(line);
                            }
                        }
                        else
                        {
                            if (line.Split(" = ")[0] == field)
                            {
                                sw.WriteLine(field + " = " + value);
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
        #endregion

        #region События
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
        #endregion
    }
}
