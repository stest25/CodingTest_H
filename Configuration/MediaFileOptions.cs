namespace VideoMediaApp.Configuration
{
    public class MediaFileOptions
    {
        public long MaxFileSizeBytes { get; set; } = 209715200; // 200MB
        public string[] AllowedExtensions { get; set; } = new[] { ".mp4" };
        public string MediaFolderName { get; set; } = "media";
    }
}
