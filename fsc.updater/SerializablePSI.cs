using System.Diagnostics;
using System.Security;

/// <summary>
/// Internal class, but public for different reasons. Not needed for your application.
/// </summary>
public class SerializablePSI
{
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string Arguments { get; set; } = string.Empty;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public bool CreateNoWindow { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string Domain { get; set; } = string.Empty;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public bool ErrorDialog { get; set; }
    //public nint ErrorDialogParentHandle { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public bool LoadUserProfile { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public SecureString? Password { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string? PasswordClearText { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public bool RedirectStandardError { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public bool RedirectStandardInput { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public bool RedirectStandardOutput { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public int StandardErrorEncoding { get; set; } = -1;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public int StandardInputEncoding { get; set; } = -1;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public int StandardOutputEncoding { get; set; } = -1;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public bool UseShellExecute { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string Verb { get; set; } = string.Empty;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public ProcessWindowStyle ProcessWindowStyle { get; set; } = ProcessWindowStyle.Normal;
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string WorkingDirectory { get; set; } = string.Empty;
}