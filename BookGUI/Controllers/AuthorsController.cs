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

        public AuthorsController(IAuthorRepositoryGUI _authorRepository, ICategoryRepositoryGUI _categoryRepository)
        {
            authorRepository = _authorRepository;
            categoryRepository = _categoryRepository;
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

            var books = authorRepository.GetBooksFromAuthor(authorId);

            if(books.Count() <= 0)
            {
                ViewBag.BookMessage += $"There are no books with author {author.LastName}, {author.FirstName}";
            }
            

            //IEnumerable <CategoryDto> categories = new List<CategoryDto>();
            IDictionary<BookDto, IEnumerable<CategoryDto>> bookCategories = new Dictionary<BookDto, IEnumerable<CategoryDto>>();

            foreach (var book in books)
            {
                var categories = categoryRepository.GetCategoriesFromBook(book.Id);
                if(categories.Count() <= 0)
                {
                    ViewBag.CategoryMessage += $"There are no categories in {book.Title}.";
                }
                bookCategories.Add(book, categories);
            }
  
            AuthorBooksCategoriesViewModel authorBooksCategories = new AuthorBooksCategoriesViewModel()
            {
                Author = author,
                BookCategories = bookCategories
            };

            return View(authorBooksCategories);
        }
    }
}