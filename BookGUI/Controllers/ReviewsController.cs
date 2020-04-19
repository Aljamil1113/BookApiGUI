using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Dtos;
using BookGUI.Services;
using BookGUI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookGUI.Controllers
{
    public class ReviewsController : Controller
    {
        IReviewRepositoryGUI reviewRepository;
        IReviewerRepositoryGUI reviewerRepository;

        public ReviewsController(IReviewRepositoryGUI _reviewRepository, IReviewerRepositoryGUI _reviewerRepository)
        {
            reviewRepository = _reviewRepository;
            reviewerRepository = _reviewerRepository;
        }


        public IActionResult Index()
        {
            var reviews = reviewRepository.GetReviews();

            if(reviews.Count() <= 0)
            {
                ViewBag.Message = "There are problems retrieving reviews " + 
                " or reviews not exist in the database.";
            }
            return View(reviews);
        }

        public IActionResult GetReviewById(int reviewId)
        {
            var review = reviewRepository.GetReview(reviewId);
           
            if(review == null)
            {
                ModelState.AddModelError("", "Error getting a review");
                ViewBag.Message = $"There was a problem retrieving review id {reviewId}" +
                $" from the database or no review with id exists."; 

                review = new ReviewDto();
            }

            var reviewer = reviewerRepository.GetReviewerPerReviews(reviewId);
            if(reviewer == null)
            {
                ModelState.AddModelError("", "Error getting a reviewer");
                ViewBag.Message += $"There was a problem retrieving reviewer " +
                $"or there is no reviewer exists from review id {reviewId}."; 

                reviewer = new ReviewerDto();
            }

            var book = reviewRepository.GetBookFromReview(reviewId);
            if(book == null)
            {
                ModelState.AddModelError("", "Error getting a book");
                ViewBag.Message += $"There was a problem retrieving book" +
                $"or there is no book exists from review id {reviewId}."; 

                book = new BookDto();
            }

            var reviewReviewerBookViewModel = new ReviewReviewerBook()
            {
                Review = review,
                Reviewer = reviewer,
                Book = book
            };

            return View(reviewReviewerBookViewModel);
        }
    }
}