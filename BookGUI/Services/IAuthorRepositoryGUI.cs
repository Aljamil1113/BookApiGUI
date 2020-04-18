using System.Collections;
using System.Collections.Generic;
using BookApi.Dtos;

namespace BookGUI.Services
{
    public interface IAuthorRepositoryGUI
    {
         IEnumerable<AuthorDto> GetAuthors();
         AuthorDto GetAuthor(int authorId);
         IEnumerable<BookDto> GetBooksFromAuthor(int authorId);
         IEnumerable<AuthorDto> GetAllAuthorsFromBook(int bookId);
    }
}