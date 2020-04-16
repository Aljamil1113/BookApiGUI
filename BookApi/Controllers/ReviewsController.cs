using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Dtos;
using BookApi.Model;
using BookApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private IReviewRepository reviewRepository { get; set; }
        private IBookRepository bookRepository { get; set; }
        private IReviewerRepository reviewerRepository { get; set; }
        public ReviewsController(IReviewRepository _reviewRepository, IReviewerRepository _reviewerRepository, IBookRepository _bookRepository)
        {
            reviewRepository = _reviewRepository;
            bookRepository = _bookRepository;
            reviewerRepository = _reviewerRepository;
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = reviewRepository.GetReviews().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDtos = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewDtos.Add(new ReviewDto
                {
                    Id = review.Id,
                    HeadLine = review.HeadLine,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                });
            }

            return Ok(reviewDtos);
        }

        [HttpGet("{reviewId}", Name = "GetReview")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReview(int reviewId)
        {
            if (!reviewRepository.ReviewIdExist(reviewId))
                return NotFound();

            var review = reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDto = new ReviewDto()
            {
                Id = review.Id,
                HeadLine = review.HeadLine,
                Rating = review.Rating,
                ReviewText = review.ReviewText
            };

            return Ok(reviewDto);
        }

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsFromBook(int bookId)
        {
            if (!bookRepository.IsBookIdExist(bookId))
                return NotFound();
            
            var reviews = reviewRepository.GetReviewsFromBook(bookId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDtos = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewDtos.Add(new ReviewDto
                {
                    Id = review.Id,
                    HeadLine = review.HeadLine,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                });
            }

            return Ok(reviewDtos);
        }


        //[HttpGet("reviewers/{reviewerId}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        //public IActionResult GetReviewsFromReviewer(int reviewerId)
        //{

        //    var reviews = reviewRepository.GetReviewsFromReviewer(reviewerId).ToList();

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var reviewDtos = new List<ReviewDto>();

        //    foreach (var review in reviews)
        //    {
        //        reviewDtos.Add(new ReviewDto
        //        {
        //            Id = review.Id,
        //            HeadLine = review.HeadLine,
        //            Rating = review.Rating,
        //            ReviewText = review.ReviewText
        //        });
        //    }

        //    return Ok(reviewDtos);
        //}


        //Book
        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBookFromReview(int reviewId)
        {
            if(!reviewRepository.ReviewIdExist(reviewId))
                return Ok();

            var book = reviewRepository.GetBookFromReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Isbn = book.Isbn,
                DatePublish = book.DatePublish,
                Title = book.Title
            };

            return Ok(bookDto);
        }

        //Reviewer
        //[HttpGet("{reviewId}/reviewer")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200, Type = typeof(BookDto))]
        //public IActionResult GetReviewerFromReviews(int reviewId)
        //{
        //    if (!reviewRepository.ReviewIdExist(reviewId))
        //        return NotFound();

        //    var reviewer = reviewRepository.GetReviewerFromReview(reviewId);

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var reviewerDto = new ReviewerDto()
        //    {
        //        Id = reviewer.Id,
        //        FirstName = reviewer.FirstName,
        //        LastName = reviewer.LastName
        //    };
        //    return Ok(reviewerDto);
        //}

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody]Review addReview)
        {
            if (addReview == null)
                return BadRequest(ModelState);

            if (!bookRepository.IsBookIdExist(addReview.Book.Id))
                ModelState.AddModelError("", $"Book doesn't exist.");

            if (!reviewerRepository.IsReviewerIdExist(addReview.Reviewer.Id))
                ModelState.AddModelError("", $"Reviwer doesn't exist.");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            addReview.Book = bookRepository.GetBook(addReview.Book.Id);
            addReview.Reviewer = reviewerRepository.GetReviewer(addReview.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewRepository.CreateReview(addReview))
            {
                ModelState.AddModelError("", $"Something wrong creating the review");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReview", new { reviewId = addReview.Id }, addReview);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReview(int id, [FromBody]Review updateReview)
        {
            if (updateReview == null)
                return BadRequest(ModelState);

            if (id != updateReview.Id)
                return BadRequest(ModelState);

            if (!reviewRepository.ReviewIdExist(id))
                ModelState.AddModelError("", $"Review doesn't exists");

            if (!bookRepository.IsBookIdExist(updateReview.Book.Id))
                ModelState.AddModelError("", $"Book doesn't exist.");

            if (!reviewerRepository.IsReviewerIdExist(updateReview.Reviewer.Id))
                ModelState.AddModelError("", $"Reviewer doesn't exist.");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            updateReview.Book = bookRepository.GetBook(updateReview.Book.Id);
            updateReview.Reviewer = reviewerRepository.GetReviewer(updateReview.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewRepository.UpdateReview(updateReview))
            {
                ModelState.AddModelError("", $"Something wrong update the review");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!reviewRepository.ReviewIdExist(reviewId))
                return NotFound();

            var deleteReview = reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!reviewRepository.DeleteReview(deleteReview))
            {
                ModelState.AddModelError("", $"Something went wrong deleting review");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}