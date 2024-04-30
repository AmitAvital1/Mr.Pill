using Newtonsoft.Json.Linq;
using System.Text;

namespace MOHService.service
{
    public class MohApiService : IMohApiService
    {
        private readonly HttpClient _httpClient;

        public MohApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetPillDetailsAPI(string barcode)
        {
                string apiUrl = "https://israeldrugs.health.gov.il/GovServiceList/IDRServer/SearchByAdv";

                var content = new StringContent(createJsonBodyToMohWithBarcode(barcode), Encoding.UTF8, "application/json");
            
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string pillDetails = await response.Content.ReadAsStringAsync();
                    JArray jsonArray = JArray.Parse(pillDetails);
                    return jsonArray.ToString();

                }
                else
                {
                    throw new HttpRequestException($"Failed to post data to the API. Status code: {response.StatusCode}");
                }
        }

        private string createJsonBodyToMohWithBarcode(string barcode)
        {
            string originalJson = @"
                                    {
                                    ""val"": ""5000158100800"",
                                    ""veterinary"": false,
                                    ""cytotoxic"": false,
                                    ""prescription"": false,
                                    ""isGSL"": true,
                                    ""healthServices"": false,
                                    ""isPeopleMedication"": false,
                                    ""fromCanceledDrags"": null,
                                    ""toCanceledDrags"": null,
                                    ""fromUpdateInstructions"": null,
                                    ""toUpdateInstructions"": null,
                                    ""fromNewDrags"": null,
                                    ""toNewDrags"": null,
                                    ""newDragsDrop"": 0,
                                    ""pageIndex"": 1,
                                    ""orderBy"": 0,
                                    ""types"": ""0""
                                    }";


            JObject jsonObject = JObject.Parse(originalJson);
            jsonObject["val"] = barcode;
            return jsonObject.ToString();
        }
    }
}