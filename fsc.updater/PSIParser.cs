using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FSC.Updater
{
    internal static class PSIParser
    {
        internal static SerializablePSI FromProcessStartInfo(ProcessStartInfo psi)
        {
            SerializablePSI spsi = new SerializablePSI
            {
                Arguments = psi.Arguments,
                CreateNoWindow = psi.CreateNoWindow,
                ErrorDialog = psi.ErrorDialog,
                //ErrorDialogParentHandle = (nint)psi.ErrorDialogParentHandle,
                FileName = psi.FileName,
                RedirectStandardError = psi.RedirectStandardError,
                RedirectStandardInput = psi.RedirectStandardInput,
                RedirectStandardOutput = psi.RedirectStandardOutput,
                UserName = psi.UserName,
                UseShellExecute = psi.UseShellExecute,
                Verb = psi.Verb,
                ProcessWindowStyle = psi.WindowStyle,
                WorkingDirectory = psi.WorkingDirectory,
#if NET5_0_OR_GREATER
                StandardErrorEncoding = psi.StandardErrorEncoding?.CodePage ?? -1,
                StandardInputEncoding = psi.StandardInputEncoding?.CodePage ?? -1,
                StandardOutputEncoding = psi.StandardOutputEncoding?.CodePage ?? -1
#endif
            };

#if NET5_0_OR_GREATER
            if (OperatingSystem.IsWindows())
#else
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
#endif
            {
                spsi.Domain = psi.Domain;
                spsi.LoadUserProfile = psi.LoadUserProfile;
            }

            return spsi;
        }
    }
}
