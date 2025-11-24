using Microsoft.Extensions.Options;
using VideoMediaApp.Configuration;

namespace VideoMediaApp.Services
{
    public class MediaFileService : IMediaFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MediaFileService> _logger;
        private readonly MediaFileOptions _options;
        private readonly string _mediaPath;

        public MediaFileService(
            IWebHostEnvironment env,
            ILogger<MediaFileService> logger,
            IOptions<MediaFileOptions> options)
        {
            _env = env;
            _logger = logger;
            _options = options.Value;
            _mediaPath = Path.Combine(_env.WebRootPath, _options.MediaFolderName);

            VerifyOrCreateMediaFolder();
        }

        private void VerifyOrCreateMediaFolder()
        {
            try
            {
                if (!Directory.Exists(_mediaPath))
                {
                    Directory.CreateDirectory(_mediaPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create media folder at: {MediaPath}", _mediaPath);
                throw;
            }
        }

        #region IMediaFileValidator Implementation

        public bool ValidateFileExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogWarning("File name is null or empty");
                return false;
            }

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return _options.AllowedExtensions.Contains(extension);
        }

        public bool ValidateFileSize(long fileSize)
        {
            bool isValid = fileSize > 0 && fileSize <= _options.MaxFileSizeBytes;

            if (!isValid)
            {
                _logger.LogWarning("Invalid file size: {FileSize} bytes (Max: {MaxSize} bytes)",
                    fileSize, _options.MaxFileSizeBytes);
            }

            return isValid;
        }

        public (bool IsValid, List<string> Errors) ValidateFile(IFormFile file)
        {
            var errors = new List<string>();

            if (file == null || file.Length == 0)
            {
                errors.Add("No file provided.");
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!ValidateFileExtension(fileName))
            {
                errors.Add("Not valid file extension.");
            }

            if (!ValidateFileSize(file.Length))
            {
                double maxSizeMB = _options.MaxFileSizeBytes / (1024.0 * 1024.0);
                errors.Add("File size exceeds the max limit.");
            }

            return (!errors.Any(), errors);
        }

        #endregion

        #region IMediaFileUploader Implementation

        public async Task<(bool Success, string? ErrorMessage)> UploadFileAsync(IFormFile file)
        {
            // Validate file
            var validation = ValidateFile(file);
            if (!validation.IsValid)
            {
                //logging can be done here for each error if needed
                var errorMessage = string.Join("; ", validation.Errors);
                return (false, errorMessage);
            }

            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_mediaPath, fileName);

                if (File.Exists(filePath))
                {
                    _logger.LogInformation("Overwriting existing file: {FileName}", fileName);
                }

                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await file.CopyToAsync(stream);

                return (true, null);
            }
            //other issues like space can be handled here
            catch (IOException ioex)
            {
                //logging can be done
                return (false, "Failed to save file");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                //logging can be done
                return (false, "Access denied to save .");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while uploading file: {FileName}", file.FileName);
                return (false, $"Error uploading file: {ex.Message}");
            }
        }

        #endregion

        #region IMediaFileRepository Implementation

        public Task<List<(string FileName, long FileSize, string FilePath)>> GetAllMediaFilesAsync()
        {
            var result = new List<(string FileName, long FileSize, string FilePath)>();

            try
            {
                if (!Directory.Exists(_mediaPath))
                {
                    _logger.LogWarning("Media folder does not exist: {MediaPath}", _mediaPath);
                    return Task.FromResult(result);
                }

                foreach (var filePath in Directory.EnumerateFiles(_mediaPath))
                {
                    var fi = new FileInfo(filePath);

                    if (!_options.AllowedExtensions.Contains(fi.Extension.ToLowerInvariant()))
                        continue;

                    result.Add((
                        FileName: fi.Name,
                        FileSize: fi.Length,
                        FilePath: $"/{_options.MediaFolderName}/{fi.Name}"
                    ));
                }

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving media files from: {MediaPath}", _mediaPath);
            }

            return Task.FromResult(result);
        }

        #endregion
    }
}
