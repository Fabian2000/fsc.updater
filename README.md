# fsc.updater

## Introduction
`fsc.updater` is an auto-updating solution for .NET applications. It provides an easy-to-use library to integrate update functionality into your applications, ensuring that the end-users always have the latest version running.

_Made for windows only_

## How to Use
To use `fsc.updater` in your project, include the `FSC.Updater` namespace in your application and follow the sample code provided below.

## Sample Code
```csharp
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
            if (await updater.CheckForUpdateAsync("https://raw.githubusercontent.com/.../.../main/version.txt"))
            {
                await updater.DownloadUpdateAsync("https://github.com/.../.../raw/main/LiveTest.zip");
                // "LiveTest.exe is an example. Please use an absolute path instead. e.g. Path.GetFullPath("LiveTest.exe");"
                updater.RestartApplication(new System.Diagnostics.ProcessStartInfo("LiveTest.exe", "HelloWorld"));
            }
            Console.ReadKey();
        }
    }
}
```

## Methods
- `public async Task<bool> CheckForUpdateAsync(string versionInfoUrl)`
   Checks for available updates by comparing the local application version with the version information located at the provided URL.

- `public async Task DownloadUpdateAsync(string updateZipFileUrl)`
   Downloads the update package from the given URL.

- `public void RestartApplication(ProcessStartInfo restartProcessInformation, bool useAdminPermissions = false, string ignoreFileThatMatchThisRegex = "")`
   The `RestartApplication` method is a pivotal part of the `fsc.updater` library. It gracefully restarts your application after an update has been downloaded and applied. This method takes a `ProcessStartInfo` object as a parameter, which includes the necessary information to start a process, such as the application's executable name and any command-line arguments.

   Additionally, the method provides two optional parameters:
   - `useAdminPermissions`: When set to `true`, the application restarts with administrative privileges. This is essential if the update process needs to modify system-protected files or settings.
   - `ignoreFileThatMatchThisRegex`: This parameter accepts a regex pattern as a string. It is used to specify any files that should not be replaced or modified during the update process. This can be useful for preserving configuration files or logs that are unique to the user's installation.
