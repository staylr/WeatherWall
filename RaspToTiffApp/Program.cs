using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OSGeo.GDAL;

namespace RaspToTiff
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.Error.WriteLine("Usage: RaspToTiff dataType raspDataFile cornersFile tiffFile");
                Environment.Exit(1);
            }

            string dataType = args[0];
            string raspDataFile = args[1];
            string cornersFile = args[2];
            string tiffFile = args[3];

            var type = (OSGeo.GDAL.DataType)Enum.Parse(typeof(OSGeo.GDAL.DataType), dataType);
            
            using (var data = RaspData.CreateRaspData(type, raspDataFile, cornersFile))
            {
                data.ToGeoTiff(tiffFile, true);
            }
        }
    }
}
