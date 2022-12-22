using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Mapper;

public class MapPublisher
{
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
}