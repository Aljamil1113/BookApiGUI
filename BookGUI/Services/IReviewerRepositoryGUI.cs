using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public interface IReviewerRepositoryGUI
    {
         IEnumerable<ReviewerDto> GetReviewer();
         ReviewerDto GetReviewer(int reviewerId);
         IEnumerable<ReviewDto> GetReviewsByReviewer();
         ReviewerDto GetReviewerPerReviews(int reviewId);
    }
}