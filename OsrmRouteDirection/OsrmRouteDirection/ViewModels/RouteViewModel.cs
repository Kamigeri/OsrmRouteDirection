﻿using OsrmRouteDirection.Models;
using OsrmRouteDirection.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;
using System.Diagnostics;

namespace OsrmRouteDirection.ViewModels
{
    public class RouteViewModel : MyViewModel
    {
        private string _origin;
        public string Origin
        {
            get { return _origin; }
            set { _origin = value; OnPropertyChanged(); }
        }

        private string _destination;
        public string Destination
        {
            get { return _destination; }
            set { _destination = value; OnPropertyChanged(); }
        }

        private double _routeDuration;
        public double RouteDuration
        {
            get { return _routeDuration; }
            set { _routeDuration = value; OnPropertyChanged(); }

        }

        private double _routeDistance;
        public double RouteDistance
        {
            get { return _routeDistance; }
            set { _routeDistance = value; OnPropertyChanged(); }
        }

        private double _fare;
        public double Fare
        {
            get { return _fare; }
            set { _fare = value; OnPropertyChanged(); }
        }

        bool _showRouteDetails;
        public bool ShowRouteDetails
        {
            get { return _showRouteDetails; }
            set { _showRouteDetails = value; OnPropertyChanged(); }
        }

        public static Xamarin.Forms.Maps.Map map;
        public MapSpan mapSpan;
        public Pin pin;
        public Polyline polyline;
        public Command GetRouteCommand { get; }

        private OSRMRouteService services;
        private OsrmRouteDirectionModels dr;


        public RouteViewModel()
        {
            ShowRouteDetails = false;
            map = new Xamarin.Forms.Maps.Map();
            services = new OSRMRouteService();
            dr = new OsrmRouteDirectionModels();
            GetRouteCommand = new Command(async () => await loadRouteAsync(Origin, Destination));
        }

        public async Task loadRouteAsync(string origin, string destination)
        {
            try
            {
                var current = Xamarin.Essentials.Connectivity.NetworkAccess;
                if (current != Xamarin.Essentials.NetworkAccess.Internet)
                {
                    await DisplayAlert("Error:", "You must be connected to the internet.", "OK");
                    return;
                }
                if (origin == null || destination == null)
                {
                    await DisplayAlert("Error:", "Origin and Destination must not be empty.", "OK");
                    return;
                }

                IsBusy = true;
                List<Route> routes = new List<Route>();
                List<Leg> legs = new List<Leg>();
                List<Step> steps = new List<Step>();
                List<Intersection> intersections = new List<Intersection>();
                List<LatLong> locations = new List<LatLong>();
                Maneuver maneuver = new Maneuver();

                //call the osrm route service
                dr = await services.GetDirectionResponseAsync(origin, destination);

                if (dr == null)
                {
                    await DisplayAlert("Error:", "Could not found route.", "OK");
                    return;
                }

                if (dr != null)
                {
                    ShowRouteDetails = false;
                    await Task.Delay(1000);
                    //List all object
                    routes = dr.Routes.ToList();
                    //convert the route duration into minute
                    RouteDuration = Math.Round((Double)routes[0].Duration / 60, 0);
                    //convert the route distance in Km
                    RouteDistance = Math.Round((double)routes[0].Distance / 1000, 1);

                    foreach (var route in routes)
                    {
                        legs = route.Legs.ToList();
                    }

                    foreach (var leg in legs)
                    {
                        steps = leg.Steps.ToList();
                    }

                    foreach (var step in steps)
                    {
                        var intersectionsLocal = step.Intersections.ToList();
                        foreach (var intersection in intersectionsLocal)
                        {
                            intersections.Add(intersection);
                        }
                    }

                    foreach (var intersection in intersections)
                    {
                        LatLong p = new LatLong();
                        p.Lat = intersection.Location[1];
                        p.Lng = intersection.Location[0];

                        locations.Add(p);
                    }

                    foreach (var step in steps)
                    {
                        maneuver = step.Maneuver;
                    }

                    Polyline polyline = new Polyline
                    {
                        StrokeColor = Color.Black,
                        StrokeWidth = 7,
                    };

                    foreach (var latlong in locations)
                    {
                        polyline.Geopath.Add(new Position(latlong.Lat, latlong.Lng));
                    }

                    map.MapElements.Add(polyline);

                    var firstPinLocation = locations[0];
                    var lastPinLocation = locations[locations.Count() - 1];

                    MapSpan mapSpan = MapSpan.FromCenterAndRadius(
                        new Position(firstPinLocation.Lat, firstPinLocation.Lng)
                        , Distance.FromKilometers(6.7));
                    map.MoveToRegion(mapSpan);

                    Pin pin = new Pin
                    {
                        Label = "Origin",
                        Address = Origin,
                        Type = PinType.SearchResult,
                        Position = new Position(firstPinLocation.Lat, firstPinLocation.Lng)
                    };

                    map.Pins.Add(pin);

                    Pin pin2 = new Pin
                    {
                        Label = "Origin",
                        Address = Origin,
                        Type = PinType.SearchResult,
                        Position = new Position(firstPinLocation.Lat, firstPinLocation.Lng)
                    };

                    map.Pins.Add(pin2);

                    Fare = Math.Round((double)RouteDistance * 1.25, 2);
                    if (Fare <= 6) { Fare = 6; }
                    await GetDestinationPosition(destination);
                    ShowRouteDetails = true;
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error:", "Could not found route.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
         }
        public async Task GetDestinationPosition(string destination)
        {
            LatLong latLong = await services.GetPositionResponseAsync(destination);
            if (latLong != null)
            {
                Position position = new Position(latLong.Lat, latLong.Lng);
                pin = new Pin
                {
                    Label = await GetLocationName(position),
                    Position = position
                };
                map.Pins.Add(pin);
                mapSpan = new MapSpan(position, 0.1, 0.1);
                map.MoveToRegion(mapSpan);
            }
        }

        public async Task<string> GetLocationName(Position position)
        {
            string strLocationName = string.Empty;
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    strLocationName = String.Format($"{placemark.Thoroughfare},{placemark.SubThoroughfare}, {placemark.SubAdminArea},{placemark.AdminArea},{placemark.Locality}");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return strLocationName;
        }
    }
}
