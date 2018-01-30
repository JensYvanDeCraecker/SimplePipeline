using System;
using System.IO;

namespace SimplePipeline.Tests.Filters
{
    public class StreamFilter : IFilter<Stream, Byte[]>, IFilter<Byte[], Stream>
    {
        public Byte[] Execute(Stream input)
        {
            using (input)
            {
                Byte[] readBytes = new Byte[input.Length];
                Int32 bytesLeft = readBytes.Length;
                Int32 bytesReaded = 0;
                while (bytesLeft > 0)
                {
                    Int32 n = input.Read(readBytes, bytesReaded, bytesLeft);
                    if (n == 0)
                        bytesLeft = n;
                    bytesReaded += n;
                    bytesLeft -= n;
                }
                return readBytes;
            }
        }

        public Stream Execute(Byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            Stream memoryStream = new MemoryStream(input.Length);
            memoryStream.Write(input, 0, input.Length);
            return memoryStream;
        }
    }
}