﻿using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models;
using Nest;
using System.Collections.Immutable;


namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticClient _client;
        private const string indexName = "products11";
        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created = DateTime.Now;

            //var response = await _client.IndexAsync(newProduct, x => x.Index(indexName));
            // fast fail
            //if (!response.IsSuccess()) return null;
            //newProduct.Id = response.Id;

            var response = await _client.IndexAsync(newProduct, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));
            // fast fail
            if (!response.IsValid) return null;
            newProduct.Id = response.Id;
            return newProduct;
        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => 
                     s.Index(indexName)
                      .Query(q => q.MatchAll()));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<Product>(id, x => x.Index(indexName));

            //if (!response.IsSuccess())
            //{
            //    return null;
            //}

            if (!response.IsValid) return null;

            response.Source.Id = response.Id;
            return response.Source;
        }


        //public async Task<bool> UpdateSynch(ProductUpdateDto updateProduct)
        //{
        //    var response = await _client.UpdateAsync<Product, ProductUpdateDto>(indexName, updateProduct.Id, x => x.Doc(updateProduct));

        //    return response.IsSuccess();

        //}
        /// <summary>
        /// Hata yönetimi için bu method ele alınmıştır.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteAsync(string id)
        {

            var response = await _client.DeleteAsync<Product>(id, x => x.Index(indexName));
            return response;
        }
    }
}