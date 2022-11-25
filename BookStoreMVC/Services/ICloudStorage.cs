namespace BookStoreMVC.Services;

public interface ICloudStorage
{
    Task DeleteFileAsync(string fileName);

    Task<string> GetSignedUrlAsync(string fileName);

    Task<string> UploadFileAsync(IFormFile file, string fileName);
    
}