using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public interface ICountryRepositoryGUI
    {
        IEnumerable<CountryDto> GetCountries();
        CountryDto GetCountryById(int countryId);
        CountryDto GetCountryOfAnAuthor(int authorId);
        IEnumerable<AuthorDto> GetAuthorFromCountry(int countryId);
    }
}