using Newtonsoft.Json;
using Syncfusion.Blazor.Inputs;
using System.Collections.ObjectModel;

namespace BlazorAISample.Pages
{
    public partial class Index
    {
        SfTextBox TextBox;
        private string[] hospitalImageList = new string[]
        {
            "hospital1.jpg",
            "hospital2.jpg",
            "hospital3.jpg",
            "hospital4.jpg",
            "hospital5.jpg",
            "hospital6.jpg"
        };
        private Random _random = new Random();
        public bool SpinnerVisibility { get; set; } = true;
        ObservableCollection<Markers> MarkerCollection = new ObservableCollection<Markers>();
        private string SearchQuery { get; set; } = "Hospitals in New York";

        private async Task ValueChanged(string changedValue)
        {
            SearchQuery = !string.IsNullOrEmpty(changedValue) ? changedValue : string.Empty;
            MarkerCollection.Clear();
            SpinnerVisibility = true;
            await GetMarkerData(SearchQuery);
        }

        public async Task GetMarkerData(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string result = await OpenAIService.GetAIResponse("Generate" + value + "for 15 important cities as a JSON object, with fields such as 'city_name', 'place_name', 'latitude', 'longitude', 'place_details' and 'address'. Provide the simple address. The information about the place with minimum 150 characters. Strictly provide flat JSON list without nested objects. Strictly provide information in English Language.");
                if (result.Contains("```json"))
                {
                    string cleanedResponseText = result.Split("```json")[1].Trim();
                    result = cleanedResponseText.Split("```")[0].Trim();
                    if (!string.IsNullOrEmpty(result))
                    {
                        MarkerCollection = JsonConvert.DeserializeObject<ObservableCollection<Markers>>(result);
                    }
                }
                else
                {
                    MarkerCollection.Clear();
                }
            }
            else
            {
                MarkerCollection.Clear();
            }
            SpinnerVisibility = false;
        }

        private async Task Loaded()
        {
            if (MarkerCollection.Count == 0)
            {
                await GetMarkerData(SearchQuery);
            }
        }

        public class Markers
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            [JsonProperty("place_name")]
            public string PlaceName { get; set; }

            [JsonProperty("city_name")]
            public string CityName { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }

            [JsonProperty("place_details")]
            public string PlaceDetails { get; set; }
        }

        public void AddSearchIcon()
        {
            TextBox.AddIconAsync("append", "fas fa-search");
        }
    }
}
