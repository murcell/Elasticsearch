using Elastiicsearch.Web.Services;
using Elastiicsearch.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Elastiicsearch.Web.Controllers
{
	public class ECommerceController : Controller
	{
		private readonly ECommerceService _service;

		public ECommerceController(ECommerceService service)
		{
			_service = service;
		}

		public async Task<IActionResult> Search([FromQuery] SearchPageViewModel searchPageView)
		{
			var (eCommerceList, totalCount, pageLinkCount) = await _service.SearchAsync(searchPageView.SearchViewModel, searchPageView.Page,
				searchPageView.PageSize);


			searchPageView.List = eCommerceList;
			searchPageView.TotalCount = totalCount;
			searchPageView.PageLinkCount = pageLinkCount;


			return View(searchPageView);
		}
	}
}
