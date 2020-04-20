using System.Collections;
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
    }
}