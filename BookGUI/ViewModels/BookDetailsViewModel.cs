using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.ViewModels
{
    public class BookDetailsViewModel
    {
        public BookDto Book { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
        public IDictionary<AuthorDto, CountryDto> AuthorsCountry { get; set; }
        public IDictionary<ReviewDto, ReviewerDto> ReviewsReviewer { get; set; }

        public decimal Rating { get; set; }
    }
}