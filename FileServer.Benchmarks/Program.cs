using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Grpc.Core;
using static FileServer.DownloadEndpoint;

namespace FileServer.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            _ = BenchmarkRunner.Run<DownloadBenchmark>();
        }

        [MemoryDiagnoser]
        public class DownloadBenchmark
        {
            [Params(FileSize.Small, FileSize.Medium, FileSize.Large, FileSize.Huge)]
            public FileSize FileSize;

            private readonly Server server;
            private readonly Channel channel;
            private readonly DownloadEndpointClient client;

            public DownloadBenchmark()
            {
                server = new Server(new[] { new ChannelOption(ChannelOptions.SoReuseport, 0) })
                {
                    Services = { BindService(new DownloadService()) },
                    Ports = { { "localhost", ServerPort.PickUnused, ServerCredentials.Insecure } }
                };

                server.Start();

                channel = new Channel("localhost", server.Ports.Single().BoundPort, ChannelCredentials.Insecure);
                client = new DownloadEndpointClient(channel);
            }

            [Benchmark]
            public async Task DownloadOld()
            {
                var download = client.DownloadOld(new DownloadRequest { ChunkSize = 1024 * 1024, FileSize = FileSize });
                await using var file = File.Open($"download_old_{FileSize}.file", FileMode.Create);
                await foreach (var chunk in download.ResponseStream.ReadAllAsync())
                    await file.WriteAsync(chunk.Chunk.Memory);
            }

            [Benchmark]
            public async Task DownloadSpan()
            {
                var download = client.Download(new DownloadRequest { ChunkSize = 1024 * 1024, FileSize = FileSize });
                await using var file = File.Open($"download_span_{FileSize}.file", FileMode.Create);
                await foreach (var chunk in download.ResponseStream.ReadAllAsync())
                    await file.WriteAsync(chunk.Chunk.Memory);
            }
        }
    }
}
