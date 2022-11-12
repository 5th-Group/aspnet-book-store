using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface ICountryRepository
{
    IEnumerable<Country> GetAll();
    
    Country GetById(string countryId);
    
    Country Add();
    
    Country Update(string countryId);
    
    Country Delete(string countryId);
}