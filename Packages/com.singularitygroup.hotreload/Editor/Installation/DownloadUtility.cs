using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.Editor.Cli;

namespace SingularityGroup.HotReload.Editor {
    static class DownloadUtility {
        const string baseUrl = "https://cdn.hotreload.net";
        
        public static async Task<DownloadResult> DownloadFile(string url, string targetFilePath, IProgress<float> progress, CancellationToken cancellationToken) {
            var tmpDir = Path.GetDirectoryName(targetFilePath);
            Directory.CreateDirectory(tmpDir);
            using(var client = HttpClientUtils.CreateHttpClient()) {
                client.Timeout = TimeSpan.FromMinutes(10);
                return await client.DownloadAsync(url, targetFilePath, progress, cancellationToken).ConfigureAwait(false);
            }
        }
        
        public static string GetPackagePrefix(string version) {
            if (PackageConst.IsAssetStoreBuild) {
                return $"releases/asset-store/{version.Replace('.', '-')}";
            }
            return $"releases/{version.Replace('.', '-')}";
        }
        
        public static string GetDownloadUrl(string key) {
            return $"{baseUrl}/{key}";
        }
        
        public static async Task<DownloadResult> DownloadAsync(this HttpClient client, string requestUri, string destinationFilePath, IProgress<float> progress, CancellationToken cancellationToken = default(CancellationToken)) {
            // Get the http headers first to examine the content length
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false)) {
                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new DownloadException($"Download failed with status code {response.StatusCode} and reason {response.ReasonPhrase}");
                }
                var contentLength = response.Content.Headers.ContentLength;
                if (!contentLength.HasValue) {
                    throw new DownloadException("Download failed: Content length unknown");
                }
    
                using (var fs = new FileStream(destinationFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using (var download = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
    
                    // Ignore progress reporting when no progress reporter was 
                    if (progress == null) {
                        await download.CopyToAsync(fs).ConfigureAwait(false);
                    } else {
                        // Convert absolute progress (bytes downloaded) into relative progress (0% - 99.9%)
                        var relativeProgress = new Progress<long>(totalBytes => progress.Report(Math.Min(99.9f, (float)totalBytes / contentLength.Value)));
                        // Use extension method to report progress while downloading
                        await download.CopyToAsync(fs, 81920, relativeProgress, cancellationToken).ConfigureAwait(false);
                    }
                    await fs.FlushAsync().ConfigureAwait(false);
                    if (fs.Length != contentLength.Value) {
                        throw new DownloadException("Download failed: download file is corrupted");
                    }
                    return new DownloadResult(HttpStatusCode.OK, null);
                }
            }
        }
        
        static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress, CancellationToken cancellationToken) {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0) {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        [Serializable]
        public class DownloadException : ApplicationException {
            public DownloadException(string message)
                : base(message) {
            }

            public DownloadException(string message, Exception innerException)
                : base(message, innerException) {
            }
        }
    }
}
