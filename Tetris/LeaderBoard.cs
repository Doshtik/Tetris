using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;

namespace Tetris
{
    public class LeaderBoard
    {
        #region Поля и свойства
        public static string _path = "LeaderBoard.txt";
        public static List<User> PlayerList { get; private set; } = new List<User>();
        public static string Name { get; set; } = "Player";
        #endregion

        #region Методы перезаписи и записи
        private static void RewriteLeaderBoard()
        {
            string tempPath = _path + ".tmp";
            using (StreamWriter streamWriter = new StreamWriter(tempPath, false)) // и сразу же пишем во временный файл
            {
                for (int i = 0; i < PlayerList.Count; i++)
                    if (i == PlayerList.Count - 1)
                        streamWriter.Write(PlayerList[i].Field);
                    else
                        streamWriter.WriteLine(PlayerList[i].Field);
            }
            File.Delete(_path); // удаляем старый файл
            File.Move(tempPath, _path); // переименовываем временный файл
        }
        public static void RewriteLineInList(int lineIndex, User player)
        {
            int i = 0;
            string tempPath = _path + ".tmp";
            using (StreamReader streamReader = new StreamReader(_path)) // читаем
            using (StreamWriter streamWriter = new StreamWriter(tempPath, false)) // и сразу же пишем во временный файл
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (streamReader.EndOfStream)
                        if (lineIndex == i)
                            streamWriter.Write($"{player.Field}");
                        else
                            streamWriter.Write(line);
                    else
                        if (lineIndex == i)
                            streamWriter.WriteLine($"{player.Field}");
                        else
                            streamWriter.WriteLine(line);
                    i++;
                }
            }
            File.Delete(_path); // удаляем старый файл
            File.Move(tempPath, _path); // переименовываем временный файл
        }
        public static void AddLineInList(User player)
        {
            using (StreamWriter writer = new StreamWriter(_path, true))
            {
                writer.Write($"\n{player.Field}");
            }
        }
        #endregion

        // Метод переноса результатов с .txt в List<>
        public static void UpdateLeaderBoardList()
        {
            List<User> players = new List<User>();
            using (StreamReader streamReader = new StreamReader(_path))
            {
                int itterator = 0;
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.Length > 0) 
                    {
                        string[] player = line.Split(" - ");
                        players.Add(new User(player[0], Int32.Parse(player[1])));
                        itterator++;
                    }
                }
                if (itterator == 0)
                {
                    throw new Exception("Пользователей нет в списке");
                }
                
            }
            PlayerList.Clear();
            IEnumerable<User> query = players.OrderBy(player => player.Score);
            foreach (User player in query)
            {
                PlayerList.Add(player);
            }
            PlayerList.Reverse();
            RewriteLeaderBoard();
        }

        // Метод, показывающий позицию игрока в списке
        public static void GetCurrentUser(out User playerInList, out int index)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].Name == Name)
                {
                    playerInList = PlayerList[i];
                    index = i;
                    return;
                }
            }
            throw new Exception("Игрока нет в списке");
        }
    }
}
