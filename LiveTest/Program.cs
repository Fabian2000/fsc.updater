using System.Diagnostics;

using FSC.Updater;

namespace LiveTest
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine(args[0]);
                Console.ReadKey();
            }

            Updater updater = new Updater();
            if (await updater.CheckForUpdateAsync("https://raw.githubusercontent.com/Fabian2000/Test/main/version.txt"))
            {
                updater.DownloadProgressChanged += Updater_DownloadProgressChanged;
                await updater.DownloadUpdateAsync("https://github.com/Fabian2000/Test/raw/main/LiveTest.zip");
                Console.WriteLine();
                Console.WriteLine("Installing ...");
                string liveTestExe = Path.GetFullPath("LiveTest.exe");
                updater.RestartApplication(new ProcessStartInfo(liveTestExe, "\"Just updated\""), true, true);
            }
            Console.ReadKey();
        }

        private static void Updater_DownloadProgressChanged(object? sender, UpdaterDownloadEventArgs e)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.Write(" ".PadRight(100));
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.Write($"Downloading: [{Math.Round((double)e.CurrentSize / (double)(1024 * 1024), 2)} MB / {Math.Round((double)e.MaxSize / (double)(1024 * 1024), 2)} MB] - {e.ProgressPercentage * 100} %");
        }
    }
}