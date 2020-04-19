using BookApi.Dtos;

namespace BookGUI.ViewModels
{
    public class ReviewReviewerBook
    {
       public ReviewDto Review { get; set; }
       public ReviewerDto Reviewer { get; set; } 

       public BookDto Book { get; set; }
    }
}