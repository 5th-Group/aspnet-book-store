using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface ILanguageRepository
{
    IEnumerable<Language> GetAll();
    Language GetById(string languageId);

    Task AddAsync(Language model);

    Task UpdateAsync(Language model);

    Task DeleteAsync(string languageId);
}