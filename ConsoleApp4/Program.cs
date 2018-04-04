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
            var blockchain = new Blockchain(4,Algorithm.sha256);

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
    enum Algorithm { sha256 = 1, sha512 = 2 };
    class Blockchain
    {
    
        
        List<Block> blocks = new List<Block>();
        public int Difficulty { get; set; }
        private Algorithm algo { get; set; }
        //if no difficulty is specified, default is 4
        public Blockchain()
        {
            this.algo = Algorithm.sha256;
            this.Difficulty = 4;
            AddGenesisBlock();
        }
        public Blockchain(int difficulty)
        {
            this.algo = Algorithm.sha256;
            this.Difficulty = difficulty;
            AddGenesisBlock();
        }
        public Blockchain(int difficulty,Algorithm algo)
        {
            this.algo = algo;
            this.Difficulty = difficulty;
            AddGenesisBlock();
        }
        //ability to seed the block chain with data
        public Blockchain(IEnumerable<string> data)
        {
            this.algo = Algorithm.sha256;
            this.Difficulty = 4;
            AddGenesisBlock();
            foreach(var d in data)
            {
                var block = this.NewBlock(d);
                Console.WriteLine("Mining.....");
                MineBlock(block);
                Console.WriteLine($"Block Mined!\n{block.ToString()}");
            }
        }
        public Blockchain(IEnumerable<string> data,  Algorithm algo)
        {
            this.algo = algo;
            this.Difficulty = 4;
            AddGenesisBlock();
            foreach (var d in data)
            {
                var block = this.NewBlock(d);
                Console.WriteLine("Mining.....");
                MineBlock(block);
                Console.WriteLine($"Block Mined!\n{block.ToString()}");
            }
        }
        public Blockchain(IEnumerable<string> data, int difficulty, Algorithm algo)
        {
            this.algo = Algorithm.sha256;
            this.Difficulty = difficulty;
            AddGenesisBlock();
            foreach (var d in data)
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
            var r = new Random();
            for (var i = 0; i < int.MaxValue; i++)
            {
                var rnd = r.Next(1, int.MaxValue);
                if (block.IsValidNonce(rnd))
                {
                    block = block.GetBlockWithNonce(rnd);
                    this.AddBlockToChain(block);
                    break;
                }
            }
        }

        //add first block upon blockchain object init
        private void AddGenesisBlock() {
            var genesis = new Block(0, "GENESIS BLOCK", "0", Difficulty,this.algo);
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
        private Algorithm Algo { get; set; }
        public int Difficulty { get; set; } = 4;

        //validate nonce that already exists in the object
        public bool ValidateNonce()
        {
            return IsValidNonce(this.Nonce);
        }

        
        public Block(int id, string data, string previousHash,int difficulty,Algorithm algo)
        {
            TimeStamp = DateTime.Now.ToString("MM/dd/yy H:mm:ss zzz");
            this.Id = id;
            this.Data = data;
            this.PreviousHash = previousHash;
            this.Difficulty = difficulty;
            this.Algo = algo;

        }

        //overriding tostring so block is printed in readable format
        public override string  ToString()
        {
            return $"----------------------------------------------------------------------------\n" +
                $"Id: {this.Id}\nNonce: {this.Nonce}\nTimestamp: {this.TimeStamp}\nData: {this.Data}\nPrevious Hash: {this.PreviousHash}\nHash: {this.Hash}\n" +
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

        private string GetHash(int nonce) {
            var ret = "";
            switch (this.Algo)
            {
                case Algorithm.sha256:
                    ret = GetSHA256Hash(nonce);
                    break;
                case Algorithm.sha512:
                    ret = GetSHA512Hash(nonce);
                    break;
            }
            return ret;
        }

        //SHA256 HASH OF BLOCK
        public string GetSHA256Hash(int nonce)
        {
            StringBuilder Sb = new StringBuilder();
           
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(GetCombinedBlockDataAsString(nonce)));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
        private  string GetSHA512Hash(int nonce)
        {
           
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = ue.GetBytes(GetCombinedBlockDataAsString(nonce));

            SHA512Managed hashString = new SHA512Managed();
            string hex = "";

            hashValue = hashString.ComputeHash(message);

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }
        private string GetCombinedBlockDataAsString(int nonce) {
            StringBuilder hashPrep = new StringBuilder();
            hashPrep.Append(this.Id);
            hashPrep.Append(this.TimeStamp);
            hashPrep.Append(this.Data);
            hashPrep.Append(this.PreviousHash);
            hashPrep.Append(nonce);
            return hashPrep.ToString();
        }

    }
}


