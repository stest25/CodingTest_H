namespace VideoMediaApp.Services
{
    /// <summary>
    /// Handles media file validation logic
    /// </summary>
    public interface IMediaFileValidator
    {
        bool ValidateFileExtension(string fileName);
        bool ValidateFileSize(long fileSize);
        (bool IsValid, List<string> Errors) ValidateFile(IFormFile file);
    }

    /// <summary>
    /// Handles media file upload operations
    /// </summary>
    public interface IMediaFileUploader
    {
        Task<(bool Success, string? ErrorMessage)> UploadFileAsync(IFormFile file);
    }

    /// <summary>
    /// Handles media file operations
    /// </summary>
    public interface IMediaFileRepository
    {
        Task<List<(string FileName, long FileSize, string FilePath)>> GetAllMediaFilesAsync();
    }

    /// <summary>
    /// Main service interface
    /// </summary>
    public interface IMediaFileService : IMediaFileValidator, IMediaFileUploader, IMediaFileRepository
    {
    }
}
