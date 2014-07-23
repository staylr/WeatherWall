using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WeatherWall.Tests
{
    [TestClass]
    [DeploymentItem("wstar.data")]
    public class RaspDataTest
    {
        [TestMethod]
        public void RaspToTiff()
        {
            using (var data = new RaspToTiff.RaspData<UInt16>(OSGeo.GDAL.DataType.GDT_UInt16,
                                            "wstar.data", "victoria.corners.d02.dat"))
            {
                data.ToGeoTiff("wstar.tif", true);
            }
        }

        public RaspDataTest()
        {
            GdalConfiguration.ConfigureGdal();
        }
    }
}
