using System.Net;
using System.Net.Http;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Dtos;
using BookGUI.Services;
using BookGUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BookApi.Model;

namespace BookGUI.Controllers
{
    public class AuthorsController : Controller
    {
        IAuthorRepositoryGUI authorRepository;
        ICategoryRepositoryGUI categoryRepository;
        ICountryRepositoryGUI countryRepository;

        public AuthorsController(IAuthorRepositoryGUI _authorRepository, ICategoryRepositoryGUI _categoryRepository, 
        ICountryRepositoryGUI _countryRepository)
        {
            authorRepository = _authorRepository;
            categoryRepository = _categoryRepository;
            countryRepository = _countryRepository;
        }

        public IActionResult Index()
        {
            var authors = authorRepository.GetAuthors();

            if(authors.Count() <= 0)
            {
                ViewBag.Message = "There are problems retrieving authors " +
                " or authors does not exist in the database.";
            }

            return View(authors);
        }

        public IActionResult GetAuthorById(int authorId)
        {
            var author = authorRepository.GetAuthor(authorId);

            if(author == null)
            {
                ModelState.AddModelError("", "Error getting an author");
                ViewBag.Message = $"There was a problem retrieving author w/ ID {authorId}" +
                $" from the database or no author with Id {authorId} exists."; 

                author = new AuthorDto();
            }

            var country = countryRepository.GetCountryOfAnAuthor(authorId);

            if(country == null)
            {
                ModelState.AddModelError("", "Error getting an author's country");
                ViewBag.Message = $"There was a problem retrieving country w/ author ID {authorId}" +
                $" from the database or no country with author Id {authorId} exists."; 

                country = new CountryDto();
            }

            var books = authorRepository.GetBooksFromAuthor(authorId);

            if(books.Count() <= 0)
            {
                ViewBag.BookMessage += $"There are no books with author {author.LastName}, {author.FirstName}";
            }
            
            IDictionary<BookDto, IEnumerable<CategoryDto>> bookCategories = new Dictionary<BookDto, IEnumerable<CategoryDto>>();

            foreach (var book in books)
            {
                var categories = categoryRepository.GetCategoriesFromBook(book.Id);
                bookCategories.Add(book, categories);
            }
  
            AuthorBooksCategoriesViewModel authorBooksCategories = new AuthorBooksCategoriesViewModel()
            {
                Author = author,
                Country = country,
                BookCategories = bookCategories
            };

            return View(authorBooksCategories);
        }

        [HttpGet]
        public IActionResult CreateAuthor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAuthor(int countryId, Author addAuthor)
        {
            using(var client = new HttpClient())
            {
                var country = countryRepository.GetCountryById(countryId);

                if(addAuthor == null || country == null)
                {
                    ModelState.AddModelError("", "Invalid author or country. Cannot create an author.");

                    return View(addAuthor); 
                }

                addAuthor.Country = new Country
                {
                    Id = country.Id,
                    Name = country.Name
                };

                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PostAsJsonAsync("authors", addAuthor);
                responseTask.Wait();

                var result = responseTask.Result;

                if(result.IsSuccessStatusCode)
                {
                    var newAuthorTask = result.Content.ReadAsAsync<Author>();
                    newAuthorTask.Wait();

                    var newAuthor = newAuthorTask.Result;
                    ViewData["SuccessMessage"] = $"Author successfully created.";
                    return RedirectToAction("GetAuthorById", new {authorId = newAuthor.Id});

                }

                ModelState.AddModelError("", "Author not created.!");
            }
            return View(addAuthor);
        }

        [HttpGet]
        public IActionResult Edit(int countryId, int authorId)
        {
            var authorDto = authorRepository.GetAuthor(authorId);
            var countryDto = countryRepository.GetCountryOfAnAuthor(authorId);

            Author author = null;

            if(authorDto == null || countryDto == null)
            {
                ModelState.AddModelError("", "Invalid author or book. Cannot update author!");
                author = new Author();
            }

            else
            {
                author = new Author
                {
                    Id = authorDto.Id,
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Country = new Country
                    {
                        Id = countryDto.Id,
                        Name = countryDto.Name
                    }
                };


            }

            return View(author);
        }

        [HttpPost]
        public IActionResult Edit(int countryId, Author updateAuthor)
        {
            var countryDto = countryRepository.GetCountryById(countryId);
            if(countryDto == null || updateAuthor == null)
            {
                ModelState.AddModelError("","Invalid Author or country. Cannot update author!");
                updateAuthor = new Author();
            }

            else
            {
                updateAuthor.Country = new Country
                {
                    Id = countryDto.Id,
                    Name = countryDto.Name
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5000/api/");
                    var responseTask = client.PutAsJsonAsync($"authors/{updateAuthor.Id}", updateAuthor);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if(result.IsSuccessStatusCode)
                    {
                        ViewData["SuccessMessage"] = "Author successfully updated";
                        return RedirectToAction("GetAuthorById", new {authorId = updateAuthor.Id});
                    }
                    ModelState.AddModelError("", "Unexpected error. Author not updated");
                }
            }
            return View(updateAuthor);
        }

        [HttpGet]
        public IActionResult Delete(int authorId)
        {
            var authorDto = authorRepository.GetAuthor(authorId);
            var countryDto = countryRepository.GetCountryOfAnAuthor(authorId);

            Author author = null;

            if(authorDto == null || countryDto == null)
            {
                ModelState.AddModelError("", "Invalid author or book. Cannot update author!");
                author = new Author();
            }

            else
            {
                author = new Author
                {
                    Id = authorDto.Id,
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Country = new Country
                    {
                        Id = countryDto.Id,
                        Name = countryDto.Name
                    }
                };
            }
            return View(author);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeleteAuthor(int authorId)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.DeleteAsync($"authors/{authorId}");
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    ViewData["SuccessMessage"] = "Author has been deleted!";
                    return RedirectToAction("Index");
                }

                if((int)result.StatusCode == 409)
                {
                    ModelState.AddModelError("",$"Author cannot be deleted because " +
                    $"this author have an existing book.");
                }
                else
                {
                    ModelState.AddModelError("","Some kind of error. Author cannot be deleted!");
                }                
            }

            var authorDto = authorRepository.GetAuthor(authorId);
            var countryDto = countryRepository.GetCountryOfAnAuthor(authorId);

            Author author = null;
            if(authorDto == null || countryDto == null)
            {
                ModelState.AddModelError("","Some kind of error getting author.");
                author = new Author();
            }
            else
            {
                author = new Author
                {
                    Id = authorDto.Id,
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Country = new Country
                    {
                        Id = countryDto.Id,
                        Name = countryDto.Name
                    }
                };

            }
            return View(author);
        }
    }
}