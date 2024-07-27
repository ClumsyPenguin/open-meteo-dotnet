using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;

namespace OpenMeteoTests
{
    [TestClass]
    public class OpenMeteoClientTests
    {
        [TestMethod]
        public void Weather_Codes_To_String_Tests()
        {
            var client = new OpenMeteoClient();
            int[] testWeatherCodes = { 0, 1, 2, 3, 51, 53, 96, 99, 100 };
            foreach (var weatherCode in testWeatherCodes)
            {
                var weatherCodeString = client.WeathercodeToString(weatherCode);
                Assert.IsInstanceOfType(weatherCodeString, typeof(string));

                switch (weatherCode)
                {
                    case 0:
                        Assert.AreEqual("Clear sky", weatherCodeString);
                        break;
                    case 100:
                        Assert.AreEqual("Invalid weathercode", weatherCodeString);
                        break;
                }
            }
        }
    }
}