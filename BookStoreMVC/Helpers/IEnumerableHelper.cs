namespace BookStoreMVC.Helpers;

public static class IEnumerableHelper
{
    public static IEnumerable<T> ToViewModel<T>(IEnumerable<T> items) where T : new()
    {
        var viewModel = new List<T>();
        var viewModelProperties = typeof(T).GetProperties();
        var modelProperties = items.GetType().GetProperties();
        for (int i = 0; i < items.Count(); i++)
        {
            for (int j = 0; j < viewModelProperties.Length - 1; j++)
            {
                viewModelProperties[j].SetValue(viewModel[i], modelProperties[j].GetValue(items));
            }
        }

        return viewModel;

        // typeof()
        // return items.Select(item => new T()
        // {
        //     
        // })
        // public IEnumerable<BookViewModel> ToViewModel(IEnumerable<Book> books)
        // {
        //     return books.Select(book => new BookViewModel
        //     {
        //         Id = book.Id,
        //         Title = book.Title,
        //         PageCount = book.PageCount,
        //         Author = book.Author,
        //         Language = book.Language,
        //         Genre = book.Genre,
        //         Type = book.Type,
        //         CreatedAt = book.CreatedAt,
        //         ImageUri = book.ImageUri,
        //         PublishDate = book.PublishDate,
        //         Publisher = book.Publisher,
        //         Isbn = book.Isbn,
        //         Description = book.Description,
        //     });
        // }
    }
}