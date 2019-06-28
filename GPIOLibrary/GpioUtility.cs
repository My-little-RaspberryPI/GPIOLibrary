using System.Collections.Generic;

namespace GPIOLibrary
{
    public static class GpioUtility
    {
        public static class RaspberryPi3 // I'm lazy to desigin it for other PI's. Have only 3b+
        {
            // pin - key, gpioN - value
            private static Dictionary<int, int> Pins = new Dictionary<int, int>()
            {
                { 3, 2 },
                { 5, 3 },
                { 7, 4 },   { 8, 14 },
                            { 10, 15 },
                { 11, 17 }, { 12, 18 },
                { 13, 27 },
                { 15, 22 }, { 16, 23 },
                            { 18, 24 },
                { 19, 10 },
                { 21, 9 },  { 22, 25 },
                { 23, 11 }, { 24, 8 },
                            { 26, 7 },
                { 29, 5 },
                { 31, 6 },  { 32, 12 },
                { 33, 13 },
                { 35, 19 }, { 36, 16 },
                { 37, 26 }, { 38, 20 },
                            { 40, 21 },
            };

            public static int GpioByPin(int pin)
                => Pins.ContainsKey(pin) ? Pins[pin] : -1;
        }

        public static bool ParseEdge(string s, out Enums.GpioEdge edge)
        {
            edge = Enums.GpioEdge.None;
            switch (s?.ToLower()?.Trim() ?? "")
            {
                case "rising": edge = Enums.GpioEdge.Rising; break;
                case "falling": edge = Enums.GpioEdge.Falling; break;
                case "both": edge = Enums.GpioEdge.Both; break;
                default: return false;
            }

            return true;
        }

        public static bool ParseDirection(string s, out Enums.GpioDirection direction)
        {
            direction = Enums.GpioDirection.Unknown;
            switch (s?.ToLower()?.Trim() ?? "")
            {
                case "in": direction = Enums.GpioDirection.In; break;
                case "out": direction = Enums.GpioDirection.Out; break;
                default: return false;
            }

            return true;
        }
    }
}
