using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookInfoSpa.Controllers
{
    [Route("api/[controller]")]
    public class BookDataController : Controller
    {

        [HttpGet("[action]")]
        public IEnumerable<Book> BookList()
        {
            return new List<Book>()
            {
                new Book {Id = 1, Author = "W. Shakespeare", Title = "Romeo and Juliet", Year = 1596, Price = 12},
                new Book {Id = 2, Author = "I. Asimov", Title = "I Robot", Year = 1950, Price = 16},
                new Book {Id = 3, Author = "J. Tolkien", Title = "Lord of the rings", Year = 1955, Price = 12.5m},
                new Book {Id = 4, Author = "D. Brown", Title = "The da vinci code", Year = 2003, Price = 13}
            };
        }

        [HttpGet("[action]/{id}")]
        public IEnumerable<BookReview> Reviews(int id)
        {
            return _bookReviews.Where(x => x.BookId == id).ToList();
        }

        private IEnumerable<BookReview> _bookReviews = new List<BookReview>()
        {
            new BookReview { Id = 1, BookId = 1, Author = "Jon Doe", Description = "An extremely entertaining play by Shakespeare. The slapstick humour is refreshing!", Title = "Fantastic Play", Rating = 4 },
            new BookReview { Id = 2, BookId = 1, Author = "Mark White", Description = "Absolutely fun and entertaining. The play lacks thematic depth when compared to other plays by Shakespeare", Title = "Thrilling", Rating = 5 },
            new BookReview { Id = 3, BookId = 2, Author = "Mark White", Description = "It was fun to read Asimov's description of machine learning from the 40s/50s", Title = "Must Read", Rating = 4 },
            new BookReview { Id = 4, BookId = 2, Author = "Edward Johnson", Description = "It's a relatively short book and will keep you entertained the entire time", Title = "Classic SciFi", Rating = 5 },
            new BookReview { Id = 5, BookId = 3, Author = "Mark White", Description = "I felt that this book was beautiful, Tolkien’s imagination is legendary.", Title = "Legendary", Rating = 5 },
            new BookReview { Id = 6, BookId = 3, Author = "Joe Green", Description = "Tolkien really creates his world from below the ground up", Title = "Amazing", Rating = 5 },
            new BookReview { Id = 7, BookId = 4, Author = "Mark Thompson", Description = "A fanciful tale of secret societies, secret codes, and espionage that keeps you interested until the end", Title = "Fanciful Tale", Rating = 5 },
            new BookReview { Id = 8, BookId = 4, Author = "Henry Kyle", Description = "This book is suspenseful, thought provoking, and above all extremely entertaining", Title = "Great Read", Rating = 5 }
        };

    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
    }

    public class BookReview
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }

    }
}
