using System;
using System.IO;
using System.Threading.Tasks;
using NDesk.Options;
using Search.Lib;

namespace Search
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = null;
            bool showHelp = false;
            var p = new OptionSet {
                { "n|name=", n => name = n },
                { "h|help", h => showHelp = h != null }
            };

            try
            {
                var extra = p.Parse(args);
                if(extra.Count != 0) {
                    showHelp = true;
                }
            }
            catch (OptionException e)
            {
                Console.Write("Search: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `Search --help' for more information.");
            }

            if (showHelp)
            {
                Console.WriteLine("Usage: Search [OPTIONS]+ message");
                p.WriteOptionDescriptions(Console.Out);
            }

            Run(name).Wait();
        }

        private static async Task Run(string name)
        {
            var repository = await CameraRepository.FromCsv("../../data/cameras-defb.csv");

            foreach(var camera in repository.Search(name))
            {
                Console.WriteLine($"{camera.Number} | {camera.Name} | {camera.Location.lat} | {camera.Location.lng}");
            }
        }
    }
}
