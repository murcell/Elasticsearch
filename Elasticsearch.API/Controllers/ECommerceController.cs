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
            return Ok(await _eCommerceRepository.TermQuery(customerFirstName));
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery([FromBody]List<string> customerFirstNameList)
        {
            return Ok(await _eCommerceRepository.TermsQuery(customerFirstNameList));
        }
    }
}
