using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastiicsearch.Web.Models;

namespace Elastiicsearch.Web.Repositories
{
    public class BlogRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "blog";
        public BlogRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            newBlog.Created = DateTime.Now;
            var response = await _client.IndexAsync(newBlog,x=>x.Index(indexName));

            if (!response.IsValidResponse)
                return null;

            newBlog.Id = response.Id;
            return newBlog;
        }


        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            List<Action<QueryDescriptor<Blog>>> listQuery = new();

            Action<QueryDescriptor<Blog>> matchAll = (q) => q.MatchAll(new MatchAllQuery());

            Action<QueryDescriptor<Blog>> matchContent = (q) => q.Match(mt => mt.Field(f => f.Content).Query(searchText));

            Action<QueryDescriptor<Blog>> matchTitleBoolPrefix = (q) => q.MatchBoolPrefix(p => p.Field(f => f.Title).Query(searchText));

			Action<QueryDescriptor<Blog>> tagTerm = (q) => q.Term(p => p.Field(f => f.Tags).Value(searchText));

			if (string.IsNullOrEmpty(searchText))
            {
                listQuery.Add(matchAll);
            }
            else
            {
                listQuery.Add(matchContent);
                listQuery.Add(matchTitleBoolPrefix);
				listQuery.Add(tagTerm);
			}
            // title => full text
            // content => full text
            // should (term1 or term2 o term3)
            var result = await _client.SearchAsync<Blog>(s => s
                                           .Index(indexName)
                                               .Size(50)
                                                   .Query(q => q
                                                       .Bool(b => b
                                                           .Should(listQuery.ToArray()))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToList();

        }
    }
}
