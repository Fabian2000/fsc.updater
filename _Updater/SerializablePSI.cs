using System.Diagnostics;
using System.Security;
using System.Text;

namespace _Updater
{
    public class SerializablePSI
    {
        public string Arguments { get; set; } = string.Empty;
        public bool CreateNoWindow { get; set; }
        public string Domain { get; set; } = string.Empty;
        public bool ErrorDialog { get; set; }
        //public nint ErrorDialogParentHandle { get; set; }
        public string FileName { get; set; } = string.Empty;
        public bool LoadUserProfile { get; set; }
        public SecureString? Password { get; set; }
        public string? PasswordClearText { get; set; }
        public bool RedirectStandardError { get; set; }
        public bool RedirectStandardInput { get; set; }
        public bool RedirectStandardOutput { get; set; }
        public int StandardErrorEncoding { get; set; } = -1;
        public int StandardInputEncoding { get; set; } = -1;
        public int StandardOutputEncoding { get; set; } = -1;
        public string UserName { get; set; } = string.Empty;
        public bool UseShellExecute { get; set; }
        public string Verb { get; set; } = string.Empty;
        public ProcessWindowStyle ProcessWindowStyle { get; set; } = ProcessWindowStyle.Normal;
        public string WorkingDirectory { get; set; } = string.Empty;
    }
}