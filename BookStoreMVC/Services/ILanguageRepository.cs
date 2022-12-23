using BookStoreMVC.Models;
using MongoDB.Driver;

namespace BookStoreMVC.Services;

public interface ILanguageRepository
{
    IEnumerable<Language> GetAll(FilterDefinition<Language>? filterDefinition = null, ProjectionDefinition<Language>? projectionDefinition = null);

    Task<Language> GetByIdAsync(string languageId, ProjectionDefinition<Language>? projectionDefinition = null);
    
    Task<Language> GetWithFilterAsync(FilterDefinition<Language> filterDefinition, ProjectionDefinition<Language>? projectionDefinition = null);

    Task AddAsync(Language model);

    Task UpdateAsync(Language model);

    Task DeleteAsync(string languageId);
}