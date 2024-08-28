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
                    MohPillDetailsDTO dtoToReturn = createPillDetailsDtoFromJson(pillJsonDetails);
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

        private MohPillDetailsDTO createPillDetailsDtoFromJson(JToken pillJsonDetails)
        {
            try
            {
                _logger.LogInformation("Starting to create MohPillDetailsDTO from JSON.");

                var packages = pillJsonDetails[MOH_JSON_PACKAGES_KEY]
                    ?.Select(package => package.ToString())
                    .ToList();

                if (packages == null || !packages.Any())
                {
                    _logger.LogWarning("No packages found in the JSON data.");
                }
                else
                {
                    _logger.LogInformation($"Found {packages.Count} packages.");
                }

                var largestPackage = packages?
                    .Select(p =>
                    {
                        int size = int.Parse(p.Split(' ')[0]);
                        _logger.LogInformation($"Parsed package size: {size}");
                        return size;
                    })
                    .Max();

                if (largestPackage.HasValue)
                {
                    _logger.LogInformation($"Largest package size determined: {largestPackage.Value}");
                }
                else
                {
                    _logger.LogWarning("Unable to determine the largest package size.");
                }

                var dto = new MohPillDetailsDTO.Builder()
                    .SetBarcode(pillJsonDetails[MOH_JSON_BARCODES_KEY]?.ToString() ?? string.Empty)
                    .SetDrugHebrewName(pillJsonDetails[MOH_JSON_DRAG_HEB_NAME_KEY]?.ToString() ?? string.Empty)
                    .SetDrugEnglishName(pillJsonDetails[MOH_JSON_DRAG_ENG_NAME_KEY]?.ToString() ?? string.Empty)
                    .SetEnglishDescription(pillJsonDetails[MOH_JSON_DRAG_ENG_DESC_KEY]?.ToString() ?? string.Empty)
                    .SetHebrewDescription(pillJsonDetails[MOH_JSON_DRAG_HEB_DESC_KEY]?.ToString())
                    .SetImagePath(MOH_IMAGE_BASE_URL + pillJsonDetails[MOH_JSON_DRAG_IMG_PATH_KEY][0]["url"].ToString() ?? string.Empty)
                    .SetPackageSize(largestPackage ?? 0)
                    .Build();

                _logger.LogInformation("MohPillDetailsDTO successfully created.");

                return dto;
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
    }
}