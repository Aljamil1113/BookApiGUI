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
using BookGUI.Components;
using BookApi.Model;

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

        [HttpGet]
        public IActionResult CreateBook()
        {
            var authors = authorRepository.GetAuthors();
            var categories = categoryRepository.GetCategories();

            if(authors.Count() <= 0 || categories.Count() <= 0)
            {
                ModelState.AddModelError("", "Some kind or error getting authors or categories");
            }

            var authorList = new AuthorsList(authors.ToList());
            var categoryList = new CategoriesList(categories.ToList());

            var createUpdateBook = new CreateUpdateBookViewModel {
                AuthorSelectListItems = authorList.GetAuthorsList(),
                CategorySelectListItems = categoryList.GetCategories()
            };

            return View(createUpdateBook);
        }

        [HttpPost]
        public IActionResult CreateBook(IEnumerable<int> AuthorIds, IEnumerable<int> CategoryIds,
        CreateUpdateBookViewModel bookToCreate)
        {
            using(var client = new HttpClient())
            {
                var book = new Book()
                {
                    Id = bookToCreate.Book.Id,
                    Isbn = bookToCreate.Book.Isbn,
                    Title = bookToCreate.Book.Title,
                    DatePublish = bookToCreate.Book.DatePublish
                };

                var uriParameters = GetAuthorsCategoriesUri(AuthorIds.ToList(), CategoryIds.ToList());

                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PostAsJsonAsync($"books?{uriParameters}", book);
                responseTask.Wait();

                var result = responseTask.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readTaskNewBook = result.Content.ReadAsAsync<Book>();
                    readTaskNewBook.Wait();

                    var newBook = readTaskNewBook.Result;

                    TempData["SuccessMessage"] = $"Book {book.Title} was successfully created.";
                    return RedirectToAction("GetBookById", new {bookId = newBook.Id});
                }

                if((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("", "ISBN already exist.");
                }
                else
                {
                    ModelState.AddModelError("","Error! Book not created!");
                }
            }
            var authorList = new AuthorsList(authorRepository.GetAuthors().ToList());
            var categoryList = new CategoriesList(categoryRepository.GetCategories().ToList());
            bookToCreate.AuthorSelectListItems = authorList.GetAuthorsList(AuthorIds.ToList());
            bookToCreate.CategorySelectListItems = categoryList.GetCategories(CategoryIds.ToList());
            bookToCreate.AuthorIds = AuthorIds.ToList();
            bookToCreate.CategoryIds = CategoryIds.ToList();
            return View(bookToCreate);
        }

        private string GetAuthorsCategoriesUri(List<int> authorIds, List<int> categoryIds)
        {
            var uri = "";
            foreach (var authorId in authorIds)
            {
                uri += $"authId={authorId}&";
            }

            foreach (var categoryId in categoryIds)
            {
                uri += $"catId={categoryId}&";
            }
            return uri;
        }

        [HttpGet]
        public IActionResult Edit(int bookId)
        {
            var bookDto = bookRepository.GetBookById(bookId);
            var authorDto = new AuthorsList(authorRepository.GetAuthors().ToList());
            var categoryDto = new CategoriesList(categoryRepository.GetCategories().ToList());

            var bookViewModel = new CreateUpdateBookViewModel()
            {
                Book = bookDto,
                AuthorSelectListItems = authorDto.GetAuthorsList(authorRepository.GetAllAuthorsFromBook(bookId).
                Select(a => a.Id).ToList()),
                CategorySelectListItems = categoryDto.GetCategories(categoryRepository.GetCategoriesFromBook(bookId).
                Select(a => a.Id).ToList())
            };

            return View(bookViewModel);
        }

        [HttpPost]
        public IActionResult Edit(List<int> authorIds, List<int> categoryIds, 
        CreateUpdateBookViewModel bookToUpdate)
        {
            using(var client = new HttpClient())
            {
                var bookDto = new Book()
                {
                    Id = bookToUpdate.Book.Id,
                    Title = bookToUpdate.Book.Title,
                    Isbn = bookToUpdate.Book.Isbn,
                    DatePublish = bookToUpdate.Book.DatePublish
                };

                var uriParameters = GetAuthorsCategoriesUri(authorIds.ToList(), categoryIds.ToList());

                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PutAsJsonAsync($"books/{bookDto.Id}?{uriParameters}", bookDto);
                responseTask.Wait();

                var result = responseTask.Result;

                if(result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Book {bookDto.Title} was successfully updated.";
                    return RedirectToAction("GetBookById", new {bookId = bookDto.Id});
                }

                if((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("","ISBN already exists!");
                }

                else
                {
                    ModelState.AddModelError("", "Error! Book not updated.");
                }
            }

            var authorList  = new AuthorsList(authorRepository.GetAuthors().ToList());
            var categoryList = new CategoriesList(categoryRepository.GetCategories().ToList());
            bookToUpdate.AuthorSelectListItems = authorList.GetAuthorsList(authorIds.ToList());
            bookToUpdate.CategorySelectListItems = categoryList.GetCategories(categoryIds.ToList());
            bookToUpdate.AuthorIds = authorIds.ToList();
            bookToUpdate.CategoryIds = categoryIds.ToList();

            return View(bookToUpdate);
        }

        [HttpGet]
        public IActionResult Delete(int bookId)
        {
            // var book = bookRepository.GetBookById(bookId);
            // var authors = new AuthorsList(authorRepository.GetAuthors().ToList());
            // var categories = new CategoriesList(categoryRepository.GetCategories().ToList());

            // var bookViewModel = new CreateUpdateBookViewModel()
            // {
            //     Book = book,
            //     AuthorSelectListItems = authors.GetAuthorsList(authorRepository.GetAllAuthorsFromBook(bookId).
            //     Select(a => a.Id).ToList()),
            //     CategorySelectListItems = categories.GetCategories(categoryRepository.GetCategoriesFromBook(bookId).
            //     Select(c => c.Id).ToList())
            // };

            var bookDto = bookRepository.GetBookById(bookId);

            return View(bookDto);
        }

        [HttpPost]
        public IActionResult Delete(int bookId, string bookTitle)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.DeleteAsync($"books/{bookId}");
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Book {bookTitle} has been deleted!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("","Some kind of error. Book cannot be deleted!");
                               
            }

            var bookDto = bookRepository.GetBookById(bookId);
            return View(bookDto);           
        } 

    }
}