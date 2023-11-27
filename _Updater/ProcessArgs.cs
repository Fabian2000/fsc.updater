namespace _Updater
{
    public class ProcessArgs
    {
        public SerializablePSI? RestartProcessInformation { get; set; }
        public string? UpdateZip { get; set; }
        public string? IgnoreFilesRegex { get; set; }
    }
}