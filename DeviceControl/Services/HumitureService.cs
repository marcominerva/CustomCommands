using DeviceControl.Models;
using Iot.Device.Common;
using Iot.Device.DHTxx;
using System;
using System.Device.Gpio;
using System.Threading.Tasks;
using UnitsNet;

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
            var humiture = new Humiture();
            var temperature = dht22.Temperature;
            var humidity = dht22.Humidity;

            if (!dht22.IsLastReadSuccessful)
            {
                // If the last read isn't successful, waits and then retries.
                await Task.Delay(2100);
                temperature = dht22.Temperature;
                humidity = dht22.Humidity;
            }

            if (dht22.IsLastReadSuccessful)
            {
                humiture.Temperature = Math.Round(temperature.Celsius, 2);
                humiture.Humidity = Math.Round(humidity, 2);

                var temperatureUnit = Temperature.FromDegreesCelsius(temperature.Celsius);
                var humidityRatio = Ratio.FromPercent(humidity);
                humiture.HeatIndex = Math.Round(WeatherHelper.CalculateHeatIndex(temperatureUnit, humidityRatio).DegreesCelsius, 2);
                humiture.AbsoluteHumidity = Math.Round(WeatherHelper.CalculateAbsoluteHumidity(temperatureUnit, humidityRatio).Value, 2);
            }

            return humiture;
        }
    }
}
