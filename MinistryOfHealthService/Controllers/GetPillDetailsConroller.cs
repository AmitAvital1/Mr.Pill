using Microsoft.AspNetCore.Mvc;
using MOHService.service;
using MrPill.DTOs.DTOs;
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
        public Task<MohPillDetailsDTO> GetPillDetailsFromApiById(string barcode)
        {
            return _mohApiService.GetPillDetailsAPI(barcode);
        }
    }
}