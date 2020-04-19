using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Dtos;
using BookGUI.Services;
using BookGUI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookGUI.Controllers
{
    public class CategoriesController : Controller
    {
        ICategoryRepositoryGUI categoriesRepository;

        public CategoriesController(ICategoryRepositoryGUI _categories)
        {
            categoriesRepository = _categories;
        }

        public IActionResult Index()
        {
            var categories = categoriesRepository.GetCategories();

            if(categories.Count() <= 0)
            {
                ViewBag.Message = "There are problems retrieving categories " +
                "or categories does not exist in the database";
            }

            return View(categories);
        }
        
        public IActionResult Detail(int categoryId)
        {
            var category = categoriesRepository.GetCategory(categoryId);

            if(category == null)
            {
                ModelState.AddModelError("", "Error getting a country");
                ViewBag.Message = $"There was a problem retrieving country id {categoryId}" +
                $" from the database or no country with id {categoryId} exists."; 

                category = new CategoryDto();
            }

            var books = categoriesRepository.GetAllBooksPerCategory(categoryId);

            if(books.Count() <= 0)
            {
                ViewBag.BookMessage = $"{category.Name} has no books.";
            }

            var categoryViewModel = new CategoryBooksViewModel()
            {
                Category = category,
                Books = books
            };

            return View(categoryViewModel);
        }
    }
}