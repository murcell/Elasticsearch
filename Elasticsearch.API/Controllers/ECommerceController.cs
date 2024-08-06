using Elasticsearch.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        // normalde servis clası oluşturup yapmak lazım.
        // bir öndeki kontrollerde yaptığım için burada direk repo üzerinden çağırdım
        private readonly ECommerceRepository _eCommerceRepository;

        public ECommerceController(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName) 
        {
            return Ok(await _eCommerceRepository.TermQueryAsync(customerFirstName));
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery([FromBody]List<string> customerFirstNameList)
        {
            return Ok(await _eCommerceRepository.TermsQueryAsync(customerFirstNameList));
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.PrefixQueryAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrize, double toPrice)
        {
            return Ok(await _eCommerceRepository.RangeQueryAsync(fromPrize, toPrice));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _eCommerceRepository.MatchAllQueryAsync());
        }

        [HttpGet]
        public async Task<IActionResult> PaginationQuery(int page=1, int pageSize=3)
        {
            return Ok(await _eCommerceRepository.PaginationQueryAsync(page,pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.WildCardQueryAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerFirstName)
        {
            return Ok(await _eCommerceRepository.FuzzyQueryAsync(customerFirstName));
        }

        [HttpGet]
        public async Task<IActionResult> FuzzyAndOrderingQuery(string customerFirstName)
        {
            return Ok(await _eCommerceRepository.FuzzyAndOrderingQueryAsync(customerFirstName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchQueryFullText(string categoryName)
        {
            return Ok(await _eCommerceRepository.MatchQueryFullTextAsync(categoryName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.MatchBoolPrefixAsync(customerFullName));
        }

        /// <summary>
        /// aranan kelimeler öbek olarak aranır. 
        /// Örn. Mehmet Ali Yılmaz-Mehmet Ali Erbil => Mehmet Ali diye aradığımızda içinde
        /// Mehmet Ali olan her şey gelir. Önde ve arkada ne olduğu önemli değildir
        /// </summary>
        /// <param name="customerFullName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> MatchPhraseQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.MatchPhraseQueryAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxfulTotalPrize, string categoryName, string manufacturer)
        {
            return Ok(await _eCommerceRepository.CompoundQueryExampleOneAsync(cityName,taxfulTotalPrize,categoryName,manufacturer));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleTwo(string customerFullName)
        {
            return Ok(await _eCommerceRepository.CompoundQueryExampleTwoAsync(customerFullName));
        }


        [HttpGet]
        public async Task<IActionResult> MultiMatchQuery(string name)
        {
            return Ok(await _eCommerceRepository.MultiMatchQueryAsync(name));
        }
    }
}
