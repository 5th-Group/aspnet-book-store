using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Mapper;

public static class BookMapper
{
    public static IndexBookViewModel MapBookViewModel(Book book, Author author, IEnumerable<BookGenre> bookGenres, Publisher publisher, Language language, IHelpers helpers)
    {
        var viewModel = new IndexBookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            PageCount = book.PageCount,
            Author = author,
            Language = MapBookLanguageViewModel(language),
            Genre = MapBookGenreViewModels(bookGenres),
            Publisher = MapPublisherViewModel(publisher),
            Type = book.Type,
            CreatedAt = book.CreatedAt,
            ImageName = book.ImageName,
            SignedUrl = helpers.GenerateSignedUrl(book.ImageName).Result,
            PublishDate = book.PublishDate,
            Isbn = book.Isbn,
            Description = book.Description
        };

        return viewModel;
    }

    public static IEnumerable<IndexBookViewModel> MapManyBookViewModels(IEnumerable<Book> books, IAuthorRepository authorRepository, IBookGenreRepository genreRepository, ILanguageRepository languageRepository, IPublisherRepository publisherRepository, IHelpers helpers)
    {
        return (from book in books let author = authorRepository.GetById(book.Author).Result let bookGenres = book.Genre.Select(genre => genreRepository.GetById(genre)) let language = languageRepository.GetById(book.Language) let publisher = publisherRepository.GetById(book.Publisher) select MapBookViewModel(book, author, bookGenres, publisher, language, helpers)).ToList();
    }


    // public IEnumerable<IndexBookViewModel> MapManyBookViewModel(IEnumerable<Book> books)
    // {
    //     return books.Select(book => new);
    // }


    public static IEnumerable<BookGenreViewModel> MapBookGenreViewModels(IEnumerable<BookGenre> bookGenres)
    {
        return bookGenres.Select(genre => new BookGenreViewModel { Id = genre.Id, Name = genre.Name });
    }
    public static LanguageViewModel MapBookLanguageViewModel(Language language)
    {
        return new LanguageViewModel
        {
            Code = language.Code,
            Id = language.Id,
            Name = language.Name
        };
    }

    public static PublisherViewModel MapPublisherViewModel(Publisher publisher)
    {
        return new PublisherViewModel
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Contact = publisher.Contact,
            Origin = publisher.Origin
        };
    }

    public static IEnumerable<PublisherViewModel> MapManyPublisherViewModels(IEnumerable<Publisher> publishers)
    {
        return publishers.Select(publisher => new PublisherViewModel
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Contact = publisher.Contact,
            Origin = publisher.Origin
        });
    }
}