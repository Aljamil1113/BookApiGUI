using System;
using System.Collections.Generic;
using System.Net.Http;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public class ReviewerRepositoryGUI : IReviewerRepositoryGUI
    {
        public IEnumerable<ReviewerDto> GetReviewers()
        {
           IEnumerable<ReviewerDto> reviewers = new List<ReviewerDto>();
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri("http://localhost:5000/api/");

               var response = client.GetAsync($"reviewers");
               response.Wait();

               var result = response.Result;

               if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ReviewerDto>>();
                    readTask.Wait();

                    reviewers = readTask.Result;
                }
           }
           return reviewers;  
        }

        public ReviewerDto GetReviewer(int reviewerId)
        {
           ReviewerDto reviewer = new ReviewerDto();
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri("http://localhost:5000/api/");

               var response = client.GetAsync($"reviewers/{reviewerId}");
               response.Wait();

               var result = response.Result;

               if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ReviewerDto>();
                    readTask.Wait();

                    reviewer = readTask.Result;
                }
           }
           return reviewer;  
        }

        public ReviewerDto GetReviewerPerReviews(int reviewId)
        {
           ReviewerDto reviewer = new ReviewerDto();
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri("http://localhost:5000/api/");

               var response = client.GetAsync($"reviewers/{reviewId}/reviewer");
               response.Wait();

               var result = response.Result;

               if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ReviewerDto>();
                    readTask.Wait();

                    reviewer = readTask.Result;
                }
           }
           return reviewer;  
        }

        public IEnumerable<ReviewDto> GetReviewsByReviewer(int reviewerId)
        {
           IEnumerable<ReviewDto> reviews = new List<ReviewDto>();
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri("http://localhost:5000/api/");

               var response = client.GetAsync($"reviewers/{reviewerId}/reviews");
               response.Wait();

               var result = response.Result;

               if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ReviewDto>>();
                    readTask.Wait();

                    reviews = readTask.Result;
                }
           }
           return reviews;  
        }
    }
}