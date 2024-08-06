using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Models.ECommerceModel;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        private const string indexName = "kibana_sample_data_ecommerce";

        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {
            ////1.Way
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Term(t => t.Field("customer_first_name.keyword"!).Value(customerFirstName))));

            // Second way
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q.Term(t => t.Field(f => f.CustomerFirstName.Suffix("keyword")).Value(customerFirstName))));

            //// third way
            //var termQuery = new TermQuery("customer_first_name.keyword"!) { Value = customerFirstName };
            // var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNameList)
        {
            var terms = new List<FieldValue>();
            customerFirstNameList.ForEach(x => { terms.Add(x); });
           
            //// First way
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword"!,
            //    Term = new TermsQueryField(terms.AsReadOnly())
            //};

            //var result = await _client.SearchAsync<ECommerce>(s=>s.Index(indexName).Query(termsQuery));

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q=>q
            .Terms(t=>t
            .Field(f=>f.CustomerFirstName.Suffix("keyword")).Term(new TermsQueryField(terms.AsReadOnly())))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

    }
    
}
