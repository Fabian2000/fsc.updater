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
    /// <summary>
    /// This class contains all methods you need to update your application.
    /// </summary>
    public class Updater
    {
        private string? _updateZip;

        /// <summary>
        /// A method to check for an available update.
        /// </summary>
        /// <param name="versionInfoUrl">This has to be an url which returns a text. Recommend: txt file on a webserver.</param>
        /// <returns>Returns a task. Recommend to use it correctly with async/await.</returns>
        /// <exception cref="Exception">In case of errors, like HTTP, parsing or others, this exception will throw</exception>
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

        /// <summary>
        /// A method to download a zip file in your temp folder to prepare the update.
        /// </summary>
        /// <param name="updateZipFileUrl">This defines the url, where the download target zip is avaiable. This has to be a zip file.</param>
        /// <returns>Returns a task. Recommend to use it correctly with async/await.</returns>
        /// <exception cref="Exception">In case of errors, like HTTP errors or others, this exception will throw</exception>
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

        /// <summary>
        /// A method to restart your application, after the update was downloaded. This will install the downloaded files. !(Absolute paths are recommend)!
        /// </summary>
        /// <param name="restartProcessInformation">The process information of the application which restarts your application. !(Absolute paths are recommend)!</param>
        /// <param name="useAdminPermissions">Good for situations, where the application has no permission to update.</param>
        /// <param name="forceRestart">True to kill the process instead of closing it normally.</param>
        /// <param name="ignoreFileThatMatchThisRegex">This will ignore important files, that may not be removed or replaced. This could be logs, configs, databases, ...</param>
        /// <exception cref="Exception">In case that this method fails, this exception will throw. This does not include exceptions of the update process itself</exception>
        public void RestartApplication(ProcessStartInfo restartProcessInformation, bool useAdminPermissions = false, bool forceRestart = false, string ignoreFileThatMatchThisRegex = "")
        {
            var processArgs = new ProcessArgs
            {
                RestartProcessInformation = PSIParser.FromProcessStartInfo(restartProcessInformation),
                UpdateZip = _updateZip ?? string.Empty,
                IgnoreFilesRegex = ignoreFileThatMatchThisRegex,
                ProcessId = Process.GetCurrentProcess().Id
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
                    throw new Exception("Embedded updater not found");
                }

                using (FileStream fileStream = new FileStream(updaterPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }

            ProcessStartInfo updater = new ProcessStartInfo(updaterPath);
            updater.UseShellExecute = true;
            updater.CreateNoWindow = true;
            if (useAdminPermissions)
            {
                updater.Verb = "runas";
            }
            updater.Arguments = argument;

            Process.Start(updater);

            if (forceRestart)
            {
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
