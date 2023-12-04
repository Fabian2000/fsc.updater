using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace _Updater
{
    internal static class Program
    {
        private static ProcessArgs? _processArgs;

        static async Task Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Environment.Exit(1);
                }

                string argument = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));

                _processArgs = JsonSerializer.Deserialize(argument, ProcessArgsJsonContext.Default.ProcessArgs);

                if (_processArgs is null)
                {
                    Environment.Exit(1);
                }

                ProcessStartInfo processStartInfo = PSIParser.ToProcessStartInfo(_processArgs.RestartProcessInformation ?? new SerializablePSI());

                if (_processArgs.ProcessId > -1)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        if (!Process.GetProcesses().Any(x => x.Id == _processArgs.ProcessId))
                        {
                            break;
                        }

                        await Task.Delay(2_000);
                        try
                        {
                            Process.GetProcessById(_processArgs.ProcessId).Close();
                            Process.GetProcessById(_processArgs.ProcessId).CloseMainWindow();
                        }
                        catch { }
                    }
                }

                string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempDirectory);

                ZipFile.ExtractToDirectory(_processArgs!.UpdateZip ?? string.Empty, tempDirectory, true);

                string targetDirectory = Path.GetDirectoryName(Path.GetFullPath(_processArgs.RestartProcessInformation?.FileName ?? string.Empty)) ?? string.Empty;

                ReplaceFiles(tempDirectory, targetDirectory);

                Directory.Delete(tempDirectory, true);
                File.Delete(_processArgs?.UpdateZip ?? string.Empty);

                Process.Start(processStartInfo);

                SelfDestruct(Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Process.GetCurrentProcess().ProcessName + ".log", ex.Message);
            }
        }

        private static void ReplaceFiles(string sourceDirectory, string targetDirectory)
        {
            HashSet<string> sourceFiles = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories)
                .Select(path => path.Substring(sourceDirectory.Length)).ToHashSet();
            HashSet<string> targetFiles = Directory.GetFiles(targetDirectory, "*.*", SearchOption.AllDirectories)
                .Select(path => path.Substring(targetDirectory.Length)).ToHashSet();
            HashSet<string> targetDirectories = Directory.GetDirectories(targetDirectory, "*", SearchOption.AllDirectories)
                .Select(path => path.Substring(targetDirectory.Length)).ToHashSet();

            foreach (string? file in targetFiles.Except(sourceFiles))
            {
                if (Regex.IsMatch(file, string.IsNullOrWhiteSpace(_processArgs?.IgnoreFilesRegex) ? "^$" : _processArgs?.IgnoreFilesRegex ?? string.Empty))
                {
                    continue;
                }

                string targetFile = file.TrimStart('\\', '/').TrimStart('\\', '/');
                targetFile = Path.Combine(targetDirectory, targetFile);
                File.Delete(targetFile);
            }

            foreach (string dir in targetDirectories)
            {
                string fullPath = Path.GetFullPath(dir.TrimStart('\\', '/'));

                if (!Directory.Exists(fullPath))
                {
                    continue;
                }

                bool containsFiles = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories).Any();
                
                if (!containsFiles)
                {
                    Directory.Delete(fullPath, true);
                }
            }

            foreach (string file in sourceFiles)
            {
                string fileName = file.TrimStart('\\', '/').TrimStart('\\', '/');
                string sourceFile = Path.Combine(sourceDirectory, fileName);
                string targetFile = Path.Combine(targetDirectory, fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(targetFile) ?? string.Empty);

                if (Regex.IsMatch(Path.GetFileName(sourceFile), string.IsNullOrWhiteSpace(_processArgs?.IgnoreFilesRegex) ? "^$" : _processArgs?.IgnoreFilesRegex ?? string.Empty))
                {
                    continue;
                }

                File.Copy(sourceFile, targetFile, true);
            }
        }

        private static void SelfDestruct(string pathToExe)
        {
            string batchFilePath = Path.GetTempFileName() + ".bat";

            string batchCommands = $@"
timeout /t 5 /nobreak > NUL
del ""{pathToExe}"" /f /q
del ""{batchFilePath}"" /f /q
";

            File.WriteAllText(batchFilePath, batchCommands);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C call \"{batchFilePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo);

            Environment.Exit(0);
        }
    }
}