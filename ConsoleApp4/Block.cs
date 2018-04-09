using System;
using System.Text;

namespace ConsoleApp4
{
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

        private double _timeToMine;
        public double TimeToMine { get { return _timeToMine; } }

        //validate nonce that already exists in the object
        public bool ValidateNonce()
        {
            return IsValidNonce(this.Nonce);
        }


        public Block(int id, string data, string previousHash, int difficulty, Algorithm algo)
        {
            TimeStamp = DateTime.Now.ToString("MM/dd/yy H:mm:ss zzz");
            this.Id = id;
            this.Data = data;
            this.PreviousHash = previousHash;
            this.Difficulty = difficulty;
            this.Algo = algo;

        }

        //overriding tostring so block is printed in readable format
        public override string ToString()
        {
            return $"----------------------------------------------------------------------------\n" +
                $"Id: {this.Id}\nNonce: {this.Nonce}\nTimestamp: {this.TimeStamp}\nData: {this.Data}\nPrevious Hash: {this.PreviousHash}\nHash: {this.Hash}\nTimeToMine: {this.TimeToMine.ToString()} sec\n" +
                $"----------------------------------------------------------------------------\n";
        }

        //check hash for appropriate leading zeroes
        private string ReturnLeadingZeroes(int zeroes)
        {
            return ("").PadLeft(zeroes, '0');
        }

        //check if a given int would be valid as the blocks nonce
        //If nonce is valid, block keeps it automatically
        public bool IsValidNonce(int nonce)
        {
            string hash = HashAlgorithm.GetHash(nonce, GetCombinedBlockDataAsString(nonce), Algo);
            bool valid = hash.Substring(0, Difficulty) == ReturnLeadingZeroes(Difficulty);
            if (valid)
            {
                this.Nonce = nonce;
                this.Hash = hash;
            }
            return valid;
        }     

        private string GetCombinedBlockDataAsString(int nonce)
        {
            StringBuilder hashPrep = new StringBuilder();
            hashPrep.Append(this.Id);
            hashPrep.Append(this.TimeStamp);
            hashPrep.Append(this.Data);
            hashPrep.Append(this.PreviousHash);
            hashPrep.Append(nonce);
            return hashPrep.ToString();
        }

        public void Mine()
        {
            DateTime s = DateTime.Now;
            var r = new Random();            
            while (true)
            {
                var rnd = r.Next(1, int.MaxValue);                
                //If nonce is valid, block keeps it automatically
                if (IsValidNonce(rnd))
                {
                    _timeToMine = (DateTime.Now - s).TotalSeconds;
                    return;
                }
            }
        }

    }
}
