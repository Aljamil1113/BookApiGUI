using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Dtos;
using BookApi.Model;
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

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category addCategory)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PostAsJsonAsync("categories", addCategory);
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    var newCategoryTask = result.Content.ReadAsAsync<Category>();
                    newCategoryTask.Wait();

                    var newCategory = newCategoryTask.Result;
                    TempData["SuccessMessage"] = $"Category {newCategory.Name} was successfully created.";
                
                    return RedirectToAction("Detail", new { categoryId = newCategory.Id });
                }

                if((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("", "Category already exists.");
                }

                else
                {
                    ModelState.AddModelError("", "Some kind of error. Category not created!");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int categoryId)
        {
            var categoryToUpdate = categoriesRepository.GetCategory(categoryId);

            if(categoryToUpdate == null)
            {
                ModelState.AddModelError("", "Error getting a category");
                categoryToUpdate = new CategoryDto();
            }
            return View(categoryToUpdate);
        }

        [HttpPost]
        public IActionResult Edit(Category categoryToUpdate)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.PutAsJsonAsync($"categories/{categoryToUpdate.Id}", categoryToUpdate);
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Category was successfully updated.";

                    return RedirectToAction("Detail", new { categoryId = categoryToUpdate.Id});
                }

                if((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("", "Category already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Category not created!");
                }
            }
            var categoryDto = categoriesRepository.GetCategory(categoryToUpdate.Id);
            return View(categoryDto);
        }

        [HttpGet]
        public IActionResult Delete(int categoryId)
        {
            var categoryToDelete = categoriesRepository.GetCategory(categoryId);

            if(categoryToDelete == null)
            {
                ModelState.AddModelError("", "Error getting a category");
                categoryToDelete = new CategoryDto();
            }

            return View(categoryToDelete);
        }

        [HttpPost]
        public IActionResult Delete(int categoryId, string categoryName)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
                var responseTask = client.DeleteAsync($"categories/{categoryId}");
                responseTask.Wait();

                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Category {categoryName} was successfully deleted.";

                    return RedirectToAction("Index","Categories");
                }

                if((int)result.StatusCode == 409)
                {
                    ModelState.AddModelError("", $"Category {categoryName} cannot be deleted because " +
                    $"it is used by at least one book.");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Category not created!");
                }
            }
            var categoryDto = categoriesRepository.GetCategory(categoryId);
            return View(categoryDto);
        }
    }
}