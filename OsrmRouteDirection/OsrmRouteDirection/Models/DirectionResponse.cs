﻿using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace OsrmRouteDirection.Models
{
    public class LatLong
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
        public partial class OsrmRouteDirectionModels
        {
            [JsonProperty("routes")]
            public Route[] Routes { get; set; }

            [JsonProperty("waypoints")]
            public Waypoint[] Waypoints { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }

        public partial class Route
        {
            [JsonProperty("geometry")]
            public string Geometry { get; set; }

            [JsonProperty("legs")]
            public Leg[] Legs { get; set; }

            [JsonProperty("weight_name")]
            public string WeightName { get; set; }

            [JsonProperty("weight")]
            public double Weight { get; set; }

            [JsonProperty("duration")]
            public long Duration { get; set; }

            [JsonProperty("distance")]
            public double Distance { get; set; }
        }

        public partial class Leg
        {
            [JsonProperty("summary")]
            public string Summary { get; set; }

            [JsonProperty("weight")]
            public double Weight { get; set; }

            [JsonProperty("duration")]
            public long Duration { get; set; }

            [JsonProperty("steps")]
            public Step[] Steps { get; set; }

            [JsonProperty("distance")]
            public double Distance { get; set; }
        }

        public partial class Step
        {
            [JsonProperty("intersections")]
            public Intersection[] Intersections { get; set; }

            [JsonProperty("driving_side")]
            public string DrivingSide { get; set; }

            [JsonProperty("geometry")]
            public string Geometry { get; set; }

            [JsonProperty("mode")]
            public string Mode { get; set; }

            [JsonProperty("maneuver")]
            public Maneuver Maneuver { get; set; }

            [JsonProperty("weight")]
            public double Weight { get; set; }

            [JsonProperty("duration")]
            public double Duration { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("distance")]
            public double Distance { get; set; }

            [JsonProperty("rotary_name", NullValueHandling = NullValueHandling.Ignore)]
            public string RotaryName { get; set; }

            [JsonProperty("ref", NullValueHandling = NullValueHandling.Ignore)]
            public string Ref { get; set; }
        }

        public partial class Intersection
        {
            [JsonProperty("out", NullValueHandling = NullValueHandling.Ignore)]
            public long? Out { get; set; }

            [JsonProperty("entry")]
            public bool[] Entry { get; set; }

            [JsonProperty("bearings")]
            public long[] Bearings { get; set; }

            [JsonProperty("location")]
            public double[] Location { get; set; }

            [JsonProperty("in", NullValueHandling = NullValueHandling.Ignore)]
            public long? In { get; set; }

            [JsonProperty("lanes", NullValueHandling = NullValueHandling.Ignore)]
            public Lane[] Lanes { get; set; }

            [JsonProperty("classes", NullValueHandling = NullValueHandling.Ignore)]
            public string[] Classes { get; set; }
        }

        public partial class Lane
        {
            [JsonProperty("valid")]
            public bool Valid { get; set; }

            [JsonProperty("indications")]
            public string[] Indications { get; set; }
        }

        public partial class Maneuver
        {
            [JsonProperty("bearing_after")]
            public long BearingAfter { get; set; }

            [JsonProperty("bearing_before")]
            public long BearingBefore { get; set; }

            [JsonProperty("location")]
            public double[] Location { get; set; }

            [JsonProperty("modifier")]
            public string Modifier { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("exit", NullValueHandling = NullValueHandling.Ignore)]
            public long? Exit { get; set; }
        }

        public partial class Waypoint
        {
            [JsonProperty("hint")]
            public string Hint { get; set; }

            [JsonProperty("distance")]
            public double Distance { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("location")]
            public double[] Location { get; set; }
        }

        public partial class OsrmRouteDirectionModels
        {
            public static OsrmRouteDirectionModels FromJson(string json) => JsonConvert.DeserializeObject<OsrmRouteDirectionModels>(json, OsrmRouteDirection.Models.Converter.Settings);
        }

        public static class Serialize
        {
            public static string ToJson(this OsrmRouteDirectionModels self) => JsonConvert.SerializeObject(self, OsrmRouteDirection.Models.Converter.Settings);
        }

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }
 }


