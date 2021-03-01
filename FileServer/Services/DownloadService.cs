using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace FileServer
{
    public class DownloadService : DownloadEndpoint.DownloadEndpointBase
    {
        public override async Task Download(DownloadRequest request, IServerStreamWriter<DownloadChunkResponse> responseStream, ServerCallContext context)
        {
            var filePath = Path.Combine(ToFilePath(request.FileSize));
            await using var file = File.OpenRead(filePath);

            while (file.Position < file.Length)
            {
                var array = ArrayPool<byte>.Shared.Rent(request.ChunkSize);
                try
                {
                    var bytesRead = await file.ReadAsync(array.AsMemory(), context.CancellationToken);
                    var chunk = UnsafeByteOperations.UnsafeWrap(array.AsMemory(0, bytesRead));
                    await responseStream.WriteAsync(new DownloadChunkResponse { Chunk = chunk });
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(array);
                }
            }
        }

        public override async Task DownloadOld(DownloadRequest request, IServerStreamWriter<DownloadChunkResponse> responseStream, ServerCallContext context)
        {
            await using var file = File.OpenRead(ToFilePath(request.FileSize));
            var length = (int)file.Length;
            var buffer = new byte[request.ChunkSize];
            var bytesToRead = length;
            Google.Protobuf.ByteString chunk;
            var sizeToRead = request.ChunkSize;
            for (int i = 0; i < length; i += request.ChunkSize)
            {
                if (bytesToRead < request.ChunkSize)
                {
                    sizeToRead = bytesToRead;
                }
                await file.ReadAsync(buffer, 0, sizeToRead);
                chunk = Google.Protobuf.ByteString.CopyFrom(buffer, 0, sizeToRead);

                //logger.LogDebug(string.Format(CultureInfo.InvariantCulture, "Sending: " + chunk.Length));
                await responseStream.WriteAsync(new DownloadChunkResponse { Chunk = chunk });
                bytesToRead -= request.ChunkSize; // remaining data to send
            }
        }

        private static string ToFilePath(FileSize fileSize)
        {
            var fileName = $"{fileSize}".ToLowerInvariant();
            return Path.Combine("Assets", $"{fileName}.file");
        }
    }
}
