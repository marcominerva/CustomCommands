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

        private Humiture lastHumitureRead;

        public HumitureService()
        {
            dht22 = new Dht22(pinNumber, PinNumberingScheme.Board);
        }

        public Task<Humiture> GetHumitureAsync()
        {
            var temperature = dht22.Temperature;
            var humidity = dht22.Humidity;

            if (dht22.IsLastReadSuccessful)
            {
                lastHumitureRead = new Humiture
                {
                    Temperature = Math.Round(temperature.Celsius, 2),
                    Humidity = Math.Round(humidity, 2)
                };

                var temperatureUnit = Temperature.FromDegreesCelsius(temperature.Celsius);
                var humidityRatio = Ratio.FromPercent(humidity);
                lastHumitureRead.HeatIndex = Math.Round(WeatherHelper.CalculateHeatIndex(temperatureUnit, humidityRatio).DegreesCelsius, 2);
                lastHumitureRead.AbsoluteHumidity = Math.Round(WeatherHelper.CalculateAbsoluteHumidity(temperatureUnit, humidityRatio).Value, 2);
            }

            return Task.FromResult(lastHumitureRead);
        }
    }
}
