using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public interface IReviewerRepositoryGUI
    {
         IEnumerable<ReviewerDto> GetReviewers();
         ReviewerDto GetReviewer(int reviewerId);
         IEnumerable<ReviewDto> GetReviewsByReviewer(int reviewerId);
         ReviewerDto GetReviewerPerReviews(int reviewId);
    }
}