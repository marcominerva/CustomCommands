using DeviceControl.Models;
using Iot.Device.DHTxx;
using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace DeviceControl.Services
{
    public class HumitureService
    {
        private const int pinNumber = 7;
        private readonly Dht22 dht22;

        public HumitureService()
        {
            dht22 = new Dht22(pinNumber, PinNumberingScheme.Board);
        }

        public async Task<Humiture> GetHumitureAsync()
        {
            var result = new Humiture();
            var temperature = dht22.Temperature;
            var humidity = dht22.Humidity;

            if (!dht22.IsLastReadSuccessful)
            {
                // If the last read isn't successfull, waits and then retries.
                await Task.Delay(2100);
                temperature = dht22.Temperature;
                humidity = dht22.Humidity;
            }

            if (dht22.IsLastReadSuccessful)
            {
                result.IsValid = true;
                result.Temperature = Math.Round(temperature.DegreesCelsius, 2);
                result.Humidity = Math.Round(humidity, 2);
                //result.HeatIndex = WeatherHelper.CalculateHeatIndex(temperature, humidity).DegreesCelsius;
                //result.AbsoluteHumidity = WeatherHelper.CalculateAbsoluteHumidity(temperature, humidity);
            }

            return result;
        }
    }
}
