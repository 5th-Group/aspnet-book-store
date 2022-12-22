using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IReviewRepository
{
    IEnumerable<Review> GetAll();

    Review GetById(string reviewId);
}