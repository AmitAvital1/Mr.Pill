using System.Text;
using MrPill.DTOs.DTOs;
using static MrPill.MOHService.Constants.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;

namespace MOHService.service
{
    public class MohApiService : IMohApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MohApiService> _logger;

        public MohApiService(IHttpClientFactory httpClientFactory, ILogger<MohApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<MohPillDetailsDTO> GetPillDetailsAPI(string barcode)
        {
            _logger.LogInformation("Attempting to retrieve pill details from API for barcode: {Barcode}", barcode);

            string apiUrl = MOH_API_URL;
            var content = new StringContent(createJsonBodyToMohWithBarcode(barcode), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Received successful response from API for barcode: {Barcode}", barcode);

                string pillDetails = await response.Content.ReadAsStringAsync();
                
                if (JArray.Parse(pillDetails).Count() > 0)
                {
                    _logger.LogInformation("Pill details found for barcode: {Barcode}", barcode);

                    JToken pillJsonDetails = JArray.Parse(pillDetails)[0];
                    string extraPillDetails = await getExtraPillDetailsAsync(pillJsonDetails[MOH_JSON_REGNUM_KEY]?.ToString() ?? string.Empty);
                    JToken extraPillJsonDetails = null;
                    if(extraPillDetails != null)
                    {
                        extraPillJsonDetails = JObject.Parse(extraPillDetails);
                    }

                    MohPillDetailsDTO dtoToReturn = createPillDetailsDtoFromJson(pillJsonDetails, extraPillJsonDetails, barcode);
                    return dtoToReturn;
                }
                else
                {
                    _logger.LogWarning("No pill details found for barcode: {Barcode}. Medication does not exist in MOH database.", barcode);
                    throw new UnsupportedContentTypeException("Bad barode: Medication not exist in Moh");
                }
            }
            else
            {
                _logger.LogError("Failed to retrieve pill details for barcode: {Barcode}. Status code: {StatusCode}", barcode, response.StatusCode);
                throw new HttpRequestException($"Failed to post data to the API. Status code: {response.StatusCode}");
            }
        }

        private MohPillDetailsDTO createPillDetailsDtoFromJson(JToken pillJsonDetails, JToken extraPillJsonDetails, string barcode)
        {
            try
            {
                 var dto = new MohPillDetailsDTO.Builder()
                 .SetPackageSize(0)
                 .SetBrochure(null);

                _logger.LogInformation("Starting to create MohPillDetailsDTO from JSON.");
                if(extraPillJsonDetails != null)
                {
                    JToken package = null;
                    foreach (JToken p in extraPillJsonDetails[MOH_JSON_PACKAGES_KEY])
                    {
                        if (p["barcode"] != null && p["barcode"].ToString() == barcode)
                        {
                            package = p;
                            break;
                        }
                    }
                    if (package == null)
                    {
                         _logger.LogWarning("No package found in the JSON extra data.");
                    }
                    else
                    {
                        int size = int.Parse(package[MOH_JSON_CAPLETSIZE_KEY].ToString().Split(' ')[0]);
                        dto.SetPackageSize(size);
                        _logger.LogInformation($"Found {size} packages.");
                    }

                    JToken brochure = null;
                    foreach (JToken b in extraPillJsonDetails[MOH_JSON_BROCHURE_KEY])
                    {
                        if (b["display"] != null && b["display"].ToString() == MOH_JSON_ALONZARHAN_KEY)
                        {
                            brochure = b;
                            break;
                        }
                    }
                    if (brochure == null)
                    {
                         _logger.LogWarning("No brochure found in the JSON extra data.");
                    }
                    else
                    {
                        string brochureUrl = MOH_BROCHURE_BASE_URL + brochure["url"].ToString();
                        dto.SetBrochure(brochureUrl);
                        _logger.LogInformation($"Found {brochureUrl}.");
                    }

                }

                dto 
                    .SetBarcode(pillJsonDetails[MOH_JSON_BARCODES_KEY]?.ToString() ?? string.Empty)
                    .SetDrugHebrewName(pillJsonDetails[MOH_JSON_DRAG_HEB_NAME_KEY]?.ToString() ?? string.Empty)
                    .SetDrugEnglishName(pillJsonDetails[MOH_JSON_DRAG_ENG_NAME_KEY]?.ToString() ?? string.Empty)
                    .SetEnglishDescription(pillJsonDetails[MOH_JSON_DRAG_ENG_DESC_KEY]?.ToString() ?? string.Empty)
                    .SetHebrewDescription(pillJsonDetails[MOH_JSON_DRAG_HEB_DESC_KEY]?.ToString())
                    .SetImagePath(MOH_IMAGE_BASE_URL + pillJsonDetails[MOH_JSON_DRAG_IMG_PATH_KEY][0]["url"].ToString() ?? string.Empty)
                    ;

                _logger.LogInformation("MohPillDetailsDTO successfully created.");

                return dto.Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating MohPillDetailsDTO from JSON.");
                throw;
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
                                    ""isGSL"": false,
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
                                    ""types"": ""9""
                                    }";

            JObject jsonObject = JObject.Parse(originalJson);
            jsonObject["val"] = barcode;

            return jsonObject.ToString();
        }

        private async Task<string> getExtraPillDetailsAsync(string regNum)
        {
            string originalJson = @"
                                    {
                                    ""dragRegNum"": ""128 74 25337 08"",
                                    }";

            JObject jsonObject = JObject.Parse(originalJson);
            jsonObject["dragRegNum"] = regNum;

             _logger.LogInformation("Attempting to retrieve caplet size and shelf life pill details from extra API");

            string apiUrl = MOA_LEAFLET_BASE_URL;
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Received successful response caplet size and shelf life pill details from extra API");
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                _logger.LogWarning("No pill details caplet size and shelf life pill details");
                return null;
            }

        }
    }
}