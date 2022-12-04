using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IHelpers
{


    public Task<string> GenerateSignedUrl(string imgname);

}