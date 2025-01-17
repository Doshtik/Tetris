using System;

namespace Tetris
{
    public abstract class Block
    {
        //Поля
        private int rotationState;
        private Position offset;

        //Свойства
        public abstract int Id { get; }
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffset { get; }

        //Конструктор
        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        public void Move(int rows, int columns)
        {
            offset.Row += rows;
            offset.Column += columns;
        }
        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }

        //Метод положения клеток фигуры на сетке
        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[rotationState])
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        public void RotateCW()
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }
        public void RotateCCW()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }
    }
}
