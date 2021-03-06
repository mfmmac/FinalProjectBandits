﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinalProjectBandits.Services
{
    public interface IMUAPService
    {
        Task<MUAPObject> GetMUAPResults(string query);
    }

    public class MUAPService : IMUAPService
    {

        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _options;


        public MUAPService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        //query=1%3D1 returns all data
        public async Task<MUAPObject> GetMUAPResults(string query)
        {
            //https: //services2.arcgis.com/qvkbeam7Wirps6zC/arcgis/rest/services/Medically_Underserved_Areas_Population/FeatureServer/0/query?where=1%3D1&outFields=muap_index,srvc_area,ObjectId&returnGeometry=true&orderByFields=muap_index&outSR=4326&f=json
            var response = await _httpClient.GetAsync($"query?where={query}&outFields=muap_index,srvc_area,ObjectId&returnGeometry=true&orderByFields=muap_index&outSR=4326&f=json");

            var jsonString = await response.Content.ReadAsStringAsync();


            var muapResponse = JsonSerializer.Deserialize<MUAPObject>(jsonString, _options);

            return muapResponse;
        }
    }

    public class MUAPObject
    {
        public string ObjectIdFieldName { get; set; }
        public UniqueIdField UniqueIdField { get; set; }
        public string GlobalIdFieldName { get; set; }
        public GeometryProperties GeometryProperties { get; set; }
        public string GeometryType { get; set; }
        public SpatialReference SpatialReference { get; set; }
        public List<OutFields> Fields { get; set; }
        public List<FeatureSet> Features { get; set; }

    }

    public class UniqueIdField
    {
        public string Name { get; set; }
        public bool IsSystemMaintained { get; set; }
    }

    public class GeometryProperties
    {
        public string ShapeAreaFieldName { get; set; }
        public string ShapeLengthFieldName { get; set; }
        public string Units { get; set; }
    }
    public class SpatialReference
    {
        public int Wkid { get; set; }
        public int LatestWkid { get; set; }
    }

    public class OutFields
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Alias { get; set; }
        public string SQLType { get; set; }
        public string? Domain { get; set; }
        public string? DefaultValue { get; set; }
    }

    public class FeatureSet
    {
        public FeatureAttributes Attributes { get; set; }
        public FeatureGeometry Geometry { get; set; }
    }

    public class FeatureAttributes
    {
        public float Muap_index { get; set; }
        public string Srvc_Area { get; set; }
        public int ObjectId { get; set; }
    }

    public class FeatureGeometry
    {
        public float[][][] Rings { get; set; }
        public RingPolygons Rings2 { get; set; }
        public Polygon Polygon { get; set; }

        public void FlattenRings()
        {
            Polygon = new Polygon();
            Polygon.Coordinates = new List<CoordinatePoint>();
            foreach (var polygon in Rings)
            {
                
                foreach (var coordinatePoint in polygon)
                {
                    var point = new CoordinatePoint {
                        Latitude = coordinatePoint[1],
                        Longitude= coordinatePoint[0]
                    };

                    Polygon.Coordinates.Add(point);
                }
            }
        }
 
    }
    //list of polygons
    public class RingPolygons
    {
        public List<Polygon> InnerAndOuterPolygon { get; set; }
    }

    //polygon
    public class Polygon
    {
        

        //want this returned
        public List<CoordinatePoint> Coordinates { get; set; }
    }

    //individual lat, individual long
    public class CoordinatePoint
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

}

