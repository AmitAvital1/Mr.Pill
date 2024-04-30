using Microsoft.AspNetCore.Mvc;
using MOHService.service;

namespace MOHService.Controllers
{
    [Route("moh-service")]
    public class MohApiController : Controller
    {
        private readonly IMohApiService _mohApiService;

        public MohApiController(IMohApiService mohApiService)
        {
            _mohApiService = mohApiService;
        }

        [HttpGet("pill-details/{barcode}")]
        public Task<string> GetPillDetailsFromApiById(string barcode)
        {
            return _mohApiService.GetPillDetailsAPI(barcode);
        }
    }
}