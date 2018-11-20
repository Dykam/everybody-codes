using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Search.Lib
{
    public class Camera
    {
        public int Number { get; }
        public string Name { get; }
        public Location Location { get; }

        static readonly Regex RawNameParseRegex = new Regex(@"^UTR-CM-(\d{3})\b", RegexOptions.Compiled);

        public Camera(int number, string name, Location location)
        {
            Number = number;
            Name = name;
            Location = location;
        }

        public void Deconstruct(out int number, out string name, out Location location)
        {
            number = Number;
            name = Name;
            location = Location;
        }

        public static Camera Parse(string rawName, string rawLatitude, string rawLongitude)
        {
            var rawNameMatch = RawNameParseRegex.Match(rawName);
            if (!rawNameMatch.Success)
                throw new ArgumentException("argument not a valid camera name. Name must fit UTR-CM-<id> <name>", nameof(rawName));

            var rawNumber = rawNameMatch.Groups[1].Value;

            if (!int.TryParse(rawNumber, out var number))
                throw new ArgumentException("argument not a valid camera name. Name must fit UTR-CM-<id> <name>.", nameof(rawName));

            if (!float.TryParse(rawLatitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var latitude))
                throw new ArgumentException("argument not a valid number.", nameof(rawLatitude));
            if (!float.TryParse(rawLongitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var longitude))
                throw new ArgumentException("argument not a valid number.", nameof(rawLongitude));

            return new Camera(number, rawName, (latitude, longitude));
        }

        public static implicit operator Camera((int number, string name, Location location) camera) =>
            new Camera(camera.number, camera.name, camera.location);

        public static implicit operator (int number, string name, Location location)(Camera camera) =>
            (camera.Number, camera.Name, camera.Location);
    }
}
