using System.Collections.Generic;
using BookApi.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookGUI.ViewModels
{
    public class CreateUpdateBookViewModel
    {
        public BookDto Book { get; set; }

        public List<int> AuthorIds { get; set; }
        public List<SelectListItem> AuthorSelectListItems { get; set; }

        public List<int> CategoryIds { get; set; }
        public List<SelectListItem> CategorySelectListItems { get; set; }
    }
}