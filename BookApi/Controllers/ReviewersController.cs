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
    public class ReviewersController : ControllerBase
    {
        private IReviewerRepository reviewerRepository { get; set; }
        private IReviewRepository reviewRepository { get; set; }

        public ReviewersController(IReviewerRepository _reviewerRepository, IReviewRepository _reviewRepository)
        {
            reviewerRepository = _reviewerRepository;
            reviewRepository = _reviewRepository;
        }

        // api/Reviewers
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            var reviewers = reviewerRepository.GetReviewers().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDtos = new List<ReviewerDto>();

            foreach (var reviewer in reviewers)
            {
                reviewerDtos.Add(new ReviewerDto
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName
                });
            }

            return Ok(reviewerDtos);
        }

        [HttpGet("{reviewerId}", Name = "GetReviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!reviewerRepository.IsReviewerIdExist(reviewerId))
                return NotFound();

            var reviewer = reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }


        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerFromReviews(int reviewId)
        {
            if (!reviewRepository.ReviewIdExist(reviewId))
                return NotFound();

            var reviewer = reviewerRepository.GetReviewerFromReviews(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

        //Reviews
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsFromReviewer(int reviewerId)
        {
            if (!reviewerRepository.IsReviewerIdExist(reviewerId))
                return NotFound();

            var reviews = reviewerRepository.GetReviewsFromReviewer(reviewerId).ToList();

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

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody]Reviewer addReviewer)
        {
            if (addReviewer == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!reviewerRepository.CreateReviwer(addReviewer))
            {
                ModelState.AddModelError("", $"Something wrong creating {addReviewer.LastName}, {addReviewer.FirstName} ...");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReviewer", new { reviewerId = addReviewer.Id }, addReviewer);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int id, [FromBody]Reviewer updateReviewer)
        {
            if (updateReviewer == null)
                return BadRequest(ModelState);

            if (id != updateReviewer.Id)
                return BadRequest(ModelState);

            if (!reviewerRepository.IsReviewerIdExist(id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewerRepository.UpdateReviewer(updateReviewer))
            {
                ModelState.AddModelError("", $"Something went wrong updating Reviewer...");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReviewer(int id)
        {
            if (!reviewerRepository.IsReviewerIdExist(id))
                return NotFound();

            var reviewerDelete = reviewerRepository.GetReviewer(id);
            var reviewsDelete = reviewerRepository.GetReviewsFromReviewer(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewerRepository.DeleteReviewer(reviewerDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {reviewerDelete.LastName}, {reviewerDelete.FirstName}");
                return StatusCode(500, ModelState);
            }

            if (!reviewRepository.DeleteReviews(reviewsDelete.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleting reviews by {reviewerDelete.LastName}, {reviewerDelete.FirstName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}