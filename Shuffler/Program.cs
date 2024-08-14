using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShufflerExample
{
    public class Program
    {
        public static void PrintUsage()
        {
            Console.WriteLine("Usage: Program <shuf|deshuf> <key> <string|filename>");
            Console.WriteLine("Example: Shuffler.exe shuf 12 testin.txt)");
            

        }
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                PrintUsage();
                return;
            }
            string operation = args[0];
            int key = int.Parse(args[1]);
            string input = args[2];
            
            
            
            byte[] result;
            if (operation == "shuf")
            {

                if (File.Exists(input))
                {
                    // It's a file, read the contents and shuffle
                    byte[] fileContent = File.ReadAllBytes(input);
                    result = Shuffler.Shuffle(fileContent, key);

                    // Save the result as a new shuffled file
                    string shuffledFileName = Path.ChangeExtension(input, ".shuffled");
                    File.WriteAllBytes(shuffledFileName, result);

                    Console.WriteLine($"Shuffled file saved as: {shuffledFileName}");
                }
                else
                {
                    // It's a string, shuffle it directly
                    Console.WriteLine("Not yet implemented string shuffle");
                    //result = Shuffler.Shuffle(input, key);
                    //Console.WriteLine($"Shuffled string: {result}");
                }
            }
            else if (operation == "deshuf")
            {
                byte[] fileContent = File.ReadAllBytes(input);

                result = Shuffler.DeShuffle(fileContent, key);
                // Save the result as a new deshuffled file
                string deshuffledFileName = Path.ChangeExtension(input, ".deshuffled");
                File.WriteAllBytes(deshuffledFileName, result);
                Console.WriteLine($"Deshuffled file saved as: {deshuffledFileName}");
            }
            else
            {
                Console.WriteLine("Invalid operation. Use 'shuf' for shuffling or 'deshuf' for deshuffling.");
            }
        }
    }


    public static class Shuffler
    {
        public static int[] GetShuffleExchanges(int size, int key)
        {
            int[] exchanges = new int[size - 1];
            var rand = new Random(key);
            for (int i = size - 1; i > 0; i--)
            {
                int n = rand.Next(i + 1);
                exchanges[size - 1 - i] = n;
            }
            return exchanges;
        }

        public static byte[] Shuffle(this byte[] toShuffle, int key)
        {
            int size = toShuffle.Length;
            byte[] bytes = new byte[size];
            Array.Copy(toShuffle, bytes, size);
            var exchanges = GetShuffleExchanges(size, key);
            for (int i = size - 1; i > 0; i--)
            {
                int n = exchanges[size - 1 - i];
                byte tmp = bytes[i];
                bytes[i] = bytes[n];
                bytes[n] = tmp;
            }
            return bytes;
        }

        public static byte[] DeShuffle(this byte[] shuffled, int key)
        {
            int size = shuffled.Length;
            byte[] bytes = new byte[size];
            Array.Copy(shuffled, bytes, size);
            var exchanges = GetShuffleExchanges(size, key);

            for (int i = 1; i < size; i++)
            {
                int n = exchanges[size - i - 1];
                byte tmp = bytes[i];
                bytes[i] = bytes[n];
                bytes[n] = tmp;
            }
            return bytes;
        }
    }
}
