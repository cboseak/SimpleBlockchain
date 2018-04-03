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
            var blockchain = new Blockchain();
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
        public int difficulty { get; set; } = 4;
        public Blockchain()
        {
            AddGenesisBlock();
        }

        //abaility to seed the block chain with data
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
        private void AddGenesisBlock() {
            var genesis = new Block(0, "GENESIS BLOCK", "0");
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

        public Block NewBlock(string data)
        {
            var block = new Block(blocks.Last().Id + 1, data, blocks.Last().Hash);
            return block;
        }
        public string PrintBlockChain()
        {
            StringBuilder Sb = new StringBuilder();
            foreach (var b in blocks)
            {
                Sb.Append(b.ToString());
            }
            return Sb.ToString();
        }
        public void AddBlockToChain(Block block)
        {
            VerifyChain(blocks);
            if (block.PreviousHash == blocks.Last().Hash && block.Hash.Substring(0, difficulty) == ("").PadLeft(difficulty, '0') && block.ValidateNonce())
            {
                blocks.Add(block);
            }
        }

        private bool VerifyChain(List<Block> blocks)
        {
            for (var i = 0; i < blocks.Count(); i++)
            {
                if (i - 1 >= 0 && blocks.ElementAt(i).PreviousHash != blocks.ElementAt(i - 1).Hash)
                {
                    return false;
                }
            }
            return true;
        }
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

        public int difficulty { get; set; } = 4;


        public bool ValidateNonce()
        {
            return IsValidNonce(this.Nonce);
        }


        public Block(int id, string data, string previousHash)
        {
            TimeStamp = DateTime.Now.ToString("MM/dd/yy H:mm:ss zzz");
            Id = id;
            Data = data;
            PreviousHash = previousHash;

        }

        //overriding tostring so block is printed in readable format
        public override string  ToString()
        {
            return $"----------------------------------------------------------------------------\n" +
                $"Id: {this.Id}\nNonce: {this.Nonce}\nTimestamp: {this.TimeStamp}\nData: {this.Data}\nHash: {this.Hash}\nPrevious Hash: {this.PreviousHash}\n" +
                $"----------------------------------------------------------------------------\n";
        }
        private string ReturnLeadingZeroes(int zeroes)
        {
            return ("").PadLeft(zeroes, '0');
        }
        public bool IsValidNonce(int nonce)
        {
            return GetHash(nonce).Substring(0, difficulty) == ReturnLeadingZeroes(difficulty);
        }
        public Block GetBlockWithNonce(int nonce)
        {
            if (IsValidNonce(nonce))
            {
                Nonce = nonce;
                Hash = GetHash(nonce);
            }
            return this;
        }

        public string GetHash(int nonce)
        {
            StringBuilder Sb = new StringBuilder();
            StringBuilder hashPrep = new StringBuilder();
            hashPrep.Append(Id);
            hashPrep.Append(Data);
            hashPrep.Append(PreviousHash);
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


