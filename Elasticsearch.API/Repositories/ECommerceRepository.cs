using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.TermVectors;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Models.ECommerceModel;
using Microsoft.AspNetCore.DataProtection.Repositories;
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

        public async Task<ImmutableList<ECommerce>> TermQueryAsync(string customerFirstName)
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

        public async Task<ImmutableList<ECommerce>> TermsQueryAsync(List<string> customerFirstNameList)
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
            .Size(100)
            .Query(q=>q
            .Terms(t=>t
            .Field(f=>f.CustomerFirstName.Suffix("keyword")).Term(new TermsQueryField(terms.AsReadOnly())))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
            .Query(q => q
                .Prefix(p=>p
                    .Field(f=>f.CustomerFullName
                        .Suffix("keyword"))
                             .Value(customerFullName))
            ));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQueryAsync(double fromPrize, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
            .Query(q => q
                .Range(r=>r
                    .NumberRange(nr=>nr
                        .Field(f=>f.TaxfulTotalPrice).Gte(fromPrize).Lte(toPrice)))
            ));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQueryAsync()
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
            .Query(q => q.MatchAll(new MatchAllQuery())));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PaginationQueryAsync(int page, int pageSize)
        {
            // page=1 pageSize=0 => 1-10
            // page=2 pageSize=10 => 11-20
            // page=3 pageSize=10 => 21-30

            var pageFrom = (page - 1) * pageSize;

            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Size(pageSize).From(pageFrom)
                .Query(q => q.MatchAll(new MatchAllQuery())));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> WildCardQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
            .Query(q => q
                .Wildcard(w=>w
                    .Field(f=>f.CustomerFullName.Suffix("keyword"))
                        .Wildcard(customerFullName))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyQueryAsync(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q.Fuzzy(fz=>fz.Field(f=>f.CustomerFirstName.Suffix("keyword")).Value(customerFirstName).Fuzziness(new Fuzziness(2)))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyAndOrderingQueryAsync(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q => q
                    .Fuzzy(fz => fz
                        .Field(f => f.CustomerFirstName.Suffix("keyword")).Value(customerFirstName)
                            .Fuzziness(new Fuzziness(2))))
                                .Sort(sort=>sort.Field(f=>f.TaxfulTotalPrice,new FieldSort() { Order = SortOrder.Desc })));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }
    }

}
