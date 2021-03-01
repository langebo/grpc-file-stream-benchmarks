using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;

namespace FileServer.Benchmarks.Mocks
{
    public class TestServerStreamWriter<T> : IServerStreamWriter<T>
    {
        public WriteOptions WriteOptions { get; set; }
        public List<T> Responses { get; } = new List<T>();

        public Task WriteAsync(T message)
        {
            Responses.Add(message);
            return Task.CompletedTask;
        }
    }
}