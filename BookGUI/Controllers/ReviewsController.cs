using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Dtos;
using BookApi.Model;
using BookGUI.Services;
using BookGUI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookGUI.Controllers
{
    public class ReviewsController : Controller
    {
        IReviewRepositoryGUI reviewRepository;
        IReviewerRepositoryGUI reviewerRepository;

        IBookRepositoryGUI bookRepository;

        public ReviewsController(IReviewRepositoryGUI _reviewRepository, IReviewerRepositoryGUI _reviewerRepository,
        IBookRepositoryGUI _bookRepository)
        {
            reviewRepository = _reviewRepository;
            reviewerRepository = _reviewerRepository;
            bookRepository = _bookRepository;
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

        [HttpGet]
        public IActionResult CreateReview(int bookId, string bookTitle)
        {
            ViewBag.BookId = bookId;
            ViewBag.BookTitle = bookTitle;
            return View();
        }

        [HttpPost]
        public IActionResult CreateReview(int bookId, int ReviewerId, Review review)
        {
            using(var client = new HttpClient())
            {
                var reviewerDto = reviewerRepository.GetReviewer(ReviewerId);
                var bookDto = bookRepository.GetBookById(bookId);

                if(reviewerDto == null || bookDto == null || review == null)
                {
                    ModelState.AddModelError("", "Invalid book, reviewer, or review. Cannot create review");
                    ViewBag.BookId = bookId;
                    ViewBag.BookTitle = bookDto == null ? "" : bookDto.Title;
                    return View(review);
                }

                review.Reviewer = new Reviewer{
                    Id = reviewerDto.Id,
                    FirstName = reviewerDto.FirstName,
                    LastName = reviewerDto.LastName
                };

                review.Book = new Book
                {
                    Id = bookDto.Id,
                    Isbn = bookDto.Isbn,
                    Title = bookDto.Title,
                };

                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PostAsJsonAsync("reviews", review);
                responseTask.Wait();

                var result = responseTask.Result;

                if(result.IsSuccessStatusCode)
                {
                    var newReviewTask = result.Content.ReadAsAsync<Review>();
                    newReviewTask.Wait();

                    var newReview = newReviewTask.Result;
                    ViewData["SuccessMessage"] = $"Review for {review.Book.Title} was successfully created!";
                    return RedirectToAction("GetReviewById", new {reviewId = newReview.Id});
                }

                ModelState.AddModelError("", "Review not created.");
                ViewBag.BookId = bookId;
                ViewBag.BookTitle = bookDto == null ? "" : bookDto.Title; 
            }

            return View(review);
        }

        [HttpGet]
        public IActionResult Edit(int bookId, int reviewId, int reviewerId)
        {
            var reviewDto = reviewRepository.GetReview(reviewId);
            var reviewerDto = reviewerRepository.GetReviewerPerReviews(reviewId);
            var bookDto = bookRepository.GetBookById(bookId);

            Review review = null;

            if(reviewDto == null || reviewerDto == null || bookDto == null)
            {
                ModelState.AddModelError("", "Invalid book, reviewer or review. Cannot update review!");
                review = new Review();
            }
            else
            {
                review = new Review
                {
                    Id = reviewDto.Id,
                    HeadLine = reviewDto.HeadLine,
                    ReviewText = reviewDto.ReviewText,
                    Rating = reviewDto.Rating,

                    Reviewer = new Reviewer
                    {
                        Id = reviewerDto.Id,
                        FirstName = reviewerDto.FirstName,
                        LastName = reviewerDto.LastName
                    },

                    Book = new Book
                    {
                        Id = bookDto.Id,
                        Title = bookDto.Title,
                        Isbn = bookDto.Isbn,
                        DatePublish = bookDto.DatePublish
                    }                   
                };
            }

            return View(review);
        }

        [HttpPost]
        public IActionResult Edit(int reviewerId, Review reviewToUpdate)
        {
            var reviewer = reviewerRepository.GetReviewer(reviewerId);
            var book = reviewRepository.GetBookFromReview(reviewToUpdate.Id);

            if(reviewToUpdate == null || reviewer == null || book == null)
            {
                ModelState.AddModelError("","Invalid book, reviewer or review. Cannot update review!");
                reviewToUpdate = new Review();
            }
            else
            {
                reviewToUpdate.Reviewer = new Reviewer
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName
                };
                reviewToUpdate.Book = new Book 
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    DatePublish = book.DatePublish
                };

                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5000/api/");
                    var responseTask = client.PutAsJsonAsync($"reviews/{reviewToUpdate.Id}", reviewToUpdate);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if(result.IsSuccessStatusCode)
                    {
                        ViewData["SuccessMessage"] = "Review updated";
                        return RedirectToAction("GetReviewById", new {reviewId = reviewToUpdate.Id});
                    }

                    ModelState.AddModelError("", "Unexpected error. Review not updated");
                }
            }
            return View(reviewToUpdate);
        }

        [HttpGet]
        public IActionResult Delete(int reviewId)
        {
            var reviewDto = reviewRepository.GetReview(reviewId);
            var reviewerDto = reviewerRepository.GetReviewerPerReviews(reviewId);
            var bookDto = reviewRepository.GetBookFromReview(reviewId);

            Review review = null;
            if(reviewDto == null || reviewerDto == null || bookDto == null)
            {
                ModelState.AddModelError("", "Some kind of error gettint review, reviewer or book");
                review = new Review();
            }

            else
            {
                review = new Review
                {
                    Id = reviewDto.Id,
                    HeadLine = reviewDto.HeadLine,
                    ReviewText = reviewDto.ReviewText,
                    Rating = reviewDto.Rating,

                    Reviewer = new Reviewer
                    {
                        Id = reviewerDto.Id,
                        FirstName = reviewerDto.FirstName,
                        LastName = reviewerDto.LastName
                    },
                    Book = new Book
                    {
                        Id = bookDto.Id,
                        Isbn = bookDto.Isbn,
                        Title = bookDto.Title,
                        DatePublish = bookDto.DatePublish
                    }
                };
            }

            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteReviewPost(int reviewId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.DeleteAsync($"reviews/{reviewId}");
                responseTask.Wait();

                var result = responseTask.Result;

                if(result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Review was deleted";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("","Some kind of error. Review was not deleted!");
            }

            var reviewDto = reviewRepository.GetReview(reviewId);
            var reviewerDto = reviewerRepository.GetReviewerPerReviews(reviewId);
            var bookDto = reviewRepository.GetBookFromReview(reviewId);

            Review review = null;
            if(reviewDto == null || reviewerDto == null || bookDto == null)
            {
                ModelState.AddModelError("", "Some kind of error gettint review, reviewer or book");
                review = new Review();
            }

            else
            {
                review = new Review
                {
                    Id = reviewDto.Id,
                    HeadLine = reviewDto.HeadLine,
                    ReviewText = reviewDto.ReviewText,
                    Rating = reviewDto.Rating,

                    Reviewer = new Reviewer
                    {
                        Id = reviewerDto.Id,
                        FirstName = reviewerDto.FirstName,
                        LastName = reviewerDto.LastName
                    },
                    Book = new Book
                    {
                        Id = bookDto.Id,
                        Isbn = bookDto.Isbn,
                        Title = bookDto.Title,
                        DatePublish = bookDto.DatePublish
                    }
                };
            }


            return View(review);
        }
    }
}