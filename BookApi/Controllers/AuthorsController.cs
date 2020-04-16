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
    public class AuthorsController : Controller
    {
        private IAuthorRepository authorRepository { get; set; }
        private IBookRepository bookRepository { get; set; }
        public ICountryRepository countryRepository { get; set; }

        public AuthorsController(IAuthorRepository _authorRepository, IBookRepository _bookRepository, ICountryRepository _countryRepository)
        {
            authorRepository = _authorRepository;
            bookRepository = _bookRepository;
            countryRepository = _countryRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        //api/authors

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthors()
        {
            var authors = authorRepository.GetAuthors().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDtos = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorDtos.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorDtos);
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        public IActionResult GetAuthor(int authorId)
        {
            if (!authorRepository.IsAuthorIdExist(authorId))
                return NotFound();

            var author = authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDto = new AuthorDto()
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };

            return Ok(authorDto);
        }

        //[HttpGet("country/{countryId}/authors")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        //public IActionResult GetAuthorsFromCountry(int countryId)
        //{

        //    var authors = authorRepository.GetAuthorsFromCountry(countryId).ToList();

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

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorFromBooks(int bookId)
        {
            if (!bookRepository.IsBookIdExist(bookId))
                return NotFound();

            var authors = authorRepository.GetAuthorsFromBook(bookId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDtos = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorDtos.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorDtos);
        }

        //[HttpGet("{authorId}/country")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(CountryDto))]
        //public IActionResult GetCountryFromAuthor(int authorId)
        //{
        //    if (!authorRepository.IsAuthorIdExist(authorId))
        //        return NotFound();

        //    var country = authorRepository.GetCountryFromAuthor(authorId);

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var countryDto = new CountryDto()
        //    {
        //        Id = country.Id,
        //        Name = country.Name
        //    };

            

        //    return Ok(countryDto);
        //}

        [HttpGet("{authorId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksFromAuthor(int authorId)
        {
            if (!authorRepository.IsAuthorIdExist(authorId))
                return NotFound();

            var books = authorRepository.GetBooksFromAuthor(authorId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                bookDtos.Add(new BookDto
                {
                    Id = book.Id,
                    Isbn = book.Isbn,
                    DatePublish = book.DatePublish,
                    Title = book.Title
                });
            }

            return Ok(bookDtos);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Author))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody]Author authorToCreate)
        {
            if (authorToCreate == null)
                return BadRequest();

            if (!countryRepository.CountryExist(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", "Country doesn't exist!");
                return StatusCode(404, ModelState);
            }

            authorToCreate.Country = countryRepository.GetCountry(authorToCreate.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving the author " +
                                            $"{authorToCreate.FirstName} {authorToCreate.LastName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }

     
        [HttpPut("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateAuthor(int authorId, [FromBody]Author authorToUpdate)
        {
            if (authorToUpdate == null)
                return BadRequest(ModelState);

            if (authorId != authorToUpdate.Id)
                return BadRequest(ModelState);

            if (!authorRepository.IsAuthorIdExist(authorId))
                ModelState.AddModelError("", "Author doesn't exist!");

            if (!countryRepository.CountryExist(authorToUpdate.Country.Id))
                ModelState.AddModelError("", "Country doesn't exist!");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            authorToUpdate.Country = countryRepository.GetCountry(authorToUpdate.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!authorRepository.UpdateAuthor(authorToUpdate))
            {
                ModelState.AddModelError("", $"Something went wrong updating the author " +
                                            $"{authorToUpdate.FirstName} {authorToUpdate.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthor(int authorId)
        {
            if (!authorRepository.IsAuthorIdExist(authorId))
                return NotFound();

            var authorToDelete = authorRepository.GetAuthor(authorId);

            if (authorRepository.GetBooksFromAuthor(authorId).Count() > 0)
            {
                ModelState.AddModelError("", $"Author {authorToDelete.FirstName} {authorToDelete.LastName} " +
                                              "cannot be deleted because it is associated with at least one book");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting " +
                                            $"{authorToDelete.FirstName} {authorToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}