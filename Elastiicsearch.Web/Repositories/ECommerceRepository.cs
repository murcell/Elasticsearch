using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastiicsearch.Web.Models.ECommerceModel;
using Elastiicsearch.Web.ViewModels;

namespace Elastiicsearch.Web.Repositories
{
	public class ECommerceRepository
	{
		private readonly ElasticsearchClient _elasticSearchClient;

		public ECommerceRepository(ElasticsearchClient elasticSearchClient)
		{
			_elasticSearchClient = elasticSearchClient;
		}

		private const string IndexName = "kibana_sample_data_ecommerce";

		public async Task<(List<ECommerce> list,long count)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {
            List<Action<QueryDescriptor<ECommerce>>> listQuery = new();

            //Action<QueryDescriptor<Blog>> matchAll = (q) => q.MatchAll(new MatchAllQuery());
            //Action<QueryDescriptor<Blog>> matchContent = (q) => q.Match(mt => mt.Field(f => f.Content).Query(searchText));

            if (searchViewModel is null)
            {
                listQuery.Add(g => g.MatchAll(new MatchAllQuery()));
                return await CalculateResultSet(page, pageSize, listQuery);
            }

            if (!string.IsNullOrEmpty(searchViewModel.Category))
            {
                listQuery.Add((q) => q.Match(mt => mt.Field(f => f.Category).Query(searchViewModel.Category!)));
            }

            if (!string.IsNullOrEmpty(searchViewModel.CustomerFullName))
            {
                listQuery.Add((q) => q.Match(mt => mt.Field(f => f.CustomerFullName).Query(searchViewModel.CustomerFullName!)));
            }

            if (searchViewModel.OrderDateStart.HasValue)
            {
                listQuery.Add((q) => q
                                    .Range(r => r
                                        .DateRange(dr => dr
                                            .Field(f => f.OrderDate)
                                            .Gte(searchViewModel.OrderDateStart.Value))));
            }

            if (searchViewModel.OrderDateEnd.HasValue)
            {
                listQuery.Add((q) => q
                                    .Range(r => r
                                        .DateRange(dr => dr
                                            .Field(f => f.OrderDate)
                                            .Lte(searchViewModel.OrderDateEnd.Value))));
            }

            if (!string.IsNullOrEmpty(searchViewModel.Gender))
            {
                listQuery.Add((q) => q.Term(t => t.
                                            Field(f => f.Gender).Value(searchViewModel.Gender)));
            }

            if (!listQuery.Any())
            {

            }

            return await CalculateResultSet(page, pageSize, listQuery);
        }

        private async Task<(List<ECommerce> list, long count)> CalculateResultSet(int page, int pageSize, List<Action<QueryDescriptor<ECommerce>>> listQuery)
        {
            var pageFrom = (page - 1) * pageSize;

            var result = await _elasticSearchClient.SearchAsync<ECommerce>(s => s
                            .Index(IndexName)
                            .Size(pageSize)
                            .From(pageFrom)
                            .Query(q => q
                                .Bool(b => b
                                    .Must(listQuery.ToArray()))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return (list: result.Documents.ToList(), result.Total);
        }
    }
}
