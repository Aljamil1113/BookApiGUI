using BookApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi.Services
{
    public interface IReviewerRepository
    {
        ICollection<Reviewer> GetReviewers();
        Reviewer GetReviewer(int reviewerId);
        Reviewer GetReviewerFromReviews(int reviewId);
        ICollection<Review> GetReviewsFromReviewer(int reviewerId);

        bool IsReviewerIdExist(int reviewerId);


        bool CreateReviwer(Reviewer reviewer);
        bool UpdateReviewer(Reviewer reviewer);
        bool DeleteReviewer(Reviewer reviewer);
        bool SaveReviewer();
    }
}
