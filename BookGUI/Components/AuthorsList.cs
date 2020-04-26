using System.Collections.Generic;
using BookApi.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookGUI.Components
{
    public class AuthorsList
    {
        private List<AuthorDto> _allAuthors = new List<AuthorDto>();

        public AuthorsList(List<AuthorDto> allAuthors)
        {
            _allAuthors = allAuthors;
        }

        public List<SelectListItem> GetAuthorsList()
        {
            var items = new List<SelectListItem>();

            foreach (var author in _allAuthors)
            {
                items.Add(new SelectListItem {
                    Text = author.FirstName + " " + author.LastName,
                    Value = author.Id.ToString(),
                    Selected = false
                });
            }

            return items;
        }

        public List<SelectListItem> GetAuthorsList(List<int> selectedAuthors)
        {
            var items = new List<SelectListItem>();

            foreach (var author in _allAuthors)
            {
                items.Add(new SelectListItem {
                    Text = author.FirstName + " " + author.LastName,
                    Value = author.Id.ToString(),
                    Selected = selectedAuthors.Contains(author.Id) ? true : false
                });
            }

            return items;
        }
    }
}