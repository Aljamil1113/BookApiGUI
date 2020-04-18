using System;
using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public interface IBookRepositoryGUI
    {
         IEnumerable<BookDto> GetBooks();
         BookDto GetBookById(int bookId);
         BookDto GetBookByIsbn(string isbn);
         decimal BookRating(int bookId);
    }
}