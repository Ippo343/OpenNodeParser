using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNodeParser;
using System.IO;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText("test.cfg");

            ConfigNode node = OpenNodeParser.Parser.Parse(text);

            Console.WriteLine(node.ToString());
            Console.ReadLine();
        }
    }
}
