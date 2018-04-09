using System;

namespace ConsoleApp4
{
    class Program
    {
        static string[] seedData;
        static int difficulty = 4;
        static Algorithm algo = Algorithm.sha256;

        static void Main(string[] args)
        {
            seedData = new string[]{ "asdfadsf", "asdfasdf3223", "av35vw35w3v5", "wa3bw35b235b235b", "ab23525b235b235", "wa35bw35bw35ba3w5b" };
            var blockchain = new Blockchain(seedData, difficulty, algo);// seedData, 4, Algorithm.sha256);
            
            //create a blockchain with a difficulty of 4
           // var blockchain = new Blockchain(4,Algorithm.sha256);

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
                    Console.Write($"Average mine time (sec): {blockchain.AverageMineTime}\n\n");
                    continue;
                }
                if (ret.ToLower() == "validate")
                {
                    Console.WriteLine($"Is chain valid: {blockchain.VerifyChain()}");
                    continue;
                }
                if(ret.ToLower().StartsWith("clear"))
                {
                    try
                    {
                        string[] input = ret.Split(' ');
                        int dif = difficulty;
                    
                        if (input.Length > 1)
                            dif = Convert.ToInt32(input[1]);

                        Console.WriteLine("Creating new blockchain...");
                        blockchain = new Blockchain(dif, algo);
                        Console.WriteLine("New blockchain created!\n");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Exception: {e.Message}");
                    }

                    
                    continue;
                }

                var block = blockchain.NewBlock(ret);
                Console.WriteLine("Mining.....");
                blockchain.MineBlock(ref block);
                Console.WriteLine($"Block Mined!\n{block.ToString()}\n");

            } while (ret != "exit");
        }
     
    }
    
    
    
}


