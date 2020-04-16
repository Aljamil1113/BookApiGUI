using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Model;

namespace BookApi.Services
{
    public class CountryRepository : ICountryRepository
    {
        private BookDbContext countryContext;

        public CountryRepository(BookDbContext _countryContext)
        {
            countryContext = _countryContext;
        }

        public bool CountryExist(int countryId)
        {
            return countryContext.Countries.Any(c => c.Id == countryId);
        }

        public bool CreateCountry(Country country)
        {
            countryContext.Add(country);

            return SaveCountry();
        }

        public bool DeleteCountry(Country country)
        {
            countryContext.Remove(country);

            return SaveCountry();
        }

        public ICollection<Author> GetAuthorsFromCountry(int countryId)
        {
            return countryContext.Authors.Where(c => c.Id == countryId).ToList();
        }

        public ICollection<Country> GetCountries()
        {
            return countryContext.Countries.OrderBy(c => c.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            return countryContext.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }

        public Country GetCountryOfAnAuthor(int authorId)
        {
            return countryContext.Authors.Where(a => a.Id == authorId).Select(c => c.Country).FirstOrDefault();
        }

        public bool IsDuplicateCountry(int countryId, string name)
        {
            var country = countryContext.Countries.Where(c => c.Name.Trim().ToUpper() == name.Trim().ToUpper() 
            && c.Id != countryId).FirstOrDefault();

            return country == null ? false : true;
        }

        public bool SaveCountry()
        {
            var c = countryContext.SaveChanges();
            return c >= 0 ? true : false;
        }

        public bool UpdateCountry(Country country)
        {
            countryContext.Update(country);
            return SaveCountry();
        }
    }
}
