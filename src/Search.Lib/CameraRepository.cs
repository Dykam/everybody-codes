using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Search.Lib
{
    public class CameraRepository
    {
        private IDictionary<int, Camera> byNumber;
        private IDictionary<string, Camera> byName;

        private IList<Camera> cameras;

        public CameraRepository()
        {
            byNumber = new Dictionary<int, Camera>();
            byName = new Dictionary<string, Camera>();
            cameras = new List<Camera>();
        }

        public void Add(Camera camera)
        {
            if (byNumber.ContainsKey(camera.Number))
                throw new ArgumentException("camera number already exists in the repository", nameof(camera));
            if (byName.ContainsKey(camera.Name))
                throw new ArgumentException("camera name already exists in the repository", nameof(camera));

            byNumber.Add(camera.Number, camera);
            byName.Add(camera.Name, camera);
            cameras.Add(camera);
        }

        public void Remove(Camera camera)
        {
            if (!byNumber.ContainsKey(camera.Number))
                throw new ArgumentException("camera number does not exists in the repository", nameof(camera));

            if (byNumber[camera.Number] != camera)
                throw new ArgumentException("camera does not exists in the repository", nameof(camera));

            byNumber.Remove(camera.Number);
            byName.Remove(camera.Name);
            cameras.Remove(camera);
        }

        public Camera this[int number] => byNumber[number];

        public Camera this[string name] => byName[name];

        public IEnumerable<Camera> Search(string partialName)
        {
            var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            return cameras.Where(camera => compareInfo.IndexOf(camera.Name, partialName, CompareOptions.IgnoreCase) >= 0);
        }
    }
}
