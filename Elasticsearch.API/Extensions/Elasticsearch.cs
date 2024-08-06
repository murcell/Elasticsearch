using Elastic.Clients.Elasticsearch;
using Elastic.Transport;


namespace Elasticsearch.API.Extensions
{
    public static class ElasticsearchExt
    {
        public static void AddElastic(this IServiceCollection services, IConfiguration configuration) 
        {
            #region NEST Library
            //var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Url"]!));
            //var setttings = new ConnectionSettings(pool);
            ////setttings.BasicAuthentication(); usr dn pass will write here
            //var client = new ElasticClient(setttings); 
            #endregion

            var userName = configuration.GetSection("Elastic")["Username"];
            var password = configuration.GetSection("Elastic")["Password"];
            var settings = new ElasticsearchClientSettings(new Uri(configuration.GetSection("Elastic")["Url"]!)).Authentication(new BasicAuthentication(userName,password));

            var client = new ElasticsearchClient(settings);

            services.AddSingleton(client);
          
        }
    }
}
