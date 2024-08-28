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
        private readonly ILogger<MohApiController> _logger;

        public MohApiController(IMohApiService mohApiService, ILogger<MohApiController> logger)
        {
            _mohApiService = mohApiService;
            _logger = logger;
        }

        [HttpGet("pill-details/{barcode}")]
        public async Task<ActionResult<MohPillDetailsDTO>> GetPillDetailsFromApiById(string barcode)
        {   
            _logger.LogInformation("Received request to get pill details for barcode: {Barcode}", barcode);

            try
            {
                MohPillDetailsDTO res = await _mohApiService.GetPillDetailsAPI(barcode);
                _logger.LogInformation("Successfully retrieved pill details for barcode: {Barcode}", barcode);
                return Ok(res);
            }
            catch(UnsupportedContentTypeException e){
                 _logger.LogWarning("No content found for barcode: {Barcode}. Exception: {ExceptionMessage}", barcode, e.Message);
                return StatusCode(((int)HttpStatusCode.NoContent),e.Data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving pill details for barcode: {Barcode}", barcode);
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}