using DeviceControl.Models;
using Iot.Device.DHTxx;
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

        public Task<Humiture> GetHumitureAsync()
        {
            var result = new Humiture();
            var temperature = dht22.Temperature;
            var humidity = dht22.Humidity;

            if (dht22.IsLastReadSuccessful)
            {
                result.IsValid = true;
                result.Temperature = temperature.Celsius;
                result.Humidity = humidity;
            }

            return Task.FromResult(result);
        }
    }
}
