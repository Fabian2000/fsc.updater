using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Formatting = Newtonsoft.Json.Formatting;

namespace FSC.Updater
{
    public class Updater
    {
        private string? _updateZip;

        public async Task<bool> CheckForUpdateAsync(string versionInfoUrl)
        {
            string newestVersion = await QuickHttp.GetAsync(versionInfoUrl);
            if (!Version.TryParse(newestVersion, out Version? version))
            {
                throw new Exception("Unable to parse the version number");
            }

            if (version is null)
            {
                throw new Exception("Version may not be null");
            }

            Assembly? exe = Assembly.GetEntryAssembly();

            if (exe is null)
            {
                throw new Exception("Unable to get the host assembly. Call this method in an exe file");
            }

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(exe.Location);

            if (!Version.TryParse(fileVersionInfo.ProductVersion, out Version? productVersion))
            {
                throw new Exception("Unable to parse the product version of the host assembly");
            }

            if (productVersion is null)
            {
                throw new Exception("Product version of the host assembly may not be null");
            }

            if (version > productVersion)
            {
                return true;
            }

            return false;
        }

        public async Task DownloadUpdateAsync(string updateZipFileUrl)
        {
            Assembly? exe = Assembly.GetEntryAssembly();

            if (exe is null)
            {
                throw new Exception("Unable to get the host assembly. Call this method in an exe file");
            }

            string filePath = Path.ChangeExtension(exe.Location, "update.zip");

            filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(filePath));

            FileInfo zipInfo = new FileInfo(filePath);

            if (string.IsNullOrWhiteSpace(zipInfo.DirectoryName))
            {
                throw new Exception("DirectoryName of ZIP may not be null");
            }

            await QuickHttp.DownloadFileAsync(updateZipFileUrl, zipInfo.DirectoryName, zipInfo.Name);

            _updateZip = zipInfo.FullName;
        }

        public void RestartApplication(ProcessStartInfo restartProcessInformation, bool useAdminPermissions = false, string ignoreFileThatMatchThisRegex = "")
        {
            var processArgs = new ProcessArgs
            {
                RestartProcessInformation = PSIParser.FromProcessStartInfo(restartProcessInformation),
                UpdateZip = _updateZip ?? string.Empty,
                IgnoreFilesRegex = ignoreFileThatMatchThisRegex
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;

            string json = JsonConvert.SerializeObject(processArgs);

            string argument = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            Assembly thisDLL = Assembly.GetExecutingAssembly();
            if (thisDLL is null)
            {
                throw new Exception("Unable to get the current assembly");
            }

            string updaterPath = Path.ChangeExtension(_updateZip, "exe") ?? string.Empty;

            string manifestRes = thisDLL.GetManifestResourceNames().First(x => x.EndsWith("Updater.exe"));
            using (Stream resourceStream = thisDLL.GetManifestResourceStream(manifestRes) ?? throw new Exception("Unable to extract updater"))
            {
                if (resourceStream == null)
                {
                    Console.WriteLine("Embedded updater not found");
                    return;
                }

                using (FileStream fileStream = new FileStream(updaterPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }

            ProcessStartInfo updater = new ProcessStartInfo(updaterPath);
            if (useAdminPermissions)
            {
                updater.Verb = "runas";
            }
            updater.Arguments = argument;

            Process.Start(updater);

            Environment.Exit(0);
        }
    }
}
