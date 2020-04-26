using System.Collections.Generic;
using BookApi.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookGUI.Components
{
    public class CategoriesList
    {
        private List<CategoryDto> allCategories = new List<CategoryDto>();

        public CategoriesList(List<CategoryDto> _allCategories)
        {
            allCategories = _allCategories;
        }

        public List<SelectListItem> GetCategories()
        {
            var items = new List<SelectListItem>();

            foreach (var category in allCategories)
            {
                items.Add(new SelectListItem{
                    Text = category.Name,
                    Value = category.Id.ToString(),
                    Selected = false
                });
            }

            return items;
        }

        public List<SelectListItem> GetCategories(List<int> selectedCategories)
        {
            var items = new List<SelectListItem>();

            foreach (var category in allCategories)
            {
                items.Add(new SelectListItem{
                    Text = category.Name,
                    Value = category.Id.ToString(),
                    Selected = selectedCategories.Contains(category.Id) ? true : false
                });
            }

            return items;
        }
    }
}