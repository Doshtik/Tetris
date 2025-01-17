using System;
using System.Windows;
using System.Windows.Controls;
using Tetris.Blocks;
using Tetris.Frames;

namespace Tetris
{
    /// <summary>
    /// Класс, где объединяются фигуры и сетка
    /// </summary>
    public class GameState
    {
        #region Поля и свойства сетки и блоков
        public GameGrid GameGrid { get; }
        private Block currentBlock;
        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }
        public BlockQueue BlockQueue { get; }
        #endregion

        #region Свойства удержания фигуры
        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }
        
        public bool GameOver { get; set; }
        
        public int Score { get; private set; }
        
        public int ClearedRows { get; private set; }
        #endregion

        #region Конструктор
        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();

            CanHold = true;
            GameOver = false;
        }
        #endregion

        #region Логические методы
        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }
        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                    return false;
            }
            return true;
        }
        #endregion

        #region Методы взаимодействия с фигурой
        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            int cleared = GameGrid.ClearFullRows();
            ClearedRows += cleared;
            switch (cleared)
            {
                case 1:
                    Score += 100;
                    break;
                case 2:
                    Score += 300;
                    break;
                case 3:
                    Score += 700;
                    break;
                case 4:
                    Score += 1500;
                    break;
            }

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }
        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }

            if (HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }
        private int TileDropDistance(Position position)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(position.Row + drop + 1, position.Column))
            {
                drop++;
            }
            return drop;
        }
        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position position in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(position));
            }
            return drop;
        }
        #endregion

        #region Методы поворота и движения фигур
        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }
        public void RotateBlcokCCW()
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }
        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
        #endregion
    }
}
