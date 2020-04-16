using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Model;

namespace BookApi.Services
{
    public class ReviewRepository : IReviewRepository
    {
        private BookDbContext ReviewContext;

        public ReviewRepository(BookDbContext _ReviewContext)
        {
            ReviewContext = _ReviewContext;
        }

        public bool CreateReview(Review review)
        {
            ReviewContext.Add(review);
            return SaveReview();
        }

        public bool DeleteReview(Review review)
        {
            ReviewContext.Remove(review);
            return SaveReview();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            ReviewContext.RemoveRange(reviews);
            return SaveReview();
        }

        public Book GetBookFromReview(int reviewId)
        {
            return ReviewContext.Reviews.Where(b => b.Id == reviewId).Select(b => b.Book).FirstOrDefault();
        }

        public Review GetReview(int reviewId)
        {
            return ReviewContext.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        //public Reviewer GetReviewerFromReview(int reviewId)
        //{
        //    return ReviewContext.Reviews.Where(r => r.Id == reviewId).Select(r => r.Reviewer).FirstOrDefault();
        //}

        public ICollection<Review> GetReviews()
        {
            return ReviewContext.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsFromBook(int bookId)
        {
            return ReviewContext.Books.Where(b => b.Id == bookId).SelectMany(r => r.Reviews).ToList();
        }

        //public ICollection<Review> GetReviewsFromReviewer(int reviewerId)
        //{
        //    return ReviewContext.Reviewers.Where(re => re.Id == reviewerId).SelectMany(r => r.Reviews).ToList();
        //}

        public bool ReviewIdExist(int reviewId)
        {
            return ReviewContext.Reviews.Any(r => r.Id == reviewId);
        }

        public bool SaveReview()
        {
            var review = ReviewContext.SaveChanges();
            return review >= 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            ReviewContext.Update(review);
            return SaveReview();
        }
    }
}
