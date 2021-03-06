﻿using OsrmRouteDirection.Models;
using OsrmRouteDirection.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Linq;

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

        bool  _showRouteDetails;
        public bool ShowRouteDetails
        {
            get { return _showRouteDetails; }
            set { _showRouteDetails = value; OnPropertyChanged(); }
        }

        public static Map map;
        public Command GetRouteCommand { get; }
        private OSRMRouteService services;
        private DirectionResponse dr;

        public RouteViewModel()
        {
            ShowRouteDetails = false;
            map = new Map();
            services = new OSRMRouteService();
            dr = new DirectionResponse();
            GetRouteCommand = new Command(async () => await loadRouteAsync(Origin, Destination));
        }

        public async Task loadRouteAsync(string origin, string destination)
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
            List<LatLong> locations = new List<LatLong>();

            //call the osrm route service
            dr = await services.GetDirectionResponseAsync(origin, destination);
            if (dr != null)
            {
                ShowRouteDetails = false;
                await Task.Delay(1000);
                routes = dr.Routes.ToList();

                //convert the route duration into minute
                RouteDuration = Math.Round((Double)routes[0].Duration / 60, 0);
                //convert the route distance in Km
                RouteDistance = Math.Round((double)routes[0].Distance / 1000, 1);
                Fare = Math.Round((double)RouteDistance * 1.25, 2);

                if (Fare <= 6) { Fare = 6; }

                ShowRouteDetails = true;
                IsBusy = false;
            }
        }
    }
}
