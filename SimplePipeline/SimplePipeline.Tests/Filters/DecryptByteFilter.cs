using System;
using System.IO;
using System.Security.Cryptography;

namespace SimplePipeline.Tests.Filters
{
    public class DecryptByteFilter : IFilter<DecryptByteFilter.Input, Byte[]>
    {
        private readonly SymmetricAlgorithm algorithm;

        public DecryptByteFilter(SymmetricAlgorithm algorithm)
        {
            this.algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        public Byte[] Execute(Input input)
        {
            if (input.Cipher == null)
                throw new ArgumentNullException(nameof(input.Cipher));
            if (input.Key == null)
                throw new ArgumentNullException(nameof(input.Key));
            if (input.InitializationVector == null)
                throw new ArgumentNullException(nameof(input.InitializationVector));
            Byte[] plain;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ICryptoTransform decryptor = algorithm.CreateDecryptor(input.Key, input.InitializationVector))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(input.Cipher, 0, input.Cipher.Length);
                        plain = memoryStream.ToArray();
                    }
                }
            }
            return plain;
        }

        public struct Input
        {
            public Byte[] Cipher { get; set; }

            public Byte[] Key { get; set; }

            public Byte[] InitializationVector { get; set; }

            public Input(Byte[] cipher, Byte[] key, Byte[] initializationVector)
            {
                Cipher = cipher;
                Key = key;
                InitializationVector = initializationVector;
            }
        }
    }
}