using Newtonsoft.Json;
using OsrmRouteDirection.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace OsrmRouteDirection.Services
{

    class OSRMRouteService
    {
        private readonly string baseRouteUrl =
            "http://router.project-osrm.org/route/v1/driving/";
        private HttpClient _httpClient;

        public OSRMRouteService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<DirectionResponse> GetDirectionResponseAsync(string origin, string destination)
        {
            var originLocations = await Geocoding.GetLocationsAsync(origin);
            var originLocation = originLocations?.FirstOrDefault();

            var destinationLocations = await Geocoding.GetLocationsAsync(destination);
            var destinationLocation = destinationLocations?.FirstOrDefault();

            if(originLocation == null || destinationLocation == null) { return null; }
            if (originLocation != null && destinationLocation != null)
            {
                string url = string.Format(baseRouteUrl) + $"{originLocation.Longitude},{ originLocation.Latitude};" +
                    $"{destinationLocation.Longitude},{destinationLocation.Latitude}?overview=full&geometries=polyline&steps=false";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<DirectionResponse>(json);
                    return result;
                }
            }
            else 
            {
                return null;
            }
            return null;
        }

        public async Task<LatLong> GetPositionResponseAsync(string position)
        {
            var locations = await Geocoding.GetLocationsAsync(position);
            var location =locations?.FirstOrDefault();
            LatLong latLong = new LatLong();


            if (location != null)
            {
                latLong.Lat = location.Latitude;
                latLong.Lng = location.Longitude;
                return latLong;
            }
            else 
            { 
                return null;
            }
        }


    }
}
