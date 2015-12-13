using System;
using org.github.fredjeck.SafeStrings;

namespace org.github.fredjeck.SafeStrings.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Usage();
                Environment.Exit(1);
            }

            switch (args[0].ToLower())
            {
                case "encrypt":
                case "enc":
                case "e":
                   Console.Out.WriteLine(args[1].EncryptUsingPassword(args[2]));
                    break;
                case "decrypt":
                case "dec":
                case "d":
                    Console.Out.WriteLine(args[1].DecryptUsingPassword(args[2]));
                    break;
                default:
                    Usage();
                    break;
            }

        }

        public static void Usage()
        {
            Console.Out.WriteLine(@"
SafeStrings
-----------
Encode and decode your strings using the SafeStings toolset.

Usage : 
    SafeStrings [Options] [String to encode] [Password]

Options :
    encrypt|enc|e : Encode the string using the provided password
    decrypt|dec|d : Decode the string using the provided password

Examples :
> SafeStrings enc ""Short string to encode"" ""MyPassword123""
x-enc:vNFWG2xadwyApHLxZ9XHbCtf65Xl+HudgO6JxWWyt0S+5UeRiypoz/MIx6xVv3CIxD6cAjWlo6E=                                                                              

> SafeStrings d ""x-enc:vNFWG2xadwyApHLxZ9XHbCtf65Xl+HudgO6JxWWyt0S+5UeRiypoz/MIx6xVv3CIxD6cAjWlo6E="" ""MyPassword123""
Short string to encode
");
        }
    }
}
