using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels.Book;

namespace BookStoreMVC.Mapper;

public class MapBook
{
    public static IndexBookViewModel MapIndexBookViewModel(Book book, Author author, IEnumerable<BookGenre> bookGenres,
        Publisher publisher, Language language, IHelpers helpers)
    {
        return new IndexBookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = MapAuthor.MapAuthorViewModel(author),
            Language = language.Name,
            Genre = MapBookGenre.MapManyBookGenreViewModels(bookGenres),
            Publisher = MapPublisher.MapPublisherViewModel(publisher),
            Type = book.Type,
            CreatedAt = book.CreatedAt,
            ImageName = book.ImageName,
            SignedUrl = helpers.GenerateSignedUrl(book.ImageName).Result,
            Description = book.Description
        };
    }

    public static IEnumerable<IndexBookViewModel> MapIndexBookViewModels(IEnumerable<Book> books,
        IAuthorRepository authorRepository, IBookGenreRepository genreRepository,
        IPublisherRepository publisherRepository, ILanguageRepository languageRepository, IHelpers helpers)
    {
        return (from book in books let author = authorRepository.GetById(book.Author).Result let bookGenres = book.Genre.Select(genreRepository.GetById) let publisher = publisherRepository.GetById(book.Publisher) let lang = languageRepository.GetByIdAsync(book.Language).Result select MapIndexBookViewModel(book, author, bookGenres, publisher, lang, helpers)).ToList();
    }


    public static DetailBookViewModel MapDetailBookViewModel(Book book, Author author, IEnumerable<BookGenre> bookGenres,
        Publisher publisher, Language language, IHelpers helpers)
    {
        return new DetailBookViewModel
        {
            Id = book.Id!,
            Title = book.Title,
            PageCount = book.PageCount,
            Author = MapAuthor.MapAuthorViewModel(author),
            Language = language.Name,
            Genre = MapBookGenre.MapManyBookGenreViewModels(bookGenres),
            Publisher = MapPublisher.MapPublisherViewModel(publisher),
            Type = book.Type,
            CreatedAt = book.CreatedAt,
            ImageName = book.ImageName,
            SignedUrl = helpers.GenerateSignedUrl(book.ImageName).Result,
            PublishDate = book.PublishDate,
            Isbn = book.Isbn,
            Description = book.Description
        };
    }
}