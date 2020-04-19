using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.ViewModels
{
    public class ReviewerReviewsBooksViewModel
    {
        public ReviewerDto Reviewer { get; set; }
        public IDictionary<ReviewDto, BookDto> ReviewBook { get; set; }
    }
}