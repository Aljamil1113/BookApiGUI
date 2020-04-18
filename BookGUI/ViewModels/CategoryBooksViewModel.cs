using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.ViewModels
{
    public class CategoryBooksViewModel
    {
        public CategoryDto Category {get; set; }
        public IEnumerable<BookDto> Books { get; set; }
    }
}