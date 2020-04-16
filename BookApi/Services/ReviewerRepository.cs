using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Model;

namespace BookApi.Services
{
    public class ReviewerRepository : IReviewerRepository
    {
        private BookDbContext ReviewerContext;

        public ReviewerRepository(BookDbContext _ReviewerContext)
        {
            ReviewerContext = _ReviewerContext;
        }

        public bool CreateReviwer(Reviewer reviewer)
        {
            ReviewerContext.Add(reviewer);
            return SaveReviewer();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            ReviewerContext.Remove(reviewer);
            return SaveReviewer();
        }

        public Reviewer GetReviewer(int reviewerId)
        {
            return ReviewerContext.Reviewers.Where(re => re.Id == reviewerId).FirstOrDefault();
        }

        public Reviewer GetReviewerFromReviews(int reviewId)
        {
            return ReviewerContext.Reviews.Where(r => r.Id == reviewId).Select(re => re.Reviewer).FirstOrDefault();         
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return ReviewerContext.Reviewers.OrderBy(re => re.LastName).ToList();
        }

        public ICollection<Review> GetReviewsFromReviewer(int reviewerId)
        {
            return ReviewerContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        } 

        public bool IsReviewerIdExist(int reviwerId)
        {
            return ReviewerContext.Reviewers.Any(re => re.Id == reviwerId);
        }

        public bool SaveReviewer()
        {
            var reviewer = ReviewerContext.SaveChanges();
            return reviewer >= 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            ReviewerContext.Update(reviewer);
            return SaveReviewer();
        }
    }
}
