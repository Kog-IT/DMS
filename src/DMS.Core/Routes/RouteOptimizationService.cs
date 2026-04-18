using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Routes;

/// <summary>
/// Pure domain service: no DB access, no ABP dependencies.
/// Accepts in-memory data, returns an optimized ordering.
/// </summary>
public class RouteOptimizationService : ITransientDependency
{
    /// <summary>Input model for each stop passed in from the app service.</summary>
    public class StopInput
    {
        public int RouteItemId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? PlannedDurationMinutes { get; set; }
    }

    /// <summary>Output model for each stop returned from Optimize.</summary>
    public class StopResult
    {
        public int RouteItemId { get; set; }
        public int OrderIndex { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double DistanceFromPreviousKm { get; set; }
        public int PlannedDurationMinutes { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
    }

    /// <summary>
    /// Reorders stops using nearest-neighbor starting from the rep's location.
    /// Stops without GPS coordinates are appended at the end in their original order.
    /// </summary>
    public List<StopResult> Optimize(
        IEnumerable<StopInput> stops,
        double repLatitude,
        double repLongitude,
        DateTime startTime,
        int defaultVisitDurationMinutes,
        double averageTravelSpeedKmh)
    {
        var stopList = stops.ToList();
        var withGps = stopList.Where(s => s.Latitude.HasValue && s.Longitude.HasValue).ToList();
        var withoutGps = stopList.Where(s => !s.Latitude.HasValue || !s.Longitude.HasValue).ToList();

        var ordered = new List<StopResult>();
        var unvisited = withGps.ToList();

        double currentLat = repLatitude;
        double currentLon = repLongitude;
        DateTime currentTime = startTime;
        int orderIndex = 0;

        while (unvisited.Count > 0)
        {
            var nearest = unvisited
                .OrderBy(s => HaversineDistance(currentLat, currentLon, s.Latitude.Value, s.Longitude.Value))
                .First();

            var distKm = HaversineDistance(currentLat, currentLon, nearest.Latitude.Value, nearest.Longitude.Value);
            var travelMinutes = averageTravelSpeedKmh > 0
                ? (distKm / averageTravelSpeedKmh) * 60.0
                : 0;

            var visitDuration = nearest.PlannedDurationMinutes ?? defaultVisitDurationMinutes;

            ordered.Add(new StopResult
            {
                RouteItemId = nearest.RouteItemId,
                OrderIndex = orderIndex,
                CustomerId = nearest.CustomerId,
                CustomerName = nearest.CustomerName,
                CustomerAddress = nearest.CustomerAddress,
                Latitude = nearest.Latitude,
                Longitude = nearest.Longitude,
                DistanceFromPreviousKm = Math.Round(distKm, 3),
                PlannedDurationMinutes = visitDuration,
                EstimatedArrivalTime = currentTime.AddMinutes(travelMinutes)
            });

            currentTime = currentTime.AddMinutes(travelMinutes + visitDuration);
            currentLat = nearest.Latitude.Value;
            currentLon = nearest.Longitude.Value;
            unvisited.Remove(nearest);
            orderIndex++;
        }

        // Append stops without GPS at the end, preserving their original relative order
        foreach (var stop in withoutGps)
        {
            var visitDuration = stop.PlannedDurationMinutes ?? defaultVisitDurationMinutes;

            ordered.Add(new StopResult
            {
                RouteItemId = stop.RouteItemId,
                OrderIndex = orderIndex,
                CustomerId = stop.CustomerId,
                CustomerName = stop.CustomerName,
                CustomerAddress = stop.CustomerAddress,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
                DistanceFromPreviousKm = 0,
                PlannedDurationMinutes = visitDuration,
                EstimatedArrivalTime = currentTime
            });

            currentTime = currentTime.AddMinutes(visitDuration);
            orderIndex++;
        }

        return ordered;
    }

    /// <summary>Computes great-circle distance in km between two GPS coordinates.</summary>
    public static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0; // Earth radius in km
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
