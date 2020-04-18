using System;
using System.Collections.Generic;
using System.Net.Http;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public class CategoryRepositoryGUI : ICategoryRepositoryGUI
    {
        public IEnumerable<BookDto> GetAllBooksPerCategory(int categoryId)
        {
            IEnumerable<BookDto> books = new List<BookDto>();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"categories/{categoryId}/books");
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

        public IEnumerable<CategoryDto> GetCategories()
        {
            IEnumerable<CategoryDto> categories = new List<CategoryDto>();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync("categories");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CategoryDto>>();
                    readTask.Wait();

                    categories = readTask.Result;
                }
            }

            return categories;
        }

        public IEnumerable<CategoryDto> GetCategoriesFromBook(int bookId)
        {
            IEnumerable<CategoryDto> categories = new List<CategoryDto>();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"categories/books/{bookId}");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CategoryDto>>();
                    readTask.Wait();

                    categories = readTask.Result;
                }
            }

            return categories;
        }

        public CategoryDto GetCategory(int categoryId)
        {
            CategoryDto category = new CategoryDto();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");

                var response = client.GetAsync($"categories/{categoryId}");
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CategoryDto>();
                    readTask.Wait();

                    category = readTask.Result;
                }
            }

            return category;
        }
    }
}