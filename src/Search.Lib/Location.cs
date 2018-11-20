namespace Search.Lib
{
    public struct Location
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public Location(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public void Deconstruct(out float latitude, out float longitude)
        {
            latitude = Latitude;
            longitude = Longitude;
        }

        public static implicit operator Location((float latitude, float longitude) location) =>
            new Location(location.latitude, location.longitude);

        public static implicit operator (float latitude, float longitude)(Location location) =>
            (location.Latitude, location.Longitude);
    }
}
