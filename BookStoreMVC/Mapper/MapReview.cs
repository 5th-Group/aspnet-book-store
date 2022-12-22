using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BookStoreMVC.Mapper;

public class MapReview
{
    public static IEnumerable<ReviewViewModel> MapReviewViewModels(IEnumerable<Review> reviews,
        UserManager<User> userManager)
    {
        return reviews.Select(r => new ReviewViewModel
        {
            Id = r.Id,
            Comment = r.Comment,
            Reviewer = MapUser.MapUserViewModel(userManager.FindByIdAsync(r.Reviewer).Result),
            RatedScore = r.RatedScore
        });
    }
}