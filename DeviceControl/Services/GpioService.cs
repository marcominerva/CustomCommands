using System.Device.Gpio;
using System.Threading.Tasks;

namespace DeviceControl.Services
{
    public class GpioService
    {
        private readonly GpioController gpioController;

        public GpioService()
        {
            //gpioController = new GpioController(PinNumberingScheme.Board);
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
    }
}
