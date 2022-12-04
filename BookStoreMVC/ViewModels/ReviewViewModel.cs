namespace BookStoreMVC.ViewModels;

public class ReviewViewModel
{
    public string? Id { get; set; }

    public string Comment { get; set; } = null!;
    
    public UserViewModel Reviewer { get; set; }

    public float RatedScore { get; set; }
}