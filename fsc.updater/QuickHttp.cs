using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FSC.Updater
{
    internal class QuickHttp
    {
        internal event EventHandler<UpdaterDownloadEventArgs>? DownloadProgressChanged;

        internal async Task<string> GetAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Unable to request the string content: {response.ReasonPhrase} & status code: {response.StatusCode}");
                }

                return await response.Content.ReadAsStringAsync();
            }
        }

        internal async Task DownloadFileAsync(string url, string targetDir, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Unable to download the file: {response.ReasonPhrase} & status code: {response.StatusCode}");
                }

                long totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);

                string filePath = Path.Combine(targetDir, fileName);

                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[8192];
                        long totalReadBytes = 0;
                        int bytesRead;

                        while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalReadBytes += bytesRead;

                            double progressPercentage = totalBytes > 0 ? (double)totalReadBytes / totalBytes : -1;

                            UpdaterDownloadEventArgs args = new UpdaterDownloadEventArgs
                            {
                                CurrentSize = totalReadBytes,
                                MaxSize = totalBytes,
                                ProgressPercentage = progressPercentage
                            };

                            DownloadProgressChanged?.Invoke(this, args);
                        }
                    }
                }
            }
        }
    }
}
