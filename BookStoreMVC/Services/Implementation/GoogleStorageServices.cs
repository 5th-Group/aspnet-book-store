using BookStoreMVC.DataAccess;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace BookStoreMVC.Services.Implementation;

public class GoogleStorageServices : ICloudStorage
{
    private readonly BookStoreCloudStorage _cloudStorageOpts;
    private readonly GoogleCredential _googleCredential;

    public GoogleStorageServices(IOptions<BookStoreCloudStorage> cloudStorageOpts)
    {
        _cloudStorageOpts = cloudStorageOpts.Value;

        _googleCredential = GoogleCredential.FromFile(_cloudStorageOpts.GCSAuthFile);
    }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            using var storageClient = await StorageClient.CreateAsync(_googleCredential);
            await storageClient.DeleteObjectAsync(_cloudStorageOpts.GCSBucketName, fileName);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<string> GetSignedUrlAsync(string fileName)
    {
        try
        {
            var sac = _googleCredential.UnderlyingCredential as ServiceAccountCredential;
            var urlSigner = UrlSigner.FromServiceAccountCredential(sac);
            var signedUrl = await urlSigner.SignAsync(_cloudStorageOpts.GCSBucketName, fileName, TimeSpan.FromMinutes(30));
            return signedUrl.ToString();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, string fileName)
    {
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                // Create Storage Client from Google Credential
                using (var storageClient = await StorageClient.CreateAsync(_googleCredential))
                {
                    // upload file stream
                    var uploadedFile = await storageClient.UploadObjectAsync(_cloudStorageOpts.GCSBucketName, fileName, file.ContentType, memoryStream);

                    return uploadedFile.MediaLink;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}