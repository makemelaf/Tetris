using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;
using System.Windows.Controls;

namespace Tetris
{
    public class BlockQueue
    {
        
    private readonly Random random = new Random();
        private List<Block> blocks;
        private int currentIndex;

        public Block NextBlock { get; private set; }

        public BlockQueue()
        {
            blocks = new List<Block>
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

            ShuffleBlocks();
            SetNextBlock();
        }

        public void ShuffleBlocks()
        {
            blocks = blocks.OrderBy(x => random.Next()).ToList();
            currentIndex = 0;
        }

        private void SetNextBlock()
        {
            NextBlock = blocks[currentIndex];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;
            currentIndex++;

            if (currentIndex >= blocks.Count)
            {
                ShuffleBlocks();
            }

            SetNextBlock();
            return block;
        }
    }
}
