using DeviceControl.Models;
using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace DeviceControl.Services
{
    public class LedService
    {
        private const int redPinNumber = 18;
        private const int greenPinNumber = 20;
        private const int bluePinNumber = 22;

        private readonly GpioService gpioService;

        public LedService(GpioService gpioService)
        {
            this.gpioService = gpioService;
        }

        public async Task SetColorAsync(LedCommand command)
        {
            // Turn off all the pins (i.e., it turns off the led).
            await gpioService.SetPinAsync(redPinNumber, PinValue.Low);
            await gpioService.SetPinAsync(greenPinNumber, PinValue.Low);
            await gpioService.SetPinAsync(bluePinNumber, PinValue.Low);

            // Check which color (i.e. pin) to enable.
            int? pin = command.Color?.ToLower() switch
            {
                "red" => redPinNumber,
                "green" => greenPinNumber,
                "blue" => bluePinNumber,
                "black" => null,
                null => null,
                _ => throw new NotSupportedException()
            };

            if (pin.HasValue)
            {
                await gpioService.SetPinAsync(pin.Value, PinValue.High);
            }
        }
    }
}
