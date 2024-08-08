using Elastiicsearch.Web.Models;
using Elastiicsearch.Web.Repositories;
using Elastiicsearch.Web.ViewModels;

namespace Elastiicsearch.Web.Services
{
    public class BlogService
    {
        // Interface'in önemleerinden bir tanesi;
        // UI/API(controller) soyutlama açısından concrete sınıfını bilmemesi gerekiyor
        // Her zaman interface'in refansı üzerinden iletişim kurmalı,
        // dependency inversion, inversion of control
  
        private readonly BlogRepository _repository;

        public BlogService(BlogRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> SaveAsync(BlogCreateViewModel model)
        {
            Blog blog = new Blog()
            {
                Title = model.Title,
                Content = model.Content,
                UserId = Guid.NewGuid(),
                Tags = model.Tags.Split(",")
            };

            var isCreated = await _repository.SaveAsync(blog);
            
            return isCreated != null;
        }

        public async Task<List<BlogViewModel>> SearchAsync(string searchText)
        {
			var blogList = await _repository.SearchAsync(searchText);

			return blogList.Select(b => new BlogViewModel()
			{
				Id = b.Id,
				Title = b.Title,
				Content = b.Content,
				Created = b.Created.ToShortDateString(),
				Tags = String.Join(",", b.Tags),
				UserId = b.UserId.ToString()

			}).ToList();
		}
    }
}
