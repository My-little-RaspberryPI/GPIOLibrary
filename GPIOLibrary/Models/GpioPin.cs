namespace GPIOLibrary.Models
{
    public class GpioPin
    {
        private int _number = 0;
        /// <summary>
        /// GPIO number
        /// </summary>
        public int Number => _number;

        private string _classPath = string.Empty;
        private string _device => System.IO.Path.Combine(_classPath, $"gpio{_number}");
        private string _pointRegister => System.IO.Path.Combine(_classPath, "export");
        private string _pointFree => System.IO.Path.Combine(_classPath, "unexport");

        private bool _initialized = false;

        /// <summary>
        /// Pin direction
        /// </summary>
        public Enums.GpioDirection Direction
        {
            get
            {
                var _val = ReadProperty("direction");
                if (!GpioUtility.ParseDirection(_val, out var _direction))
                    return Enums.GpioDirection.Unknown;

                return _direction;
            }
            set
            {
                if (value == Enums.GpioDirection.Unknown)
                    return;

                WriteProperty("direction", value.ToString().ToLower());
            }
        }
        /// <summary>
        /// Pin state (/gpioN/value, if 1 will be true)
        /// </summary>
        public bool Value
        {
            get => ReadProperty("value")?.Trim() == "1";
            set => WriteProperty("value", (value) ? "1" : "0");
        }
        /// <summary>
        /// I'm just leave it here https://www.kernel.org/doc/Documentation/gpio/sysfs.txt
        /// </summary>
        public Enums.GpioEdge Edge
        {
            get
            {
                var _val = ReadProperty("edge");
                if (!GpioUtility.ParseEdge(_val, out var _edge))
                    return Enums.GpioEdge.None;

                return _edge;
            }
            set => WriteProperty("edge", value.ToString().ToLower());
        }
        /// <summary>
        /// You can write 'true' here to invert <see cref="Value"/>
        /// </summary>
        public bool ActiveLow
        {
            get => ReadProperty("active_low")?.Trim() == "1";
            set => WriteProperty("active_low", (value) ? "1" : "0");
        }

        /// <summary>
        /// Initialize GPIO pin
        /// </summary>
        /// <param name="number">GPIO number
        /// <para/>For example: GPIO17 (11 pin on PI 3b+ board) in wiringPi is 0.</param>
        public GpioPin(int number)
        {
            _number = number;
        }

        internal void Initialize(string classPath)
        {
            _classPath = classPath;

            if (System.IO.Directory.Exists(_device)) // if it already registered we can return ^v^
            {
                _initialized = true;
                return;
            }

            if (!System.IO.File.Exists(_pointRegister))
                throw new GpioPinInitializeException($"Pin register point not found '{_pointRegister}'.");

            System.IO.File.WriteAllText(_pointRegister, $"{_number}");
            System.Threading.Thread.Sleep(300); // wait for pin register complete
            if (!System.IO.Directory.Exists(_device))
                throw new GpioPinInitializeException("Cannot register pin.");

            _initialized = true;
        }

        internal void Free()
        {
            if (!_initialized || !System.IO.File.Exists(_pointFree))
                return;

            System.IO.File.WriteAllText(_pointFree, $"{_number}");
        }

        private string ReadProperty(string propName)
        {
            if (!_initialized)
                return string.Empty;

            var path = System.IO.Path.Combine(_device, propName);
            if (!System.IO.File.Exists(path))
                return string.Empty;

            return System.IO.File.ReadAllText(path);
        }

        private void WriteProperty(string propName, string value)
        {
            if (!_initialized)
                return;

            var path = System.IO.Path.Combine(_device, propName);
            if (!System.IO.File.Exists(path))
                return;

            System.IO.File.WriteAllText(path, value);
        }

        public override string ToString()
            => $"GPIO{Number} | direction={Direction}; edge={Edge}; value={Value}; active_low={ActiveLow}";
    }
}
