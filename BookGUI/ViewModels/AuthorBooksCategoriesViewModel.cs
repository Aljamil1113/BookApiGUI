using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.ViewModels
{
    public class AuthorBooksCategoriesViewModel
    {
        public AuthorDto Author { get; set; }
        public IDictionary<BookDto, IEnumerable<CategoryDto>> BookCategories { get; set; }
    }
}