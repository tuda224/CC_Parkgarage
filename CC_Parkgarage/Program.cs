using System;
using System.IO;

namespace CC_Parkgarage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                var input = ReadInput();
                var splittedInput = input.Split(' ');
                Console.WriteLine(input);
            }
        }

        private static string ReadInput()
        {
            Console.WriteLine("Using input file: C:\\temp\\input.txt\nPress Return for run.");
            Console.ReadLine();
            var input = File.ReadAllText("C:\\temp\\input.txt");
            return input;
        }
    }
}