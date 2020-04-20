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

namespace BookGUI.Controllers
{
    public class HomeController : Controller
    {
        IBookRepositoryGUI bookRepository;
        ICategoryRepositoryGUI categoryRepository;
        IAuthorRepositoryGUI authorRepository;
        IReviewRepositoryGUI reviewRepository;
        IReviewerRepositoryGUI reviewerRepository;

        ICountryRepositoryGUI countryRepository;

        public HomeController(IBookRepositoryGUI _bookRepository, ICategoryRepositoryGUI _categoryRepository, ICountryRepositoryGUI _countryRepository, 
        IAuthorRepositoryGUI _authorRepository, IReviewRepositoryGUI _reviewRepository, IReviewerRepositoryGUI _reviewerRepository)
        {
            bookRepository = _bookRepository;
            categoryRepository = _categoryRepository;
            authorRepository = _authorRepository;
            reviewRepository = _reviewRepository;
            reviewerRepository = _reviewerRepository;
            countryRepository = _countryRepository;
        }

        public IActionResult Index()
        {
            var books = bookRepository.GetBooks();

            IEnumerable<AuthorDto> authors = new List<AuthorDto>();
            IEnumerable<CategoryDto> categories = new List<CategoryDto>();
            decimal rating = 0.0m;

            if(books.Count() <= 0)
            {
                 ViewBag.Message = "There are problems retrieving books " +
                " or books does not exist in the database.";
            }

            IList<BookAuthorsCategoriesViewModel> allBooks = new List<BookAuthorsCategoriesViewModel>();          

            foreach(var book in books)
            {
                authors = authorRepository.GetAllAuthorsFromBook(book.Id);
                if(authors.Count() <= 0)
                {
                    ModelState.AddModelError("", "There are problems retrieving authors of a book");
                }
                categories = categoryRepository.GetCategoriesFromBook(book.Id);
                if(categories.Count() <= 0)
                {
                    ModelState.AddModelError("","There are problems retrieving categories of a book");
                }
                rating = bookRepository.BookRating(book.Id);     
                
                allBooks.Add(new BookAuthorsCategoriesViewModel {
                    Book = book,
                    Authors = authors,
                    Categories = categories,
                    Rating = rating
                });
            }
            return View(allBooks);
        }

        public IActionResult GetBookById(int bookId)
        {
            var book = bookRepository.GetBookById(bookId);

            if(book == null)
            {
                ModelState.AddModelError("", "Error retrieving a book");
                ViewBag.BookMessage = $"There are no book";
            }

            var categories = categoryRepository.GetCategoriesFromBook(bookId);
            if(categories.Count() <= 0)
            {
                ModelState.AddModelError("","There are problems retrieving categories of a book");
            }

            var authors = authorRepository.GetAllAuthorsFromBook(book.Id);
            if(authors.Count() <= 0)
            {
                ModelState.AddModelError("", "There are problems retrieving authors of a book");
            }
            IDictionary<AuthorDto, CountryDto> authorCountry = new Dictionary<AuthorDto, CountryDto>();
            foreach(var author in authors)
            {
                var country = countryRepository.GetCountryOfAnAuthor(author.Id);
                authorCountry.Add(author, country);
            }

            var reviews = reviewRepository.GetReviewsFromBook(bookId);
            if(reviews.Count() <= 0)
            {
                ModelState.AddModelError("", "There are problems retrieving reviews of a book");
            }
            IDictionary<ReviewDto, ReviewerDto> reviewReviewer = new Dictionary<ReviewDto, ReviewerDto>();
            foreach(var review in reviews)
            {
                var reviewer = reviewerRepository.GetReviewerPerReviews(review.Id);
                reviewReviewer.Add(review, reviewer);
            }
            decimal rating = bookRepository.BookRating(bookId);
            BookDetailsViewModel bookDetails = new BookDetailsViewModel()
            {
                Book = book,
                Categories = categories,
                AuthorsCountry = authorCountry,
                ReviewsReviewer = reviewReviewer,
                Rating = rating
            };

            return View(bookDetails);
        }
    }
}