using System.Collections;
using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public interface IReviewRepositoryGUI
    {
         IEnumerable<ReviewDto> GetReviews();
         ReviewDto GetReview(int reviewId);
         IEnumerable<ReviewDto> GetReviewsFromBook(int bookId);
         BookDto GetBookFromReview(int reviewId);
    }
}