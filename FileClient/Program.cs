using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FileServer;
using Grpc.Core;
using Grpc.Net.Client;

namespace FileClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:9090");
            var client = new DownloadEndpoint.DownloadEndpointClient(channel);

            var request = new DownloadRequest { ChunkSize = 1024 * 1024, FileSize = FileSize.Huge };
            await DownloadOld(request, client);
            await Download(request, client);
        }

        private static async Task Download(DownloadRequest request, DownloadEndpoint.DownloadEndpointClient client)
        {
            var downloadStream = client.Download(request);

            await using var file = File.OpenWrite("download.zip");
            var sw = Stopwatch.StartNew();
            await foreach (var chunk in downloadStream.ResponseStream.ReadAllAsync())
            {
                await file.WriteAsync(chunk.Chunk.Memory);
            }

            Console.WriteLine($"NEW DONE ({sw.Elapsed:g})");
        }

        private static async Task DownloadOld(DownloadRequest request, DownloadEndpoint.DownloadEndpointClient client)
        {
            var downloadStream = client.DownloadOld(request);

            await using var file = File.OpenWrite("download_old.zip");
            var sw = Stopwatch.StartNew();
            await foreach (var chunk in downloadStream.ResponseStream.ReadAllAsync())
            {
                await file.WriteAsync(chunk.Chunk.Memory);
            }

            Console.WriteLine($"OLD DONE ({sw.Elapsed:g})");
        }
    }
}
