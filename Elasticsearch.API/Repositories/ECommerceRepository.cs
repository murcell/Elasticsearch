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

        public async Task<ImmutableList<ECommerce>> MatchQueryFullTextAsync(string categoryName)
        {
            var result = await _client.SearchAsync<ECommerce>(s=>s
            .Index(indexName)
                .Size(50)    
                    .Query(q=>q
                        .Match(m=>m.
                            Field(f=>f.CustomerFullName)
                                .Query(categoryName)
                                    .Operator(Operator.And)))); // default'u Operator.Or

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }


        /// <summary>
        ///  arama birden fazla kelime içeriyorsa en sndaki kelimeyi bir prefix olarak ele alıyorÖrn: Name : Kırmızı Buz => Buz ile başlayan herhangi bir kelimeyi getirir. Kırmızı varsa onu da getirir. Buz or Kırmızı
        /// </summary>
        /// <param name="customerFullName"></param>
        /// <returns></returns>
        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(50)
                    .Query(q => q
                        .MatchBoolPrefix(m => m.
                            Field(f => f.CustomerFullName)
                                .Query(customerFullName).Operator(Operator.Or)))); 

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhraseQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(50)
                    .Query(q => q
                        .MatchPhrase(m => m.
                            Field(f => f.CustomerFullName)
                                .Query(customerFullName))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleOneAsync(string cityName, double taxfulTotalPrize, string categoryName, string manufacturer)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(50)
                    .Query(q => q.
                        Bool(b=>b
                            .Must(m=>m
                                .Term(t=>t.Field("geoip.city_name"!).Value(cityName)))
                            .MustNot(mn=>mn
                                .Range(r=>r
                                    .NumberRange(nr=>nr.Field(f=>f.TaxfulTotalPrice).Lte(taxfulTotalPrize))))
                            .Should(sh=>sh.Term(t=>t.Field(f=>f.Category.Suffix("keyword")).Value(categoryName)))
                            .Filter(f=>f.Term(t=>t.Field("manufacturer.keyword").Value(manufacturer))))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleTwoAsync(string customerFullName)
        {
            //var result = await _client.SearchAsync<ECommerce>(s => s
            //.Index(indexName)
            //    .Size(50)
            //        .Query(q => q
            //            .Bool(b=>b
            //                .Must(m=>m
            //                    .Match(mt=>mt
            //                        .Field(f=>f.CustomerFullName)
            //                            .Query(customerFullName))))));


            //// or
            //var result = await _client.SearchAsync<ECommerce>(s => s
            //   .Index(indexName)
            //       .Size(50)
            //           .Query(q => q.MatchPhrasePrefix(m=>m.Field(f=>f.CustomerFullName).Query(customerFullName))));

            //// or
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                    .Size(50)
                        .Query(q => q
                            .Bool(b => b
                                .Should(
                                m => m.Match(mt => mt
                                        .Field(f => f.CustomerFullName)
                                        .Query(customerFullName)),
                                m=>m.Prefix(p => p
                                        .Field(f => f.CustomerFullName
                                            .Suffix("keyword"))
                                        .Value(customerFullName))))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MultiMatchQueryAsync(string name)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
               .Index(indexName)
                   .Size(50)
                       .Query(q => q
                            .MultiMatch(mm=>mm
                                .Fields(new Field("customer_first_name")
                                   .And(new Field("customer_last_name"))
                                   .And(new Field("customer_full_name")))
                                .Query(name)))); 

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }
    }

}
