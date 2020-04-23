using System.Linq;
using BookGUI.Models;
using BookGUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookGUI.Components
{
    public class ReviewersListViewComponent : ViewComponent
    {
        IReviewerRepositoryGUI reviewerRepositoryGUI;
        public ReviewersListViewComponent(IReviewerRepositoryGUI _reviewerRepositoryGUI)
        {
            reviewerRepositoryGUI = _reviewerRepositoryGUI;
        }

        public IViewComponentResult Invoke()
        {
            var reviewers = reviewerRepositoryGUI.GetReviewers().OrderBy(r => r.LastName)
            .Select(x => new {Id = x.Id, Value = x.FirstName + " " + x.LastName});

            var reviewersList = new ReviewerSelectList
            {
                ReviewersList = new SelectList(reviewers, "Id", "Value")
            };

            return View("_ReviewersList", reviewersList);
        }
    }
}