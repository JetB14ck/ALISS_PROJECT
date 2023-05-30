using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALISS.Data
{
    public class DownloadFileService
    {
        private IJSRuntime JSRuntime { get; set; }
        private IConfiguration _configuration { get; }
        public DownloadFileService(IJSRuntime jSRuntime, IConfiguration configuration)
        {
            JSRuntime = jSRuntime;
            _configuration = configuration;
            Task.Run(async () => await JSRuntime.InvokeVoidAsync("eval", DownloadFileScript.InitializeBlazorDownloadFile()));
        }

        public ValueTask AddBuffer(string bytesBase64)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", bytesBase64);
        }
        /// <inheritdoc/>>
        public ValueTask AddBuffer(string bytesBase64, CancellationToken cancellationToken)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationToken, bytesBase64);
        }
        /// <inheritdoc/>
        public ValueTask AddBuffer(string bytesBase64, TimeSpan timeOut)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOut, bytesBase64);
        }
        /// <inheritdoc/>
        public ValueTask AddBuffer(byte[] bytes)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", bytes.Select(s => s));
        }
        /// <inheritdoc/>
        public ValueTask AddBuffer(byte[] bytes, CancellationToken cancellationToken)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationToken, bytes.Select(s => s));
        }
        /// <inheritdoc/>
        public ValueTask AddBuffer(byte[] bytes, TimeSpan timeOut)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOut, bytes.Select(s => s));
        }
        /// <inheritdoc/>
        public ValueTask AddBuffer(IEnumerable<byte> bytes)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", bytes);
        }
        /// <inheritdoc/>
        public ValueTask AddBuffer(IEnumerable<byte> bytes, CancellationToken cancellationToken)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationToken, bytes);
        }
        /// <inheritdoc/>
        public ValueTask AddBuffer(IEnumerable<byte> bytes, TimeSpan timeOut)
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOut, bytes);
        }        
        /// <inheritdoc/>
        public ValueTask<bool> AnyBuffer()
        {
            return JSRuntime.InvokeAsync<bool>("_blazorDownloadFileAnyBuffer");
        }
        /// <inheritdoc/>
        public ValueTask<int> BuffersCount()
        {
            return JSRuntime.InvokeAsync<int>("_blazorDownloadFileBuffersCount");
        }
        /// <inheritdoc/>
        public ValueTask ClearBuffers()
        {
            return JSRuntime.InvokeVoidAsync("_blazorDownloadFileClearBuffer");
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadBase64Buffers(string fileName, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64StringPartitioned", fileName, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadBase64Buffers(string fileName, CancellationToken cancellationToken, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64StringPartitioned", cancellationToken, fileName, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadBase64Buffers(string fileName, TimeSpan timeOut, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64StringPartitioned", timeOut, fileName, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadBinaryBuffers(string fileName, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", fileName, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadBinaryBuffers(string fileName, CancellationToken cancellationToken, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", cancellationToken, fileName, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadBinaryBuffers(string fileName, TimeSpan timeOut, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", timeOut, fileName, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadFile(string fileName, string bytesBase64, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64String", fileName, bytesBase64, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadFile(string fileName, string bytesBase64, CancellationToken cancellationToken, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64String", cancellationToken, fileName, bytesBase64, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadFile(string fileName, string bytesBase64, TimeSpan timeOut, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64String", timeOut, fileName, bytesBase64, contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadFile(string fileName, byte[] bytes, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64String", fileName, Convert.ToBase64String(bytes), contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadFile(string fileName, byte[] bytes, CancellationToken cancellationToken, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64String", cancellationToken, fileName, Convert.ToBase64String(bytes), contentType);
        }
        /// <inheritdoc/>
        public ValueTask<DowloadFileResult> DownloadFile(string fileName, byte[] bytes, TimeSpan timeOut, string contentType = "application/octet-stream")
        {
            return JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64String", timeOut, fileName, Convert.ToBase64String(bytes), contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, IEnumerable<byte> bytes, string contentType = "application/octet-stream")
        {
            await ClearBuffers();
            await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", bytes);
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, IEnumerable<byte> bytes, CancellationToken cancellationToken, string contentType = "application/octet-stream")
        {
            await ClearBuffers();
            await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationToken, bytes);
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", cancellationToken, fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, IEnumerable<byte> bytes, TimeSpan timeOut, string contentType = "application/octet-stream")
        {
            await ClearBuffers();
            await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOut, bytes);
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", timeOut, fileName, contentType);
        }  
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, string bytesBase64, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            foreach (var partFile in Partition(bytesBase64, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytesBase64.Length;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", string.Join("", partFile));
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64StringPartitioned", fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, string bytesBase64, CancellationToken cancellationToken, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            foreach (var partFile in Partition(bytesBase64, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytesBase64.Length;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationToken, string.Join("", partFile));
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64StringPartitioned", cancellationToken, fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, string bytesBase64, TimeSpan timeOut, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            foreach (var partFile in Partition(bytesBase64, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytesBase64.Length;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOut, string.Join("", partFile));
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileBase64StringPartitioned", timeOut, fileName, contentType);
        }
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, byte[] bytes, CancellationToken cancellationToken, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            foreach (var partFile in Partition(bytes, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytes.Length;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationToken, partFile);
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", cancellationToken, fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, byte[] bytes, TimeSpan timeOut, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            foreach (var partFile in Partition(bytes, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytes.Length;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOut, partFile);
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", timeOut, fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, IEnumerable<byte> bytes, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            var bytesLength = bytes.Count();
            foreach (var partFile in Partition(bytes, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytesLength;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", partFile);
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, IEnumerable<byte> bytes, CancellationToken cancellationToken, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            var bytesLength = bytes.Count();
            foreach (var partFile in Partition(bytes, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytesLength;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationToken, partFile);
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", cancellationToken, fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, IEnumerable<byte> bytes, TimeSpan timeOut, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var bytesReaded = 0;
            var bytesLength = bytes.Count();
            foreach (var partFile in Partition(bytes, bufferSize))
            {
                bytesReaded += partFile.Count;
                var totalProgress = bytesReaded / bytesLength;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOut, partFile);
            }
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", timeOut, fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, Stream stream, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var totalOfBytes = (int)stream.Length;
            var totalOfBytesReaded = 0;
            var pendingBytesToRead = totalOfBytes;
            do
            {
                var currentBufferSize = bufferSize > totalOfBytes ? totalOfBytes : bufferSize > pendingBytesToRead ? pendingBytesToRead : bufferSize;
                var buffer = new byte[currentBufferSize];
                totalOfBytesReaded += await stream.ReadAsync(buffer, 0, currentBufferSize);
                pendingBytesToRead -= totalOfBytesReaded;
                var totalProgress = totalOfBytesReaded / totalOfBytes;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", buffer.Select(b => b));
            } while (pendingBytesToRead > 0);
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, Stream stream, CancellationToken cancellationTokenBytesRead, CancellationToken cancellationTokenJavaScriptInterop, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var totalOfBytes = (int)stream.Length;
            var totalOfBytesReaded = 0;
            var pendingBytesToRead = totalOfBytes;
            do
            {
                var currentBufferSize = bufferSize > totalOfBytes ? totalOfBytes : bufferSize > pendingBytesToRead ? pendingBytesToRead : bufferSize;
                var buffer = new byte[currentBufferSize];
                totalOfBytesReaded += await stream.ReadAsync(buffer, 0, currentBufferSize, cancellationTokenBytesRead);
                pendingBytesToRead -= totalOfBytesReaded;
                var totalProgress = totalOfBytesReaded / totalOfBytes;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", cancellationTokenJavaScriptInterop, buffer.Select(b => b));
            } while (pendingBytesToRead > 0);
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", cancellationTokenJavaScriptInterop, fileName, contentType);
        }
        /// <inheritdoc/>
        public async ValueTask<DowloadFileResult> DownloadFile(string fileName, Stream stream, CancellationToken cancellationTokenBytesRead, TimeSpan timeOutJavaScriptInterop, int bufferSize = 32768, string contentType = "application/octet-stream", IProgress<double>? progress = null)
        {
            await ClearBuffers();
            var totalOfBytes = (int)stream.Length;
            var totalOfBytesReaded = 0;
            var pendingBytesToRead = totalOfBytes;
            do
            {
                var currentBufferSize = bufferSize > totalOfBytes ? totalOfBytes : bufferSize > pendingBytesToRead ? pendingBytesToRead : bufferSize;
                var buffer = new byte[currentBufferSize];
                totalOfBytesReaded += await stream.ReadAsync(buffer, 0, currentBufferSize, cancellationTokenBytesRead);
                pendingBytesToRead -= totalOfBytesReaded;
                var totalProgress = totalOfBytesReaded / totalOfBytes;
                progress?.Report(totalProgress);
                await JSRuntime.InvokeVoidAsync("_blazorDownloadFileBuffersPush", timeOutJavaScriptInterop, buffer.Select(b => b));
            } while (pendingBytesToRead > 0);
            return await JSRuntime.InvokeAsync<DowloadFileResult>("_blazorDowloadFileByteArrayPartitioned", timeOutJavaScriptInterop, fileName, contentType);
        }

        internal IEnumerable<IList<T>> Partition<T>(IEnumerable<T> source, int bufferSize)
        {
            for (int i = 0; i < Math.Ceiling(source.Count() / (double)bufferSize); i++)
            {
                yield return new List<T>(source.Skip(bufferSize * i).Take(bufferSize));
            }
        }
    }
}
