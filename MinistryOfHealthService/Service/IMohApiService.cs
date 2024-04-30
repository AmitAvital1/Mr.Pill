namespace MOHService.service
{
    public interface IMohApiService
    {
        Task<string> GetPillDetailsAPI(string barcode);
    }
}