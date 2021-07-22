using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PaperScissorsRock
{
    class Computer
    {
        byte[] key = new byte[16];
        string move;
        HMACSHA256 hmac;
        byte[] hash;
        string formatedHash;
        
        public void Move(string [] args)
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(key);

            move = args[new Random().Next(0, args.Length)];
            hmac = new HMACSHA256(key);

            byte[] moveBytes = System.Text.Encoding.UTF8.GetBytes(move);
            hash = hmac.ComputeHash(moveBytes);

            //Get hex hash
            var sBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }

            formatedHash = sBuilder.ToString();
        }

        public string GetHmac()
        {
            return formatedHash;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Computer computer = new Computer();
            computer.Move(args);
            Console.WriteLine(computer.GetHmac());


        }
    }
}