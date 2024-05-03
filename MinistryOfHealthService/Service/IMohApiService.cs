using MrPill.DTOs.DTOs;
namespace MOHService.service
{
    public interface IMohApiService
    {
        Task<MohPillDetailsDTO> GetPillDetailsAPI(string barcode);
    }
}