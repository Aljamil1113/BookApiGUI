using System.Collections;
using System.Net.Http;
using System.Collections.Generic;
using BookApi.Dtos;
using System;

namespace BookGUI.Services
{
    public class ReviewRepositoryGUI : IReviewRepositoryGUI
    {
        public BookDto GetBookFromReview(int reviewId)
        {
           BookDto book = new BookDto();
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri("http://localhost:5000/api/");

               var response = client.GetAsync($"reviews/{reviewId}/book");
               response.Wait();

               var result = response.Result;

               if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<BookDto>();
                    readTask.Wait();

                    book = readTask.Result;
                }
           }
           return book;  
        }

        public ReviewDto GetReview(int reviewId)
        {
            ReviewDto review = new ReviewDto();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"reviews/{reviewId}");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ReviewDto>();
                    readTask.Wait();

                    review = readTask.Result;
                }
            }

            return review;
        }

        public IEnumerable<ReviewDto> GetReviews()
        {
           IEnumerable<ReviewDto> reviews = new List<ReviewDto>();
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri("http://localhost:5000/api/");

               var response = client.GetAsync($"reviews");
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

        public IEnumerable<ReviewDto> GetReviewsFromBook(int bookId)
        {
           IEnumerable<ReviewDto> reviews = new List<ReviewDto>();
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri("http://localhost:5000/api/");

               var response = client.GetAsync($"reviews/books/{bookId}");
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