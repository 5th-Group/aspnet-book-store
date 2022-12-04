using BookStoreMVC.Models;

namespace BookStoreMVC.Services.Implementation;

public class HelperService : IHelpers
{
    private readonly ICloudStorage _cloudStorage;
    public HelperService(ICloudStorage cloudStorage)
    {
        _cloudStorage = cloudStorage;
    }
    public async Task<string> GenerateSignedUrl(string imgname)
    {
        if (string.IsNullOrWhiteSpace(imgname)) return string.Empty;


        var signedUrl = await _cloudStorage.GetSignedUrlAsync(imgname);

        return signedUrl;
    }
}