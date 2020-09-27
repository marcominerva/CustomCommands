using DeviceControl.Models;
using System.Device.Gpio;
using System.Threading.Tasks;
using PinValue = System.Device.Gpio.PinValue;

namespace DeviceControl.Services
{
    public class GpioService
    {
        private readonly GpioController gpioController;

        public GpioService()
        {
            gpioController = new GpioController(PinNumberingScheme.Board);
        }

        public Task SetPinAsync(int pinNumber, PinValue value)
        {
            if (!gpioController.IsPinOpen(pinNumber))
            {
                gpioController.OpenPin(pinNumber, PinMode.Output);
            }

            gpioController.Write(pinNumber, value);

            return Task.CompletedTask;
        }

        public Task SetPinAsync(PinCommand command)
            => SetPinAsync(command.PinNumber, command.Value == Models.PinValue.High ? PinValue.High : PinValue.Low);
    }
}
