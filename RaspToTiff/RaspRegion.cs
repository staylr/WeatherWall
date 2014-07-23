using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaspToTiff
{
    public class RaspRegion
    {
        public int NumRows { get; internal set; }
        public int NumColumns { get; internal set; }
        public string Region { get; internal set; }
        public string GridId { get; internal set; }
        public double ResolutionKm { get; internal set; }

        /// <summary>
        /// Proj4 string for the projection.
        /// </summary>
        public string Projection { get; internal set; }

        /// <summary>
        /// WGS84 lat-lon corners of the region.
        /// </summary>
        public Tuple<double, double> LowerLeft { get; internal set; }
        public Tuple<double, double> LowerRight { get; internal set; }
        public Tuple<double, double> UpperLeft { get; internal set; }
        public Tuple<double, double> UpperRight { get; internal set; }

        internal void ReadCorners(string cornersFileName)
        {
            using (var cornerStream = new StreamReader(cornersFileName))
            {
                this.LowerLeft = ReadCornersLine(cornerStream);
                this.UpperLeft = ReadCornersLine(cornerStream);
                this.LowerRight = ReadCornersLine(cornerStream);                
                this.UpperRight = ReadCornersLine(cornerStream);
            }
        }

        private static Tuple<double, double> ReadCornersLine(StreamReader cornerStream)
        {
            var line = cornerStream.ReadLine();

            if (line != null)
            {
                var result = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (result.Length != 2)
                {
                    throw new ApplicationException("Invalid format for corner co-ordinate: " + line);
                }

                return new Tuple<double, double>(Double.Parse(result[1]), Double.Parse(result[0]));
            }
            else
            {
                return null;
            }
        }

    }
}
