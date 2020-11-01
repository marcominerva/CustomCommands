﻿namespace DeviceControl.Commands
{
    public class PinCommand
    {
        public int PinNumber { get; set; }

        public PinValue Value { get; set; }
    }

    public enum PinValue
    {
        Low,
        High
    };
}
