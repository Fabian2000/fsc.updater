using System.Diagnostics;
using System.Text;

namespace _Updater
{
    internal static class PSIParser
    {
        internal static ProcessStartInfo ToProcessStartInfo(SerializablePSI serializablePSI)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                Arguments = serializablePSI.Arguments,
                CreateNoWindow = serializablePSI.CreateNoWindow,
                ErrorDialog = serializablePSI.ErrorDialog,
                //ErrorDialogParentHandle = (IntPtr)serializablePSI.ErrorDialogParentHandle,
                FileName = serializablePSI.FileName,
                RedirectStandardError = serializablePSI.RedirectStandardError,
                RedirectStandardInput = serializablePSI.RedirectStandardInput,
                RedirectStandardOutput = serializablePSI.RedirectStandardOutput,
                StandardErrorEncoding = serializablePSI.StandardErrorEncoding == -1 ? null : Encoding.GetEncoding(serializablePSI.StandardErrorEncoding),
                StandardInputEncoding = serializablePSI.StandardInputEncoding == -1 ? null : Encoding.GetEncoding(serializablePSI.StandardInputEncoding),
                StandardOutputEncoding = serializablePSI.StandardOutputEncoding == -1 ? null : Encoding.GetEncoding(serializablePSI.StandardOutputEncoding),
                UserName = serializablePSI.UserName,
                UseShellExecute = serializablePSI.UseShellExecute,
                Verb = serializablePSI.Verb,
                WindowStyle = serializablePSI.ProcessWindowStyle,
                WorkingDirectory = serializablePSI.WorkingDirectory
            };

            if (OperatingSystem.IsWindows())
            {
                psi.Domain = serializablePSI.Domain;
                psi.LoadUserProfile = serializablePSI.LoadUserProfile;
                psi.Password = serializablePSI.Password;
                psi.PasswordInClearText = serializablePSI.PasswordClearText;
            }

            return psi;
        }
    }
}
