using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<ActionResult<MohPillDetailsDTO>> GetPillDetailsFromApiById(string barcode)
        {   try
            {
                MohPillDetailsDTO res = await _mohApiService.GetPillDetailsAPI(barcode);
                return Ok(res);
            }
            catch(UnsupportedContentTypeException e){
                return StatusCode(((int)HttpStatusCode.NoContent),e.Data);
            }
        }
    }
}