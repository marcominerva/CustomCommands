using DeviceControl.Commands;
using System.Device.Gpio;
using System.Threading.Tasks;
using PinValue = System.Device.Gpio.PinValue;

namespace DeviceControl.Services
{
    public class GpioService
    {
        public GpioController GpioController { get; }

        public GpioService()
        {
            GpioController = new GpioController(PinNumberingScheme.Board);
        }

        public Task SetPinAsync(int pinNumber, PinValue value)
        {
            if (!GpioController.IsPinOpen(pinNumber))
            {
                GpioController.OpenPin(pinNumber, PinMode.Output);
            }

            GpioController.Write(pinNumber, value);

            return Task.CompletedTask;
        }

        public Task SetPinAsync(PinCommand command)
            => SetPinAsync(command.PinNumber, command.Value == Commands.PinValue.High ? PinValue.High : PinValue.Low);
    }
}
