using System;
using System.Text;
using SimplePipeline;
using SimplePipeline.Builder;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            IPipeline<String, Byte[]> demoPipelineLambda = PipelineBuilder.Create<String, Byte[]>(builder => builder.Chain(new TrimFilter()).Chain(((Func<String, Byte[]>) (input => Encoding.Unicode.GetBytes(input))).ToFilter()).Chain(new HashingFilter()));
            IPipeline<String, Byte[]> demoPipeline = new PipelineBuilder<String>().Chain(new TrimFilter()).Chain(((Func<String, Byte[]>)(input => Encoding.Unicode.GetBytes(input))).ToFilter()).Chain(new HashingFilter()).Build(); 
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