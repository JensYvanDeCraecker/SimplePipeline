using System;
using System.Security.Cryptography;
using System.Text;
using SimplePipeline;

namespace ConsoleTest
{
    public class HashingFilter : IFilter<Byte[], Byte[]>
    {
        private readonly SHA256CryptoServiceProvider hashingProvider = new SHA256CryptoServiceProvider();
        public Byte[] Execute(Byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return hashingProvider.ComputeHash(input);
        }
    }
}