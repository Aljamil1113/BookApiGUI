using System;
using System.Collections.Generic;
using System.Net.Http;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public class BookRepositoryGUI : IBookRepositoryGUI
    {
        public decimal BookRating(int bookId)
        {
            decimal book = 0;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"books/{bookId}/rating");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<decimal>();
                    readTask.Wait();

                    book = readTask.Result;
                }
            }

            return book;
        }

        public BookDto GetBookById(int bookId)
        {
            
            BookDto book = new BookDto();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"books/{bookId}");
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

        public BookDto GetBookByIsbn(string isbn)
        {
            BookDto book = new BookDto();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"books/ISBN/{isbn}");
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

        public IEnumerable<BookDto> GetBooks()
        {
            
            IEnumerable<BookDto> books = new List<BookDto>();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync("books");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<BookDto>>();
                    readTask.Wait();

                    books = readTask.Result;
                }
            }

            return books;
        }
    }
}