using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FSC.Updater
{
    internal static class QuickHttp
    {
        internal static async Task<string> GetAsync(string url)
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

        internal static async Task DownloadFileAsync(string url, string targetDir, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Unable to download the file: {response.ReasonPhrase} & status code: {response.StatusCode}");
                }

                string filePath = Path.Combine(targetDir, fileName);

                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await responseStream.CopyToAsync(fileStream);
                    }
                }
            }
        }
    }
}
