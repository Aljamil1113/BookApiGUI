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
    public class CategoriesController : Controller
    {
        private ICategoryRepository categoryRepository { get; set; }
        public IBookRepository bookRepository { get; set; }

        public CategoriesController(ICategoryRepository _categoryRepository, IBookRepository _bookRepository)
        {
            categoryRepository = _categoryRepository;
            bookRepository = _bookRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        // api/categories

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = categoryRepository.GetCategories().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoryDtos.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(categoryDtos);
        }

        [HttpGet("{categoryId}", Name = "GetCategory")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategory(int categoryId)
        {
            if (!categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var category = categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDto = new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }


        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategoriesOfABook(int bookId)
        {
            if (!bookRepository.IsBookIdExist(bookId))
                return NotFound();

            var categories = categoryRepository.GetCategoriesOfABook(bookId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoryDtos.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(categoryDtos);
        }

        //BOOKS

        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetAllBooksCategory(int categoryId)
        {
            if (!categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var books = categoryRepository.GetAllBooksForCategory(categoryId).ToList();

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

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody]Category addCategory)
        {
            if (addCategory == null)
                return BadRequest(ModelState);

            var category = categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == addCategory.Name.Trim().ToUpper()).FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", $"Category {addCategory.Name} already exits ...");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!categoryRepository.CreateCategory(addCategory))
            {
                ModelState.AddModelError("", $"Something wrong creating {addCategory.Name} ...");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { categoryId = addCategory.Id }, addCategory);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int id, [FromBody]Category updateCategory)
        {
            if (updateCategory == null)
                return BadRequest(ModelState);

            if (id != updateCategory.Id)
                return BadRequest(ModelState);

            if (!categoryRepository.CategoryExist(id))
                return NotFound();

            if(categoryRepository.IsDuplicateCategory(id, updateCategory.Name))
            {
                ModelState.AddModelError("", $"Category {updateCategory.Name} is already existing...");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!categoryRepository.UpdateCategory(updateCategory))
            {
                ModelState.AddModelError("", $"Something went wrong updating {updateCategory.Name}...");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCategory(int id)
        {
            if (!categoryRepository.CategoryExist(id))
                return NotFound();

            var categoryDelete = categoryRepository.GetCategory(id);

            if(categoryRepository.GetAllBooksForCategory(id).Count() > 0)
            {
                ModelState.AddModelError("", $"Category {categoryDelete.Name} could not be deleted because " +
                    $"it already used by some books");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!categoryRepository.DeleteCategory(categoryDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {categoryDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}