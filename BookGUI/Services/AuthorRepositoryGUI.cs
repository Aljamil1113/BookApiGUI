using System;
using System.Collections.Generic;
using System.Net.Http;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public class AuthorRepositoryGUI : IAuthorRepositoryGUI
    {
        public IEnumerable<AuthorDto> GetAllAuthorsFromBook(int bookId)
        {
            IEnumerable<AuthorDto> authors = new List<AuthorDto>();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"authors/books/{bookId}");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<AuthorDto>>();
                    readTask.Wait();

                    authors = readTask.Result;
                }
            }

            return authors;
        }

        public AuthorDto GetAuthor(int authorId)
        {
            AuthorDto author = new AuthorDto();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"authors/{authorId}");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<AuthorDto>();
                    readTask.Wait();

                    author = readTask.Result;
                }
            }

            return author;
        }

        public IEnumerable<AuthorDto> GetAuthors()
        {
            IEnumerable<AuthorDto> authors = new List<AuthorDto>();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync("authors");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<AuthorDto>>();
                    readTask.Wait();

                    authors = readTask.Result;
                }
            }

            return authors;
        }

        public IEnumerable<BookDto> GetBooksFromAuthor(int authorId)
        {
            IEnumerable<BookDto> books = new List<BookDto>();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"authors/{authorId}/books");
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