using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface ILanguageRepository
{
    IEnumerable<Language> GetAll();

    Language GetById(string languageId);

    Language Add();

    Language Update(string languageId);

    Language Delete(string languageId);
}