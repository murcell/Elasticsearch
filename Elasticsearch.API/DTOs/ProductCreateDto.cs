using Elasticsearch.API.Models;

namespace Elasticsearch.API.DTOs
{
    // record olunca immutable oluyoryani nesne üretildikten sonra değiştirilemiyor
    public record ProductCreateDto(string Name, decimal Price, int Stock, ProductFeatureDto Feature)
    {
        public Product CreateProduct()
        {
            var color = (EColor)int.Parse(Feature.Color);

            return new Product
            {
                Name = Name,
                Price = Price,
                Stock = Stock,
                Feature = new ProductFeature()
                {
                    Width = Feature.Width,
                    Height = Feature.Height,
                    Color = color
                }
            };

        }
    }
}
