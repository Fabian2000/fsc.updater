/// <summary>
/// Internal class, but public for different reasons. Not needed for your application.
/// </summary>
public class ProcessArgs
{
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public SerializablePSI? RestartProcessInformation { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string? UpdateZip { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public string? IgnoreFilesRegex { get; set; }
    /// <summary>
    /// Internal property, but public for different reasons. Not needed for your application.
    /// </summary>
    public int ProcessId { get; set; } = -1;
}
