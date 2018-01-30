using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimplePipeline.Tests.Filters
{
    public class ComputeHashFilter : IFilter<Byte[], Byte[]>
    {
        private readonly IDictionary<Byte[], Byte[]> hashHistory = new Dictionary<Byte[], Byte[]>();
        private readonly SHA256CryptoServiceProvider hashProvider = new SHA256CryptoServiceProvider();

        public Byte[] Execute(Byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (hashHistory.ContainsKey(input))
                return hashHistory[input];
            Byte[] newHash = hashProvider.ComputeHash(input);
            hashHistory.Add(input, newHash);
            return newHash;
        }
    }
}