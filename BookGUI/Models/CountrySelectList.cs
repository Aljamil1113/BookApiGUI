using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookGUI.Models
{
    public class CountrySelectList
    {
        public int countryId { get; set; }
        public SelectList CountryList { get; set; }
    }
}