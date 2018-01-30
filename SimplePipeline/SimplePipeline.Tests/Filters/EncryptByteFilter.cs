using System;
using System.IO;
using System.Security.Cryptography;

namespace SimplePipeline.Tests.Filters
{
    public class EncryptByteFilter : IFilter<Byte[], EncryptByteFilter.Output>
    {
        private readonly SymmetricAlgorithm algorithm;

        public EncryptByteFilter(SymmetricAlgorithm algorithm)
        {
            this.algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        public Output Execute(Byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            algorithm.GenerateKey();
            algorithm.GenerateKey();
            Byte[] cipher;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(input, 0, input.Length);
                        cipher = memoryStream.ToArray();
                    }
                }
            }
            return new Output(cipher, algorithm.Key, algorithm.IV);
        }

        public struct Output
        {
            public Byte[] Cipher { get; set; }

            public Byte[] Key { get; set; }

            public Byte[] InitializationVector { get; set; }

            public Output(Byte[] cipher, Byte[] key, Byte[] initializationVector)
            {
                Cipher = cipher;
                Key = key;
                InitializationVector = initializationVector;
            }
        }
    }
}