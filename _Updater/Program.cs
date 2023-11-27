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

        static void Main(string[] args)
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

        private static void ReplaceFiles(string sourceDirectory, string targetDirectory)
        {
            HashSet<string> sourceFiles = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories)
                .Select(path => path.Substring(sourceDirectory.Length)).ToHashSet();
            HashSet<string> targetFiles = Directory.GetFiles(targetDirectory, "*.*", SearchOption.AllDirectories)
                .Select(path => path.Substring(targetDirectory.Length)).ToHashSet();

            foreach (string? file in targetFiles.Except(sourceFiles))
            {
                if (Regex.IsMatch(file, _processArgs?.IgnoreFilesRegex ?? string.Empty))
                {
                    continue;
                }

                File.Delete(Path.Combine(targetDirectory, file));
            }

            foreach (string file in sourceFiles)
            {
                string sourceFile = Path.Combine(sourceDirectory, file);
                string targetFile = Path.Combine(targetDirectory, file);

                Directory.CreateDirectory(Path.GetDirectoryName(targetFile) ?? string.Empty);

                if (Regex.IsMatch(Path.GetFileName(sourceFile), _processArgs?.IgnoreFilesRegex ?? string.Empty))
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