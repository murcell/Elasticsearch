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
    }
}
