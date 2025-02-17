using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using Tetris.Frames;

namespace Tetris
{
    public class LeaderBoard
    {
        #region Поля и свойства
        public static string _path = "LeaderBoard.txt";
        public static List<User> PlayerList { get; private set; } = new List<User>();
        #endregion

        #region Методы перезаписи и записи
        private static void RewriteLeaderBoard()
        {
            string tempPath = _path + ".tmp";
            using (var streamWriter = new StreamWriter(tempPath, false)) // и сразу же пишем во временный файл
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
            using (var sr = new StreamReader(_path)) // читаем
            using (var sw = new StreamWriter(tempPath, false)) // и сразу же пишем во временный файл
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (sr.EndOfStream)
                        if (lineIndex == i)
                            sw.Write($"{player.Field}");
                        else
                            sw.Write(line);
                    else
                        if (lineIndex == i)
                            sw.WriteLine($"{player.Field}");
                        else
                            sw.WriteLine(line);
                    i++;
                }
            }
            File.Delete(_path); // удаляем старый файл
            File.Move(tempPath, _path); // переименовываем временный файл
        }
        public static void AddLineInList(User player)
        {
            using (var writer = new StreamWriter(_path, true))
            {
                writer.Write($"\n{player.Field}");
            }
        }
        #endregion

        // Метод переноса результатов с .txt в List<>
        public static void UpdateLeaderBoardList()
        {
            List<User> players = new List<User>();
            using (var streamReader = new StreamReader(_path))
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
                if (PlayerList[i].Name == SettingsMenu.Name)
                {
                    playerInList = PlayerList[i];
                    index = i;
                    return;
                }
            }
            throw new Exception("Игрока нет в списке");
        }

        // Метод фиксирующий результат игрока
        public static void UpdateTable(string name, int score)
        {
            //Высчитывание результатов и внесение в таблицу
            User player = new User(name, score);
            try
            {
                //Список рекордсменов (Если нет - сработает обработка исключений)
                LeaderBoard.UpdateLeaderBoardList();
                //Позиция игрока в списке (Если игрока в списке нет - сработает отработка исключений)
                LeaderBoard.GetCurrentUser(out User playerInList1, out int index1);
                if (player.Score > playerInList1.Score)
                {
                    LeaderBoard.RewriteLineInList(index1, player);
                }
            }
            catch
            {
                LeaderBoard.AddLineInList(player);
            }
            LeaderBoard.UpdateLeaderBoardList();
        }
    }
}
