using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class User
    {
        public string Field 
        {
            get => $"{Name} - {Score}";
        }
        public string Name { get; set; }
        public int Score { get; set; }

        public User(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
