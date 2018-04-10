using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp4
{
    class Blockchain
    {
        List<Block> blocks = new List<Block>();
        public int Difficulty { get; set; }
        private Algorithm algo { get; set; }

        public double AverageMineTime { get {
                return (from b in blocks select b.TimeToMine).Sum() / blocks.Count;
            } }

        //if no difficulty is specified, default is 4
        public Blockchain()
        {
            Init(null, 4, Algorithm.sha256);
        }
        public Blockchain(int difficulty)
        {
            Init(null, difficulty, Algorithm.sha256);            
        }
        public Blockchain(int difficulty, Algorithm algo)
        {
            Init(null, difficulty, algo);
        }        

        //ability to seed the block chain with data
        public Blockchain(IEnumerable<string> data)
        {
            Init(data, 4, Algorithm.sha256);
        }
        public Blockchain(IEnumerable<string> data, Algorithm algo)
        {
            Init(data, 4, algo);
        }
        public Blockchain(IEnumerable<string> data, int difficulty, Algorithm algo)
        {
            Init(data, difficulty, algo);
        }

        public void Init(IEnumerable<string> data, int difficulty, Algorithm algo)
        {
            this.algo = algo;
            this.Difficulty = difficulty;
            AddGenesisBlock();
            if (data != null)
            {
                foreach (var d in data)
                {
                    var block = this.NewBlock(d);
                    Console.WriteLine("Mining.....");
                    MineBlock(ref block);
                    Console.WriteLine($"Block Mined!\n{block.ToString()}");
                }
            }
        }

        //loop through and try all ints as Nonce value until appropriate hash is found
        public void MineBlock(ref Block block)
        {
            block.Mine();
            this.AddBlockToChain(ref block);
        }        

        //add first block upon blockchain object init
        private void AddGenesisBlock()
        {            
            var genesis = new Block(0, "GENESIS BLOCK", ("").PadLeft(64, '0'), Difficulty, this.algo);
            Console.WriteLine("Mining Genesis Block....");
            genesis.Mine();
            blocks.Add(genesis);
        }

        //create a new block where 
        public Block NewBlock(string data)
        {
            var block = new Block(blocks.Last().Id + 1, data, blocks.Last().Hash, Difficulty, this.algo);
            return block;
        }

        //print out all blocks
        public string PrintBlockChain()
        {
            StringBuilder Sb = new StringBuilder();
            foreach (var b in blocks)
            {
                Sb.Append(b.ToString());
            }
            return Sb.ToString();
        }

        //add to blockchain
        public void AddBlockToChain(ref Block block)
        {
            VerifyChain(ref blocks);
            if (block.PreviousHash == blocks.Last().Hash && block.Hash.Substring(0, Difficulty) == ("").PadLeft(Difficulty, '0') && block.ValidateNonce())
            {
                blocks.Add(block);
            }
        }

        

        //private int VerifyChain(ref List<Block> blocks)
        //{
        //    foreach(var block in blocks)
        //    {

        //    }
        //    return 0;
        //}

        //verify chain as called by our other method, with dependancy injection
        private bool VerifyChain(ref List<Block> blocks)
        {
            for (var i = 0; i < blocks.Count(); i++)
            {
                if (i - 1 >= 0 && (blocks.ElementAt(i).PreviousHash != blocks.ElementAt(i - 1).Hash || blocks[i].Id != i))
                {
                    return false;
                }
            }
            return true;
        }

        //verify chain public facing method
        public bool VerifyChain()
        {
            return VerifyChain(ref blocks);
        }


    }
}
