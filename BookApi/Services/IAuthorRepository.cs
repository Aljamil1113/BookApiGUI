using BookApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi.Services
{
    public interface IAuthorRepository
    {
        ICollection<Author> GetAuthors();
        Author GetAuthor(int authorId);
        bool IsAuthorIdExist(int authorId);

        //Country GetCountryFromAuthor(int authorId);
        //ICollection<Author> GetAuthorsFromCountry(int countryId);

        ICollection<Author> GetAuthorsFromBook(int bookId);
        ICollection<Book> GetBooksFromAuthor(int authorId);

        bool CreateAuthor(Author author);
        bool UpdateAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool SaveAuthor();
    }
}
