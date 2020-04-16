using BookApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi.Services
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        bool IsBookIdExist(int bookId);

        Book GetBook(string bookIsbn);
        bool IsBookExist(string bookIsbn);
        decimal GetBookRating(int bookId);
        bool IsDuplicateIsbn(int bookId, string bookIsbn);

        //Book GetBookFromReviews(int reviewId);
        //ICollection<Review> GetReviewsFromBook(int bookId);

        //ICollection<Book> GetBooksFromAuthor(int authorId);
        //ICollection<Author> GetAuthorsFromBook(int bookId);

        //ICollection<Book> GetBooksFromCategory(int categoryId);
        //ICollection<Category> GetCategoriesFromBook(int bookId);

        bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool UpdateBook(List<int> authorsId, List<int> categoriesId,Book book);
        bool RemoveBook(Book book);
        bool SaveBook();
    }
}
