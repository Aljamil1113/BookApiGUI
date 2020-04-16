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
    public class CountriesController : Controller
    {
        private ICountryRepository countryRepository { get; set; }
        private IAuthorRepository authorRepository { get; set; }

        public CountriesController(ICountryRepository _countryRepository, IAuthorRepository _authorRepository)
        {
            countryRepository = _countryRepository;
            authorRepository = _authorRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        // api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = countryRepository.GetCountries().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new List<CountryDto>();

            foreach(var country in countries)
            {
                countryDto.Add(new CountryDto
                    {
                    Id = country.Id,
                    Name = country.Name
                });
            }

            return Ok(countryDto);
        }

        // api/countries/countryId
        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountry(int countryId)
        {
            if (!countryRepository.CountryExist(countryId))
                return NotFound();

            var country = countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAuthor(int authorId)
        {
            //Validate author exists
            if (!authorRepository.IsAuthorIdExist(authorId))
                return NotFound();

            var country = countryRepository.GetCountryOfAnAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }


        //authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsFromCountry(int countryId)
        {
            if (!countryRepository.CountryExist(countryId))
                return NotFound();

            var authors = countryRepository.GetAuthorsFromCountry(countryId).ToList();

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

        //POST api/countries
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody]Country countryToCreate)
        {
            if (countryToCreate == null)
                return BadRequest(ModelState);

            var country = countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryToCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if(country != null)
            {
                ModelState.AddModelError("", $"Country {countryToCreate.Name} already exists...");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!countryRepository.CreateCountry(countryToCreate))
            {
                ModelState.AddModelError("", $"Something wrong on saving {countryToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCountry", new { countryId = countryToCreate.Id }, countryToCreate);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int id, [FromBody]Country updateCountry)
        {
            if (updateCountry == null)
                return BadRequest(ModelState);

            if (id != updateCountry.Id)
                return BadRequest(ModelState);           

            if (!countryRepository.CountryExist(id))
                return NotFound();

            if (countryRepository.IsDuplicateCountry(id, updateCountry.Name))
            {
                ModelState.AddModelError("", $"Country {updateCountry.Name} already exists...");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest();

            if (!countryRepository.UpdateCountry(updateCountry))
            {
                ModelState.AddModelError("", $"Something wrong on updating {updateCountry.Name}");
                return StatusCode(500, ModelState);
            }
               
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCountry(int id)
        {
            if (!countryRepository.CountryExist(id))
                return NotFound();

            var countryToDelete = countryRepository.GetCountry(id);

            if(countryRepository.GetAuthorsFromCountry(id).Count() > 0)
            {
                ModelState.AddModelError("", $"Country {countryToDelete.Name} is" +
                    $" already use by some authors");

                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {countryToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}