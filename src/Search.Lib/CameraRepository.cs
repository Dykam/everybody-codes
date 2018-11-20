using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Nito.AsyncEx;

namespace Search.Lib
{
    public class CameraRepository : ICameraRepository
    {
        private AsyncReaderWriterLock locker;
        private IDictionary<int, Camera> byNumber;
        private IDictionary<string, Camera> byName;

        private IList<Camera> cameras;

        public CameraRepository()
        {
            locker = new AsyncReaderWriterLock();

            byNumber = new Dictionary<int, Camera>();
            byName = new Dictionary<string, Camera>();
            cameras = new List<Camera>();
        }

        public async Task Add(Camera camera)
        {
            using (await locker.WriterLockAsync())
            {
                if (byNumber.ContainsKey(camera.Number))
                    throw new ArgumentException("camera number already exists in the repository", nameof(camera));
                if (byName.ContainsKey(camera.Name))
                    throw new ArgumentException("camera name already exists in the repository", nameof(camera));

                byNumber.Add(camera.Number, camera);
                byName.Add(camera.Name, camera);
                cameras.Add(camera);
            }
        }

        public async Task Remove(Camera camera)
        {
            using (await locker.WriterLockAsync())
            {
                if (!byNumber.ContainsKey(camera.Number))
                    throw new ArgumentException("camera number does not exists in the repository", nameof(camera));

                if (byNumber[camera.Number] != camera)
                    throw new ArgumentException("camera does not exists in the repository", nameof(camera));

                byNumber.Remove(camera.Number);
                byName.Remove(camera.Name);
                cameras.Remove(camera);
            }
        }

        public Camera this[int number]
        {
            get
            {
                using (locker.ReaderLock())
                {
                    return byNumber[number];
                }
            }
        }

        public Camera this[string name]
        {
            get
            {
                using (locker.ReaderLock())
                {
                    return byName[name];
                }
            }
        }

        public async Task<Camera> Get(int number)
        {
            using (await locker.ReaderLockAsync())
            {
                return byNumber[number];
            }
        }

        public async Task<Camera> Get(string name)
        {
            using (await locker.ReaderLockAsync())
            {
                return byName[name];
            }
        }

        public async Task<IEnumerable<Camera>> Search(string partialName)
        {

            using (await locker.ReaderLockAsync())
            {
                var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
                return cameras
                    .Where(camera => compareInfo.IndexOf(camera.Name, partialName, CompareOptions.IgnoreCase) >= 0)
                    .ToArray();
            }
        }

        public ICameraRepositorySnapshot GetSnapshot()
        {
            return new CameraRepositorySnapshot(cameras.ToArray());
        }

        public static async Task<CameraRepository> FromCsv(string fileName)
        {
            var csv = new CsvReader(new StreamReader(File.OpenRead(fileName)), new Configuration
            {
                Delimiter = ";"
            });

            if (!await csv.ReadAsync())
            {
                throw new Exception("No CSV header found. Is the file empty?");
            }
            csv.ReadHeader();

            var repository = new CameraRepository();
            var index = 0;
            while (await csv.ReadAsync())
            {
                try
                {
                    var camera = Camera.Parse(csv["Camera"], csv["Latitude"], csv["Longitude"]);
                    await repository.Add(camera);
                }
                catch (ArgumentException e) when (e.ParamName != "camera")
                {
                    await Console.Error.WriteLineAsync($"Malformed camera record encountered at index {index}");
                }
                catch (ArgumentException e) when (e.ParamName == "camera")
                {
                    await Console.Error.WriteLineAsync($"Duplicate camera record encountered at index {index}");
                }
                catch (CsvHelper.MissingFieldException)
                {
                    await Console.Error.WriteLineAsync($"Malformed camera record encountered at index {index}");
                }
                index++;
            }

            return repository;
        }

        class CameraRepositorySnapshot : ICameraRepositorySnapshot
        {
            public CameraRepositorySnapshot(IEnumerable<Camera> snapshot)
            {
                Snapshot = snapshot;
            }

            public IEnumerable<Camera> Snapshot { get; }

            public IEnumerator<Camera> GetEnumerator()
            {
                return Snapshot.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
