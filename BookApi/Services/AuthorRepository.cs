using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Model;

namespace BookApi.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private BookDbContext authorRepository;

        public AuthorRepository(BookDbContext _authorRepository)
        {
            authorRepository = _authorRepository;
        }

        public bool CreateAuthor(Author author)
        {
            authorRepository.Add(author);
            return SaveAuthor();
        }

        public bool DeleteAuthor(Author author)
        {
            authorRepository.Remove(author);
            return SaveAuthor();
        }

        public Author GetAuthor(int authorId)
        {
            return authorRepository.Authors.Where(a => a.Id == authorId).FirstOrDefault();
        }

        public ICollection<Author> GetAuthors()
        {
            return authorRepository.Authors.OrderBy(a => a.LastName).ToList();
        }

        public ICollection<Author> GetAuthorsFromBook(int bookId)
        {
            return authorRepository.BookAuthors.Where(b => b.BookId == bookId).Select(a => a.Author).ToList();
        }

        //public ICollection<Author> GetAuthorsFromCountry(int countryId)
        //{
        //    return authorRepository.Countries.Where(c => c.Id == countryId).SelectMany(a => a.Authors).ToList();
        //}

        public ICollection<Book> GetBooksFromAuthor(int authorId)
        {
            return authorRepository.BookAuthors.Where(a => a.AuthorId == authorId).Select(b => b.Book).ToList();
        }

        //public Country GetCountryFromAuthor(int authorId)
        //{
        //    return authorRepository.Authors.Where(a => a.Id == authorId).Select(c => c.Country).FirstOrDefault();
        //}

        public bool IsAuthorIdExist(int authorId)
        {
            return authorRepository.Authors.Any(a => a.Id == authorId);
        }

        public bool SaveAuthor()
        {
            var author = authorRepository.SaveChanges();
            return author >= 0 ? true : false;
        }

        public bool UpdateAuthor(Author author)
        {
            authorRepository.Update(author);
            return SaveAuthor();
        }
    }
}
