using System;

namespace Tetris
{
    /// <summary>
    /// Класс игровой сетки со своей логикой и методами взаимодействия
    /// </summary>
    public class GameGrid
    {
        #region Поле, свойства и индексатор
        private readonly int[,] grid;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }
        #endregion

        #region Конструктор
        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];              
        }
        #endregion

        #region Логические методы
        public bool IsInside(int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }
        public bool IsEmpty(int r, int c)
        {
            return IsInside(r, c) && grid[r, c] == 0;            
        }
        public bool IsRowFull(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r,c] == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r,c] != 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Методы взаимодействия
        private void ClearRow(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r, c] = 0;
            }
        }
        private void MoveRowDown(int r, int numRows)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r, c] = 0;
            }
        }
        public int ClearFullRows()
        {
            int cleared = 0;

            for (int r = Rows - 1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }
            return cleared;
        }
        #endregion
    }
}
