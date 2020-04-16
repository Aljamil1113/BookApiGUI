using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Dtos;
using BookApi.Model;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private IBookRepository bookRepository { get; set; }
        public IAuthorRepository authorRepository { get; set; }
        public ICategoryRepository categoryRepository { get; set; }
        public IReviewRepository reviewRepository { get; set; }

        public BooksController(IBookRepository _bookRepository, IAuthorRepository _authorRepository, 
        ICategoryRepository _categoryRepository, IReviewRepository _reviewRepository)
        {
            bookRepository = _bookRepository;
            authorRepository = _authorRepository;
            categoryRepository = _categoryRepository;
            reviewRepository = _reviewRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        // api/books
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooks()
        {
            var books = bookRepository.GetBooks().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                bookDtos.Add(new BookDto
                {
                    Id = book.Id,
                    Isbn = book.Isbn,
                    Title = book.Title,
                    DatePublish = book.DatePublish
                });
            }

            return Ok(bookDtos);
        }

        [HttpGet("{bookId}", Name = "GetBook")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(int bookId)
        {
            if (!bookRepository.IsBookIdExist(bookId))
                return NotFound();

            var book = bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                DatePublish = book.DatePublish
            };

            return Ok(bookDto);
        }


        [HttpGet("ISBN/{isbn}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(string isbn)
        {
            if (!bookRepository.IsBookExist(isbn))
                return NotFound();

            var book = bookRepository.GetBook(isbn);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                DatePublish = book.DatePublish
            };

            return Ok(bookDto);
        }


        [HttpGet("{bookId}/rating")]
        public IActionResult GetBookRating(int bookId)
        {
            if (!bookRepository.IsBookIdExist(bookId))
                return NotFound();

            decimal rating = bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);
        }
        //[HttpGet("reviews/{reviewId}/book")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(BookDto))]
        //public IActionResult GetBookFromReviews(int reviewId)
        //{
        //    var book = bookRepository.GetBookFromReviews(reviewId);

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var bookDto = new BookDto()
        //    {
        //        Id = book.Id,
        //        Isbn = book.Isbn,
        //        Title = book.Title,
        //        DatePublish = book.DatePublish
        //    };

        //    return Ok(bookDto);
        //}

        //[HttpGet("{bookId}/reviews")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        //public IActionResult GetReviewsFromBook(int bookId)
        //{

        //    if (!bookRepository.IsBookIdExist(bookId))
        //        return NotFound();

        //    var reviews = bookRepository.GetReviewsFromBook(bookId).ToList();

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var reviewDtos = new List<ReviewDto>();

        //    foreach (var review in reviews)
        //    {
        //        reviewDtos.Add(new ReviewDto
        //        {
        //            Id = review.Id,
        //            HeadLine = review.HeadLine,
        //            Rating = review.Rating,
        //            ReviewText = review.ReviewText
        //        });
        //    }

        //    return Ok(reviewDtos);
        //}

        //[HttpGet("authors/{authorId}/books")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        //public IActionResult GetBooksFromAuthor(int authorId)
        //{

        //    var books = bookRepository.GetBooksFromAuthor(authorId).ToList();

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var bookDtos = new List<BookDto>();

        //    foreach (var book in books)
        //    {
        //        bookDtos.Add(new BookDto
        //        {
        //            Id = book.Id,
        //            Isbn = book.Isbn,
        //            Title = book.Title,
        //            DatePublish = book.DatePublish
        //        });
        //    }

        //    return Ok(bookDtos);
        //}

        //[HttpGet("{bookId}/authors")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        //public IActionResult GetAuthorsFromBook(int bookId)
        //{

        //    if (!bookRepository.IsBookIdExist(bookId))
        //        return NotFound();

        //    var authors = bookRepository.GetAuthorsFromBook(bookId).ToList();

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var authorDtos = new List<AuthorDto>();

        //    foreach (var author in authors)
        //    {
        //        authorDtos.Add(new AuthorDto
        //        {
        //            Id = author.Id,
        //            FirstName = author.FirstName,
        //            LastName = author.LastName
        //        });
        //    }

        //    return Ok(authorDtos);
        //}

        //[HttpGet("category/{categoryId}/books")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        //public IActionResult GetBooksFromCategory(int categoryId)
        //{

        //    var books = bookRepository.GetBooksFromCategory(categoryId).ToList();

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var bookDtos = new List<BookDto>();

        //    foreach (var book in books)
        //    {
        //        bookDtos.Add(new BookDto
        //        {
        //            Id = book.Id,
        //            Isbn = book.Isbn,
        //            Title = book.Title,
        //            DatePublish = book.DatePublish
        //        });
        //    }

        //    return Ok(bookDtos);
        //}

        //[HttpGet("{bookId}/categories")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        //public IActionResult GetCategoriesFromBook(int bookId)
        //{

        //    if (!bookRepository.IsBookIdExist(bookId))
        //        return NotFound();

        //    var categories = bookRepository.GetCategoriesFromBook(bookId).ToList();

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var categoriesDtos = new List<CategoryDto>();

        //    foreach (var category in categories)
        //    {
        //        categoriesDtos.Add(new CategoryDto
        //        {
        //            Id = category.Id,
        //            Name = category.Name
        //        });
        //    }

        //    return Ok(categoriesDtos);
        //}

        private StatusCodeResult ValidateBook(List<int> authId, List<int> catId, Book book)
        {
            if(book == null || authId.Count() <= 0 || catId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing book, author, or category");
                return BadRequest();
            }

            if(bookRepository.IsDuplicateIsbn(book.Id, book.Isbn))
            {
                ModelState.AddModelError("", "Duplicate ISBN");
                return StatusCode(422);
            }

            foreach (var id in authId)
            {
                if(!authorRepository.IsAuthorIdExist(id))
                {
                    ModelState.AddModelError("", "Author not found");
                    return StatusCode(404);
                }
            }

            foreach (var id in catId)
            {
                if(!categoryRepository.CategoryExist(id))
                {
                    ModelState.AddModelError("", "Category not found");
                    return StatusCode(404);
                }
            }

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateBook([FromQuery] List<int> authId, [FromQuery] List<int> catId,
                                        [FromBody] Book bookToCreate)
        {
            var statusCode = ValidateBook(authId, catId, bookToCreate);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!bookRepository.CreateBook(authId, catId, bookToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving the book " +
                                            $"{bookToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetBook", new { bookId = bookToCreate.Id }, bookToCreate);
        }


        [HttpPut("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateBook(int bookId, [FromQuery] List<int> authId, [FromQuery] List<int> catId,
                                        [FromBody] Book bookToUpdate)
        {
            var statusCode = ValidateBook(authId, catId, bookToUpdate);

            if (bookId != bookToUpdate.Id)
                return BadRequest();

            if (!bookRepository.IsBookIdExist(bookId))
                return NotFound();

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!bookRepository.UpdateBook(authId, catId, bookToUpdate))
            {
                ModelState.AddModelError("", $"Something went wrong updating the book " +
                                            $"{bookToUpdate.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/books/bookId
        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteBook(int bookId)
        {
            if (!bookRepository.IsBookIdExist(bookId))
                return NotFound();

            var reviewsToDelete = reviewRepository.GetReviewsFromBook(bookId);
            var bookToDelete = bookRepository.GetBook(bookId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleting reviews");
                return StatusCode(500, ModelState);
            }

            if (!bookRepository.RemoveBook(bookToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting book {bookToDelete.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}