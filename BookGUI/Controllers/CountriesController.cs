using System.Net.Http;
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
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(countryAuthors);
        }

        [HttpGet]
        public IActionResult CreateCountry()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCountry(Country country)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PostAsJsonAsync("countries", country);
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    var newCountryTask = result.Content.ReadAsAsync<Country>();
                    newCountryTask.Wait();

                    var newCountry = newCountryTask.Result;
                    TempData["SuccessMessage"] = $"Country {newCountry.Name} was successfully created.";

                    return RedirectToAction("Detail", new { countryId = newCountry.Id});
                }

                if((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("", "Country already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Country not created!");
                }
            }

            return View();

        }

        [HttpGet]
        public IActionResult Edit(int countryId)
        {
            var countryToUpdate = countryRepository.GetCountryById(countryId);

            if(countryToUpdate == null)
            {
                ModelState.AddModelError("", "Error getting a country");
                countryToUpdate = new CountryDto();
            }

            return View(countryToUpdate);
        }

        [HttpPost]
        public IActionResult Edit(Country countryToUpdate)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PutAsJsonAsync($"countries/{countryToUpdate.Id}", countryToUpdate);
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Country was successfully updated.";

                    return RedirectToAction("Detail", new { countryId = countryToUpdate.Id});
                }

                if((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("", "Country already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Country not created!");
                }
            }
            var countryDto = countryRepository.GetCountryById(countryToUpdate.Id);
            return View(countryDto);
        }

        [HttpGet]
        public IActionResult Delete(int countryId)
        {
            var countryToDelete = countryRepository.GetCountryById(countryId);

            if(countryToDelete == null)
            {
                ModelState.AddModelError("", "Error getting a country");
                countryToDelete = new CountryDto();
            }

            return View(countryToDelete);
        }

        [HttpPost]
        public IActionResult Delete(int countryId, string countryName)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.DeleteAsync($"countries/{countryId}");
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Country {countryName} was successfully deleted.";

                    return RedirectToAction("Index","Countries");
                }

                if((int)result.StatusCode == 409)
                {
                    ModelState.AddModelError("", $"Country {countryName} cannot be deleted because " +
                    $"it is used by at least one author.");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Country not created!");
                }
            }
            var countryDto = countryRepository.GetCountryById(countryId);
            return View(countryDto);
        }
    }
}