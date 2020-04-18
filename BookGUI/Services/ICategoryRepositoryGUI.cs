using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public interface ICategoryRepositoryGUI
    {
         IEnumerable<CategoryDto> GetCategories();
         CategoryDto GetCategory(int categoryId);
         IEnumerable<BookDto> GetAllBooksPerCategory(int categoryId);
         IEnumerable<CategoryDto> GetCategoriesFromBook(int bookId);
    }
}