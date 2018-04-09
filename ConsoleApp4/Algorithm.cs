using System;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp4
{
    enum Algorithm
    {
        sha256 = 1,
        sha512 = 2
    };

    class HashAlgorithm
    {
        public static string GetHash(int nonce, string combinedBlockData, Algorithm algo)
        {
            var ret = "";
            switch (algo)
            {
                case Algorithm.sha256:
                    ret = GetSHA256Hash(nonce, ref combinedBlockData);
                    break;
                case Algorithm.sha512:
                    ret = GetSHA512Hash(nonce, ref combinedBlockData);
                    break;
            }
            return ret;
        }
        

        //SHA256 HASH OF BLOCK
        static string GetSHA256Hash(int nonce, ref string data)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(data));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
        static string GetSHA512Hash(int nonce, ref string data)
        {

            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = ue.GetBytes(data);

            SHA512Managed hashString = new SHA512Managed();
            string hex = "";

            hashValue = hashString.ComputeHash(message);

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }
    }
}
