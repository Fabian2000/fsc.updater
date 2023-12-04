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
                await updater.DownloadUpdateAsync("https://github.com/Fabian2000/Test/raw/main/LiveTest.zip");
                string liveTestExe = Path.GetFullPath("LiveTest.exe");
                updater.RestartApplication(new System.Diagnostics.ProcessStartInfo(liveTestExe, "Hallo Welt"), true, true);
            }
            Console.ReadKey();
        }
    }
}