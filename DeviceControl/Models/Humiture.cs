namespace DeviceControl.Models
{
    public class Humiture
    {
        public double? Temperature { get; set; }

        public double? Humidity { get; set; }

        public double? HeatIndex { get; set; }

        public double? AbsoluteHumidity { get; set; }

        public bool IsValid
            => Temperature.HasValue && Humidity.HasValue && HeatIndex.HasValue && AbsoluteHumidity.HasValue;
    }
}
