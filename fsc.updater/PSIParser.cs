using System.Diagnostics;

namespace FSC.Updater
{
    internal static class PSIParser
    {
        internal static SerializablePSI FromProcessStartInfo(ProcessStartInfo psi)
        {
            return new SerializablePSI
            {
                Arguments = psi.Arguments,
                CreateNoWindow = psi.CreateNoWindow,
                Domain = psi.Domain,
                ErrorDialog = psi.ErrorDialog,
                //ErrorDialogParentHandle = (nint)psi.ErrorDialogParentHandle,
                FileName = psi.FileName,
                LoadUserProfile = psi.LoadUserProfile,
                RedirectStandardError = psi.RedirectStandardError,
                RedirectStandardInput = psi.RedirectStandardInput,
                RedirectStandardOutput = psi.RedirectStandardOutput,
                UserName = psi.UserName,
                UseShellExecute = psi.UseShellExecute,
                Verb = psi.Verb,
                ProcessWindowStyle = psi.WindowStyle,
                WorkingDirectory = psi.WorkingDirectory,
#if NET6_0_OR_GREATER
                StandardErrorEncoding = psi.StandardErrorEncoding?.CodePage ?? -1,
                StandardInputEncoding = psi.StandardInputEncoding?.CodePage ?? -1,
                StandardOutputEncoding = psi.StandardOutputEncoding?.CodePage ?? -1
#endif
            };
        }
    }
}
