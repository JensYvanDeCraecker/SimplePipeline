using System;
using SimplePipeline;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            IPipeline<String, String> pipeline = new Pipeline<String>()
            {
                ((Func<String,String>)(input=> input.Trim())).ToFilter(),
                ((Func<String,String>)(input=> input.ToUpper())).ToFilter(),
                ((Func<String,String>)(input=> input.Replace(" ", ""))).ToFilter(),
            };
            Console.Read();
        }
    }
}