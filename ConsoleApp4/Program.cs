using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            //create a blockchain with a difficulty of 4
            var blockchain = new Blockchain(4);

            //loop and ask user to insert blocks
            LoopAndAskForBlockData(ref blockchain);
        }
        static void LoopAndAskForBlockData(ref Blockchain blockchain) {
            var ret = "";
            do
            {
                Console.Write("Enter Data: ");
                ret = Console.ReadLine();
                if (ret.ToLower() == "print")
                {
                    Console.WriteLine(blockchain.PrintBlockChain());
                    continue;
                }
                if (ret.ToLower() == "validate")
                {
                    Console.WriteLine($"Is chain valid: {blockchain.VerifyChain()}");
                    continue;
                }

                var block = blockchain.NewBlock(ret);
                Console.WriteLine("Mining.....");
                blockchain.MineBlock(block);
                Console.WriteLine($"Block Mined!\n{block.ToString()}");

            } while (ret != "exit");
        }
     
    }
    class Blockchain
    {

        List<Block> blocks = new List<Block>();
        public int Difficulty { get; set; }

        //if no difficulty is specified, default is 4
        public Blockchain()
        {
            this.Difficulty = 4;
            AddGenesisBlock();
        }
        public Blockchain(int difficulty)
        {
            this.Difficulty = difficulty;
            AddGenesisBlock();
        }
        //ability to seed the block chain with data
        public Blockchain(IEnumerable<string> data)
        {
            AddGenesisBlock();
            foreach(var d in data)
            {
                var block = this.NewBlock(d);
                Console.WriteLine("Mining.....");
                MineBlock(block);
                Console.WriteLine($"Block Mined!\n{block.ToString()}");
            }
        }

        //loop through and try all ints as Nonce value until appropriate hash is found
         public void MineBlock(Block block)
        {
            for (var i = 0; i < int.MaxValue; i++)
            {
                if (block.IsValidNonce(i))
                {
                    block = block.GetBlockWithNonce(i);
                    this.AddBlockToChain(block);
                    break;
                }
            }
        }

        //add first block upon blockchain object init
        private void AddGenesisBlock() {
            var genesis = new Block(0, "GENESIS BLOCK", "0", Difficulty);
            Console.WriteLine("Mining Gensis Block....");
            for (var i = 0; i < int.MaxValue; i++)
            {
                if (genesis.IsValidNonce(i))
                {
                    genesis = genesis.GetBlockWithNonce(i);
                    blocks.Add(genesis);
                    break;
                }
            }
        }

        //create a new block where 
        public Block NewBlock(string data)
        {
            var block = new Block(blocks.Last().Id + 1, data, blocks.Last().Hash, Difficulty);
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
        public void AddBlockToChain(Block block)
        {
            VerifyChain(blocks);
            if (block.PreviousHash == blocks.Last().Hash && block.Hash.Substring(0, Difficulty) == ("").PadLeft(Difficulty, '0') && block.ValidateNonce())
            {
                blocks.Add(block);
            }
        }

        //verify chain as called by our other method, with dependancy injection
        private bool VerifyChain(List<Block> blocks)
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
            return VerifyChain(this.blocks);
        }


    }
    class Block
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string PreviousHash { get; set; }
        public string TimeStamp { get; set; }
        public string Hash { get; set; }
        int Nonce { get; set; }

        public int Difficulty { get; set; } = 4;

        //validate nonce that already exists in the object
        public bool ValidateNonce()
        {
            return IsValidNonce(this.Nonce);
        }

        
        public Block(int id, string data, string previousHash,int difficulty)
        {
            TimeStamp = DateTime.Now.ToString("MM/dd/yy H:mm:ss zzz");
            this.Id = id;
            this.Data = data;
            this.PreviousHash = previousHash;
            this.Difficulty = difficulty;

        }

        //overriding tostring so block is printed in readable format
        public override string  ToString()
        {
            return $"----------------------------------------------------------------------------\n" +
                $"Id: {this.Id}\nNonce: {this.Nonce}\nTimestamp: {this.TimeStamp}\nData: {this.Data}\nHash: {this.Hash}\nPrevious Hash: {this.PreviousHash}\n" +
                $"----------------------------------------------------------------------------\n";
        }

        //check hash for appropriate leading zeroes
        private string ReturnLeadingZeroes(int zeroes)
        {
            return ("").PadLeft(zeroes, '0');
        }

        //check if a given int would be valid as the blocks nonce
        public bool IsValidNonce(int nonce)
        {
            return GetHash(nonce).Substring(0, Difficulty) == ReturnLeadingZeroes(Difficulty);
        }


        //given a nonce, return complete block.
        public Block GetBlockWithNonce(int nonce)
        {
            if (IsValidNonce(nonce))
            {
                Nonce = nonce;
                Hash = GetHash(nonce);
            }
            return this;
        }

        //SHA256 HASH OF BLOCK
        public string GetHash(int nonce)
        {
            StringBuilder Sb = new StringBuilder();
            StringBuilder hashPrep = new StringBuilder();
            hashPrep.Append(this.Id);
            hashPrep.Append(this.TimeStamp);
            hashPrep.Append(this.Data);
            hashPrep.Append(this.PreviousHash);
            hashPrep.Append(nonce);
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(hashPrep.ToString()));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

    }
}


