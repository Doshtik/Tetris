using System;

namespace Tetris.Blocks
{
    public class BlockQueue
    {
        #region Поля и свойства
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new TBlock(),
            new LBlock(),
            new SBlock(),
            new ZBlock(),
            new OBlock()
        };
        private readonly Random random = new Random();
        public Block NextBlock { get; private set; }
        #endregion

        #region Конструктор
        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }
        #endregion

        #region Работа с очередью
        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);

            return block;
        }
        #endregion
    }
}
