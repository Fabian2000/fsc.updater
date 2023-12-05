namespace FSC.Updater
{
    /// <summary>
    /// Download information class
    /// </summary>
    public class UpdaterDownloadEventArgs
    {
        internal UpdaterDownloadEventArgs()
        {
            
        }

        /// <summary>
        /// The currently downloaded size
        /// </summary>
        public long CurrentSize { get; set; }
        /// <summary>
        /// The maximum size to download
        /// </summary>
        public long MaxSize { get; set; }
        /// <summary>
        /// The value of the progress in percent 0D - 1D
        /// </summary>
        public double ProgressPercentage { get; set; }
        /// <summary>
        /// The maximum progress value (Always 100% = 1D)
        /// </summary>
        public double MaxProgressPercentage { get; } = 1D;
    }
}
