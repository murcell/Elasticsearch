using Elastiicsearch.Web.Models;
using Elastiicsearch.Web.Services;
using Elastiicsearch.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Elastiicsearch.Web.Controllers
{
    public class BlogController : Controller
    {
        private BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        public IActionResult Save()
        {
            return View();
        }

        public async Task<IActionResult> Search()
        {
            return View(await _blogService.SearchAsync(string.Empty));
        }

        [HttpPost]
		public async Task<IActionResult> Search(string searchText)
		{
            ViewBag.searchText = searchText;

            return View(await _blogService.SearchAsync(searchText));
        }

		[HttpPost]
        public async Task<IActionResult> Save(BlogCreateViewModel model)
        {
            var isSuccess = await _blogService.SaveAsync(model);
            if(!isSuccess)
            {
                TempData["result"] = "Kayıt işlemi başarısız";
                return RedirectToAction(nameof(BlogController.Save));
            }
            TempData["result"] = "Kayıt işlemi başarılı";
            return RedirectToAction(nameof(BlogController.Save));
        }

        
    }
}
