using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Settings
    {
        private static float _musicVolume;
        private static float _soundVolume;

        public static float DifficultModificator {  get; set; }
        public static float GeneralVolume { get; set; }
        public static float MusicVolume 
        { 
            get => _musicVolume * GeneralVolume;
            set => _musicVolume = value;
        }
        public static float SoundVolume 
        {
            get => _soundVolume * GeneralVolume; 
            set => _soundVolume = value;
        }
    }
}
