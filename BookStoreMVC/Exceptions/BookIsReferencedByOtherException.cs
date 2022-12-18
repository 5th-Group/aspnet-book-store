namespace BookStoreMVC.Exceptions;

[Serializable]
public class BookIsReferencedByOtherException : Exception
{
    public string BookId { get; } = null!;

    public BookIsReferencedByOtherException()
    {
    }
    
    public BookIsReferencedByOtherException(string message) : base(message)
    {
    }

    public BookIsReferencedByOtherException(string message, Exception inner) : base(message, inner)
    {
    }
    
    public BookIsReferencedByOtherException(string message, string bookId) : this(message)
    {
        BookId = bookId;
    }
}