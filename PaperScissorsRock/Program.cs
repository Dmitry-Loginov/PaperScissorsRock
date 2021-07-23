using System;
using System.Text;
using System.Security.Cryptography;

namespace PaperScissorsRock
{
    class Computer
    {
        byte[] key = new byte[16];
        string move;
        int moveNum = 0;
        HMACSHA256 hmac;
        byte[] hash;
        string formatedHash;
        string formatedKey;
        
        public void Move(string [] args)
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(key);

            Random random = new Random();
            moveNum = random.Next(0, args.Length);
            move = args[moveNum];
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
        public int GetMoveNum { get { return moveNum; } }
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
                if(arguments[i] == "0")
                {
                    Console.WriteLine("0 is a reserved point. Please, use another option.");
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

        public static void DefineWinner(string[] args, int computerMoveNum, int userMoveNum)
        {
            if(computerMoveNum == userMoveNum)
            {
                Console.WriteLine("Draw!");
                return;
            }
            int half = args.Length / 2;
            int i = computerMoveNum;
            int counter = 0;
            while(counter < half)
            {
                counter++;
                i++;
                if (i == args.Length) i = 0;
                if(i == userMoveNum)
                {
                    Console.WriteLine("You win!");
                    return;
                }
            }
            Console.WriteLine("You lose!");
            return;
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
        public int GetMoveNum { get { return moveNum; } }

        bool CheckMove(string moveString)
        {
            try
            {
                if (int.Parse(moveString) > args.Length)
                    return false;
            }
            catch
            { 
                return false;
            }
            moveNum = int.Parse(moveString);
            moveNum--;
            return true;
        }

        void ShowMenu()
        {
            StringBuilder moves = new StringBuilder();
            moves.Append("Available moves:\n");
            for (int i = 0; i < args.Length; i++)
            {
                moves.Append((i+1).ToString() + " - " + args[i] + "\n");
            }
            moves.Append("0 - exit");
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

            if (moveNum == 0) return 0;
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

            Inspector.DefineWinner(args, computer.GetMoveNum, user.GetMoveNum);

            Console.WriteLine("HMAC key: {0}", computer.GetKey());
        }
    }
}