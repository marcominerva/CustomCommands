using DeviceControl.Models;
using Iot.Device.Common;
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

        private Humiture lastHumitureRead;

        public HumitureService(GpioService gpioService)
        {
            dht22 = new Dht22(pinNumber, PinNumberingScheme.Board, gpioService.GpioController, false);
        }

        public Task<Humiture> GetHumitureAsync()
        {
            var temperature = dht22.Temperature;
            var humidity = dht22.Humidity;

            if (dht22.IsLastReadSuccessful)
            {
                lastHumitureRead = new Humiture
                {
                    Temperature = Math.Round(temperature.DegreesCelsius, 2),
                    Humidity = Math.Round(humidity.Percent, 2)
                };

                lastHumitureRead.HeatIndex = Math.Round(WeatherHelper.CalculateHeatIndex(temperature, humidity).DegreesCelsius, 2);
                lastHumitureRead.AbsoluteHumidity = Math.Round(WeatherHelper.CalculateAbsoluteHumidity(temperature, humidity).GramsPerCubicMeter, 2);
            }

            return Task.FromResult(lastHumitureRead);
        }
    }
}
