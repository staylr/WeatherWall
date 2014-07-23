using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using OSGeo.GDAL;

namespace RaspToTiff
{
    public abstract class RaspData : IDisposable
    {
        public RaspRegion Region { get; protected set; }
        public string Description { get; protected set; }
        public string Day { get; protected set; }
        public DateTime LocalTime { get; protected set; }
        public TimeSpan UtcTime { get; protected set; }
        public string InitializationTime { get; protected set; }
        public string ForecastTime { get; protected set; }
        public double Multiple { get; protected set; }
        public string Parameter { get; protected set; }
        public string Unit { get; protected set; }
        public DataType Type { get; protected set; }

        public RaspData()
        {
            this.Region = new RaspRegion();
        }

        public static RaspData CreateRaspData(DataType type, string raspDataFile, string cornersFile)
        {
            RaspData data = null;

            switch (type)
            {
                case DataType.GDT_Byte:
                    data = new RaspData<byte>(type, raspDataFile, cornersFile);
                    break;

                case DataType.GDT_CInt16:
                case DataType.GDT_Int16:
                    data = new RaspData<short>(type, raspDataFile, cornersFile);
                    break;

                case DataType.GDT_CInt32:
                case DataType.GDT_Int32:
                    data = new RaspData<int>(type, raspDataFile, cornersFile);
                    break;

                case DataType.GDT_UInt16:
                    data = new RaspData<UInt16>(type, raspDataFile, cornersFile);
                    break;

                case DataType.GDT_UInt32:
                    data = new RaspData<UInt32>(type, raspDataFile, cornersFile);
                    break;

                case DataType.GDT_Float64:
                case DataType.GDT_CFloat64:
                    data = new RaspData<double>(type, raspDataFile, cornersFile);
                    break;

                case DataType.GDT_CFloat32:
                case DataType.GDT_Float32:
                    data = new RaspData<float>(type, raspDataFile, cornersFile);
                    break;
            }

            return data;
        }

        public abstract void ToGeoTiff(string fileName, bool generateWorldFile = false);

        public virtual void Dispose()
        {
        }
    }

    public class RaspData<T> : RaspData
        where T : struct, IEquatable<T>, IComparable<T>
    {
        public T Min { get; protected set; }
        public T Max { get; protected set; }

        private T[][] _Data;

        private string GridRegex = 
                        @"Model= RASP Region= (?<region>\w+) Grid= d\d\d? Reskm= (?<reskm>\d+(\.\d+)?) " +
                        @"Indexs= (?<x1>\d+) (?<cols>\d+) (?<y1>\d+) (?<rows>\d+) Proj= lambert (?<dx>\d+(\.\d+)?) " +
                        @"(?<dy>\d+(\.\d+)?) (?<lat1>-?\d+(\.\d+)?) (?<lat2>-?\d+(\.\d+)?) (?<lon1>-?\d+(\.\d+)?) " +
                        @"(?<lat0>-?\d+(\.\d+)?) (?<lon0>-?\d+(\.\d+)?)";

        private string DayRegex =
                        @"Day= (?<year>\d\d\d\d) (?<month>\d\d?) (?<day>\d\d?) (?<wday>\w\w\w) ValidLST= (?<lst>\d\d\d\d?) Local Time " +
                        @"ValidZ= (?<zt>\d\d\d\d?) Fcst= (?<fcst>\d+(\.\d+)?) Init= (?<init>\d+(\.\d+)?) " +
                        @"Param= (?<param>\S+) Unit= (?<unit>\S+) Mult= (?<mult>\d+(\.\d+)?) " +
                        @"Min= (?<min>\d+(\.\d+)?) Max= (?<max>\d+(\.\d+)?)";

        public RaspData() : base()
        {
        }

        public RaspData(DataType type, string fileName, string cornersFileName = null) : this()
        {
            this.Type = type;

            if (cornersFileName == null)
            {
                this.Region.LowerLeft = new Tuple<double, double>(0, 0);
            }
            else
            {
                this.Region.ReadCorners(cornersFileName);
            }

            this.ReadDataFile(fileName);
        }

        private void ReadDataFile(string fileName)
        {
            using (var stream = new StreamReader(fileName))
            {
                var firstLine = stream.ReadLine();

                if (firstLine != "---")
                {
                    throw new ApplicationException("First line not '---'");
                }

                this.Description = stream.ReadLine();
                if (String.IsNullOrEmpty(this.Description))
                {
                    throw new ApplicationException("Missing description");
                }

                ParseGridDescription(stream);
                ParseForecastTimeAndDataDescription(stream);

                ParseData(stream);
            }
        }

        private void ParseGridDescription(StreamReader stream)
        {
            string gridLine = stream.ReadLine();
            if (String.IsNullOrEmpty(gridLine))
            {
                throw new ApplicationException("Missing grid description");
            }

            var gridMatch = Regex.Match(gridLine, this.GridRegex);

            if (!gridMatch.Success)
            {
                throw new ApplicationException("Grid line does not match");
            }

            this.Region.ResolutionKm = Double.Parse(gridMatch.Groups["reskm"].Value);
            this.Region.NumColumns = Int32.Parse(gridMatch.Groups["cols"].Value);
            this.Region.NumRows = Int32.Parse(gridMatch.Groups["rows"].Value);

            double lat0 = Double.Parse(gridMatch.Groups["lat0"].Value);
            double lat1 = Double.Parse(gridMatch.Groups["lat1"].Value);
            double lat2 = Double.Parse(gridMatch.Groups["lat2"].Value);
            double lon0 = Double.Parse(gridMatch.Groups["lon0"].Value);
            double lon1 = Double.Parse(gridMatch.Groups["lon1"].Value);
            this.Region.Projection = this.LambertProjection(lat1, lat2, lon1, lat0, lon0);
        }

        private void ParseForecastTimeAndDataDescription(StreamReader stream)
        {
            string forecastLine = stream.ReadLine();
            if (String.IsNullOrEmpty(forecastLine))
            {
                throw new ApplicationException("Missing forecast description");
            }

            var dayMatch = Regex.Match(forecastLine, this.DayRegex);

            if (!dayMatch.Success)
            {
                throw new ApplicationException("Forecast day line does not match");
            }

            int year = Int32.Parse(dayMatch.Groups["year"].Value);
            int month = Int32.Parse(dayMatch.Groups["month"].Value);
            int day = Int32.Parse(dayMatch.Groups["day"].Value);

            int localTime = Int32.Parse(dayMatch.Groups["lst"].Value);
            DateTime localDate = new DateTime(year, month, day, localTime / 100,
                                            localTime % 100, 0, DateTimeKind.Local);

            int utcTime = Int32.Parse(dayMatch.Groups["zt"].Value);
            this.UtcTime = new TimeSpan(utcTime / 100, utcTime % 100, 0);

            this.Parameter = dayMatch.Groups["param"].Value;
            this.Unit = dayMatch.Groups["unit"].Value;
            this.Multiple = Double.Parse(dayMatch.Groups["mult"].Value);
            this.Min = this.ParseDataElement(dayMatch.Groups["min"].Value);
            this.Max = this.ParseDataElement(dayMatch.Groups["max"].Value);
        }

        private T ParseDataElement(string data)
        {
            if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
            {
                return (T)Convert.ChangeType(Double.Parse(data), typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(Int64.Parse(data), typeof(T));
            }
        }

        private void ParseData(StreamReader stream)
        {
            string line = null;
            this._Data = new T[this.Region.NumRows][];

            int row = 0;
            var dataSep = new char[] { ' ', '\t' };
            while ((line = stream.ReadLine()) != null)
            {
                var entries = line.Split(dataSep, StringSplitOptions.RemoveEmptyEntries);
                this._Data[row] = entries.Select(e => this.ParseDataElement(e)).ToArray();
                row++;
            }
        }

        public override void ToGeoTiff(string fileName, bool generateWorldFile = false)
        {
            string[] options = null;
            if (generateWorldFile)
            {
                options = new[] { "TFW=YES" };
            }

            var latlonProj = new ProjApi.Projection("+proj=latlong +a=6370000.0 +b=6370000.0 +ellps=sphere +no_defs");
            var proj = new ProjApi.Projection(this.Region.Projection);

            var x = new double[] { this.Region.LowerLeft.Item1 };
            var y = new double[] { this.Region.LowerLeft.Item2 };
            latlonProj.Transform(proj, x, y);
            
            Driver drv = Gdal.GetDriverByName("GTiff");

            using (var ds = drv.Create(fileName, this.Region.NumColumns, this.Region.NumRows, 1, this.Type, options))
            {
                SetProjection(ds);
                ds.SetDescription(this.Description);
                GeorereferenceDataSet(ds);
                WriteData(ds);
                ds.FlushCache();
            }
        }

        private void SetProjection(Dataset ds)
        {
            var spatialRef = new OSGeo.OSR.SpatialReference("");
            spatialRef.ImportFromProj4(this.Region.Projection);
            string wktProjection;
            spatialRef.ExportToWkt(out wktProjection);
            ds.SetProjection(wktProjection);
        }

        private void WriteData(Dataset ds)
        {
            int dataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[this.Region.NumRows * this.Region.NumColumns * dataSize];

            using (var stream = new MemoryStream(buffer))
            using (var writer = new BinaryWriter(stream))
            {
                foreach (var row in this._Data)
                {
                    foreach (var col in row)
                    {
                        var val = (dynamic)(T)col;
                        // Dynamic to force correct number of bytes.
                        writer.Write(val);
                    }
                }

                writer.Flush();
                stream.Flush();
            }

            ds.WriteRaster(0, 0, this.Region.NumColumns, this.Region.NumRows, buffer,
                    dataSize * this.Region.NumColumns, dataSize * this.Region.NumRows, 1, null, 0, 0, 0);
        }

        private void GeorereferenceDataSet(Dataset ds)
        {
            // XXX Revisit for regions that are not north-up - Alpine.
            double[] transform = new double[6];
            ds.GetGeoTransform(transform);

            // Width and height.
            transform[1] = this.Region.ResolutionKm * 1000;
            transform[5] = this.Region.ResolutionKm * 1000;

            // Top left corner of top left pixel.
            transform[0] = -((this.Region.NumColumns / 2) + 0.5) * this.Region.ResolutionKm * 1000;
            transform[3] = -((this.Region.NumRows / 2) + 0.5) * this.Region.ResolutionKm * 1000;

            ds.SetGeoTransform(transform);
        }

        public virtual string LambertProjection(double lat1, double lat2, double lon0,
                                                double lat_origin, double long_origin)
        {
            // WRF sphere - radius 6370k
            string proj4 = String.Format(
                        @"+proj=lcc +lat_1={0} +lat_2={1} +lon_1={2} +lat_0={3} +lon_0={4} +x_0=0 +y_0=0 +ellps=sphere +a=6370000.0 +b=6370000.0 +units=m +no_defs",
                        lat1, lat2, lon0, lat_origin, long_origin);
            return proj4;
        }
    }
}
