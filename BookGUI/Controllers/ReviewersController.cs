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
    public class ReviewersController : Controller
    {
         IReviewerRepositoryGUI reviewerRepository;
         IReviewRepositoryGUI reviewRepository;

         public ReviewersController(IReviewerRepositoryGUI _reviewerRepository, IReviewRepositoryGUI _reviewRepository)
         {
             reviewerRepository = _reviewerRepository;
             reviewRepository = _reviewRepository;
         }

         public IActionResult Index()
         {
             var reviewers = reviewerRepository.GetReviewers();

             if(reviewers.Count() <= 0)
             {
              ViewBag.Message = "There are problems retrieving reviewers " + 
                " or reviewers not exist in the database.";
             }

             return View(reviewers);
         }

         public IActionResult DetailReviewer(int reviewerId)
         {
             var reviewer = reviewerRepository.GetReviewer(reviewerId);

             if(reviewer == null)
             {
                 ModelState.AddModelError("", "Error getting a country");
                  ViewBag.Message = $"There was a problem retrieving country id {reviewerId}" +
                $" from the database or no country with id exists."; 

                reviewer = new ReviewerDto();
             }

             var reviews = reviewerRepository.GetReviewsByReviewer(reviewerId);

             if(reviews.Count() <= 0)
             {
                 ViewBag.ReviewMessage = $"{reviewer.LastName}, {reviewer.FirstName} has no reviewss.";
             }

             IDictionary<ReviewDto, BookDto> ReviewsAdnBooks = new Dictionary<ReviewDto, BookDto>();

             foreach(var review in reviews)
             {
                 var book = reviewRepository.GetBookFromReview(review.Id);
                 ReviewsAdnBooks.Add(review, book);
             }

             var reviewerReviewsBookModel = new ReviewerReviewsBooksViewModel()
             {
                 Reviewer = reviewer,
                 ReviewBook = ReviewsAdnBooks
             };

             return View(reviewerReviewsBookModel);
         }
        
    }
}