namespace Walkies.API.Services
{
    /// <summary>
    /// Provides distance calculation for geo-location-based search
    /// functionality. Uses the Haversine formula to calculate great
    /// circle distance between two points on the Earth's surface.
    /// </summary>
    public class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371.0; // Average radius of the Earth in kilometers

        /// <summary>
        /// Calculates the great circle distance between two points
        /// using the haversine formula.
        /// </summary>
        /// <param name="lat1">Latitude of the first point in degrees</param>
        /// <param name="lon1">Longitude of the first point in degrees</param>
        /// <param name="lat2">Latitude of the second point in degrees</param>
        /// <param name="lon2">Longitude of the second point in degrees</param>
        /// <returns>The distance between the two points in kilometers</returns>
        public static double Calculate(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Angle in radians</returns>
        private static double ToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }
    }
}