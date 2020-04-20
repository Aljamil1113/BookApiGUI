using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.ViewModels
{
    public class CountryAuthorsViewModel
    {
        public CountryDto Country { get; set; }
        public IEnumerable<AuthorDto> Authors { get; set; }
    }
}