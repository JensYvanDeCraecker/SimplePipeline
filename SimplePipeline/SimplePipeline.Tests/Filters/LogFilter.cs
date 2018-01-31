using System;
using System.IO;

namespace SimplePipeline.Tests.Filters
{
    public class LogFilter<T> : IFilter<T, T>
    {
        private readonly String filePath;

        public LogFilter(String filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public T Execute(T input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            File.AppendAllLines(filePath, new[] { $"Logged on {DateTime.Now}, data: {input}" });
            return input;
        }
    }
}