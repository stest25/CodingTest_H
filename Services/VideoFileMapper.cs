using VideoMediaApp.Models;

namespace VideoMediaApp.Services
{
    /// <summary>
    /// Service for mapping and transforming media file data
    /// </summary>
    public interface IVideoFileMapper
    {
        List<VideoFile> MapToVideoFiles(List<(string FileName, long FileSize, string FilePath)> filesData);
    }

    public class VideoFileMapper : IVideoFileMapper
    {
        public List<VideoFile> MapToVideoFiles(List<(string FileName, long FileSize, string FilePath)> filesData)
        {
            return filesData.Select(f => new VideoFile
            {
                FileName = f.FileName,
                FileSize = f.FileSize,
                FilePath = f.FilePath
            }).ToList();
        }
    }
}
