using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookGUI.Models
{
    public class ReviewerSelectList
    {
        public int ReviewerId { get; set; }
        public SelectList ReviewersList { get; set; }
    }
}