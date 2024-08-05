using Elasticsearch.Net;
using Nest;

namespace Elasticsearch.API.Extensions
{
    public static class ElasticsearchExt
    {
        public static void AddElastic(this IServiceCollection services, IConfiguration configuration) 
        {
            var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Url"]!));
            var setttings = new ConnectionSettings(pool);
            //setttings.BasicAuthentication(); usr dn pass will write here
            var client = new ElasticClient(setttings);
            services.AddSingleton(client);
          
        }
    }
}
