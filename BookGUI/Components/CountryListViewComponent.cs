using System.Linq;
using BookGUI.Models;
using BookGUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookGUI.Components
{
    public class CountryListViewComponent : ViewComponent
    {
        ICountryRepositoryGUI countryRepositoryGUI;
        
        public CountryListViewComponent(ICountryRepositoryGUI _countryRepository)
        {
            countryRepositoryGUI = _countryRepository;
        }

        public IViewComponentResult Invoke()
        {
            var countries = countryRepositoryGUI.GetCountries().OrderBy(x => x.Name).Select(x => new {Id = x.Id, Value = x.Name});

            var countryList = new CountrySelectList
            {
                CountryList = new SelectList(countries, "Id", "Value")
            };

            return View("_CountryList", countryList);
        }

    }
}