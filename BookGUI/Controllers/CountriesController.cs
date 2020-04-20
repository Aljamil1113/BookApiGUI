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
    public class CountriesController : Controller
    {
        ICountryRepositoryGUI countryRepository;

        public CountriesController(ICountryRepositoryGUI _countryRepository)
        {
            countryRepository = _countryRepository;
        }
        public IActionResult Index()
        {
            var countries = countryRepository.GetCountries();

            if(countries.Count() <= 0)
            {
                ViewBag.Message = "There are problems retrieving countries " + 
                " or countries not exist in the database.";
            }
            return View(countries);
        }

        public IActionResult Detail(int countryId)
        {
            var country = countryRepository.GetCountryById(countryId);
           
            if(country == null)
            {
                ModelState.AddModelError("", "Error getting a country");
                ViewBag.Message = $"There was a problem retrieving country id {countryId}" +
                $" from the database or no country with id exists."; 

                country = new CountryDto();
            }

            var authors = countryRepository.GetAuthorFromCountry(countryId);

            if(authors.Count() <= 0)
            {
                ViewBag.AuthorMessage = $"There was a problem retrieving authors in country id {countryId}" +
                $" from the database or no authors with country id exists."; 
            }

            CountryAuthorsViewModel countryAuthors = new CountryAuthorsViewModel()
            {
                Country = country,
                Authors = authors
            };

            return View(countryAuthors);
        }
    }
}