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

         [HttpGet]
         public IActionResult CreateReviewer()
         {
             return View();
         }

         [HttpPost]
         public IActionResult CreateReviewer(Reviewer addReviewer)
         {
             using (var client = new HttpClient())
             {
                 client.BaseAddress = new Uri("http://localhost:5000/api/");
                 var responseTask = client.PostAsJsonAsync("reviewers", addReviewer);
                 responseTask.Wait();

                 var result = responseTask.Result;

                 if(result.IsSuccessStatusCode)
                 {
                     var newReviewerTask = result.Content.ReadAsAsync<Reviewer>();
                     newReviewerTask.Wait();

                     var newReviewer = newReviewerTask.Result;

                     TempData["SuccessMessage"] = $"Reviewer {newReviewer.LastName}, {newReviewer.FirstName} successfully created!";
                     return RedirectToAction("DetailReviewer", new {reviewerId = newReviewer.Id});
                 }

                 if((int) result.StatusCode == 500)
                 {
                     ModelState.AddModelError("", "Some kind of error. Reviewer not created!");
                 }
             }
             return View();
         }

         [HttpGet]
         public IActionResult Edit(int reviewerId)
         {
             var reviewerToUpdate = reviewerRepository.GetReviewer(reviewerId);

             if(reviewerToUpdate == null)
             {
                 ModelState.AddModelError("", "Error getting the reviewer");
                 reviewerToUpdate = new ReviewerDto();
             }

             return View(reviewerToUpdate);
         }

         [HttpPost]
         public IActionResult Edit(Reviewer reviewerToUpdate)
         {
             using (var client = new HttpClient())
             {
                 client.BaseAddress = new Uri("http://localhost:5000/api/");
                 var responseTask = client.PutAsJsonAsync($"reviewers/{reviewerToUpdate.Id}", reviewerToUpdate);
                 responseTask.Wait();

                 var result = responseTask.Result;
                 if(result.IsSuccessStatusCode)
                 {
                     ViewData["SuccessMessage"] = $"Successfully update Reviewer";
                     return RedirectToAction("DetailReviewer", new{reviewerId = reviewerToUpdate.Id});
                 } 

                 if((int)result.StatusCode == 500)
                 {
                     ModelState.AddModelError("", "Some error updating the reviewer!");
                 }
             }
             var reviewerDto = reviewerRepository.GetReviewer(reviewerToUpdate.Id);
             return View(reviewerDto);
         }

         [HttpGet]
         public IActionResult Delete(int reviewerId)
         {
             var reviewer = reviewerRepository.GetReviewer(reviewerId);

             if(reviewer == null)
             {
                 ModelState.AddModelError("", "Error getting reviewer!");
                 reviewer = new ReviewerDto();
             }

             return View(reviewer);
         }

         [HttpPost]
         public IActionResult Delete(int reviewerId, string firstName, string lastName)
         {
             using (var client = new HttpClient())
             {
                 client.BaseAddress = new Uri("http://localhost:5000/api/");
                 var responseTask = client.DeleteAsync($"reviewers/{reviewerId}");
                 responseTask.Wait();

                 var result = responseTask.Result;
                 if(result.IsSuccessStatusCode)
                 {
                     TempData["SuccessMessage"] = $"Successfully delete reviewer {lastName}, {firstName}";
                     return RedirectToAction("Index","Reviewers");
                 }

                 if((int)result.StatusCode == 500)
                 {
                     ModelState.AddModelError("","Some kind of error, reviewer not deleted!");
                 }

                 var reviewerDto = reviewerRepository.GetReviewer(reviewerId);
                return View(reviewerDto);
             }
            
         }
        
    }
}