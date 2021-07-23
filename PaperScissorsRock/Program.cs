using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;

namespace PaperScissorsRock
{
    class Computer
    {
        byte[] key = new byte[16];
        string move;
        HMACSHA256 hmac;
        byte[] hash;
        string formatedHash;
        string formatedKey;
        
        public void Move(string [] args)
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(key);

            move = args[new Random().Next(0, args.Length)];
            hmac = new HMACSHA256(key);

            byte[] moveBytes = System.Text.Encoding.UTF8.GetBytes(move);
            hash = hmac.ComputeHash(moveBytes);
        }

        public string GetHmac()
        {
            //Get hex hash
            var sBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }

            formatedHash = sBuilder.ToString();
            return formatedHash;
        }

        public string GetKey()
        {
            //Get hex key
            var sBuilder = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                sBuilder.Append(key[i].ToString("x2"));
            }

            formatedKey = sBuilder.ToString();
            return formatedKey;
        }

        public string GetMove { get { return move; } }
    }

    static class Inspector
    {
        static string[] arguments;

        static bool CheckLength()
        {
            if (arguments.Length < 3)
            {
                Console.WriteLine("The number of parameters must be >= 3.");
                return false;
            }
            return true;
        }

        static bool CheckParity()
        {
            if (arguments.Length % 2 == 0)
            {
                Console.WriteLine("The number of parameters must be odd.");
                return false;
            }
            return true;
        }

        static bool CheckRepeat()
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                for (int k = i + 1; k < arguments.Length; k++)
                {
                    if (arguments[i] == arguments[k])
                    {
                        Console.WriteLine("There are duplicate rows. Try another combination.");
                        return false;
                    }
                }
            }
            return true;
        }

        static bool CheckItem()
        {
            for(int i = 0; i<arguments.Length; i++)
            {
                if(arguments[i] == "-1")
                {
                    Console.WriteLine("-1 is a reserved point. Please, use another option.");
                    return false;
                }
            }
            return true;
        }

        public static bool IsTrueArgs(string[] args)
        {
            arguments = args;
            return CheckLength() && CheckParity() && CheckRepeat() && CheckItem();
        }
    }

    class User
    {
        string[] args;
        public User(string[] args)
        {
            this.args = args;
        }

        int moveNum = 0;
        string move = "";

        public string GetMove { get { return args[moveNum]; } }

        bool CheckMove(string moveString)
        {
            try
            {
                if (int.Parse(moveString) > args.Length - 1)
                    return false;
            }
            catch
            { 
                return false;
            }
            moveNum = int.Parse(moveString);
            return true;
        }

        void ShowMenu()
        {
            StringBuilder moves = new StringBuilder();
            moves.Append("Available moves:\n");
            for (int i = 0; i < args.Length; i++)
            {
                moves.Append(i.ToString() + " - " + args[i] + "\n");
            }
            moves.Append("-1 - exit");
            Console.WriteLine(moves.ToString());
            Console.Write("Enter you move: ");
        }

        public int Move()
        {
            while(!CheckMove(move))
            {
                ShowMenu();
                move = Console.ReadLine();
            }

            if (moveNum == -1) return -1;
            return moveNum;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (!Inspector.IsTrueArgs(args)) return;
            Computer computer = new Computer();
            computer.Move(args);
            Console.WriteLine("HMAC: {0}", computer.GetHmac());

            User user = new User(args);

            if (user.Move() == -1)
            {
                Console.WriteLine("Exit!");
                return;
            }

            Console.WriteLine("You move: {0}", user.GetMove);
            Console.WriteLine("Computer move: {0}", computer.GetMove);

            Console.WriteLine("HMAC key: {0}", computer.GetKey());
        }
    }
}