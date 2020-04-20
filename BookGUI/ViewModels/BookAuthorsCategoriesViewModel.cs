using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.ViewModels
{
    public class BookAuthorsCategoriesViewModel
    {
        public BookDto Book { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
        public IEnumerable<AuthorDto> Authors { get; set; }
        public decimal Rating { get; set; }
    }
}