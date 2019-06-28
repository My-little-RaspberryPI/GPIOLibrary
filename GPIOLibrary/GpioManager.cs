using GPIOLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace GPIOLibrary
{
    public class GpioManager : IDisposable
    {
        private const string GPIO_CLASS_PATH = "/sys/class/gpio"; // default at least for debian 9 :p

        private List<GpioPin> Pins = new List<GpioPin>();
        public ReadOnlyCollection<GpioPin> Registered => new ReadOnlyCollection<GpioPin>(Pins);

        /// <summary>
        /// path/to/gpio
        /// <para/>Default: /sys/class/gpio
        /// </summary>
        public string ClassPath { get; set; } = GPIO_CLASS_PATH;

        public GpioManager()
        {
            RefreshPins();
        }

        /// <summary>
        /// Get currently registered pins
        /// </summary>
        public void RefreshPins()
        {
            Pins.Clear();

            if (!System.IO.Directory.Exists(ClassPath))
                return;

            var folders = System.IO.Directory.GetDirectories(ClassPath);
            foreach (var folder in folders)
            {
                var regex = Regex.Match(folder ?? "", @"gpio(\d+)");
                if (!regex.Success)
                    continue;

                var number = int.Parse(regex.Groups[1].Value);
                TryRegisterPin(number, out var pin);
            }
        }

        /// <summary>
        /// Set to 'false' if you need unregistering pins at dispose
        /// </summary>
        public bool NoDisposeExecution { get; set; } = true;
        public void Dispose()
        {
            if (NoDisposeExecution)
                return;

            // unregister all pins at shutdown
            foreach (var pin in Pins)
            {
                pin.Free();
            }
        }

        /// <summary>
        /// Registering pin by number
        /// </summary>
        /// <param name="number">GPIO number
        /// <para/>For example: GPIO17 (11 pin on PI 3b+ board) in wiringPi is 0.</param>
        /// <returns>Pin object</returns>
        /// <exception cref="GpioPinInitializeException"/>
        public GpioPin RegisterPin(int number)
        {
            var _registeredPin = Pins.FirstOrDefault(x => x.Number == number);
            if (_registeredPin != null)
                return _registeredPin;

            var pin = new GpioPin(number);
            pin.Initialize(ClassPath);
            Pins.Add(pin);

            return pin;
        }

        /// <summary>
        /// Registering pin by number
        /// </summary>
        /// <param name="number">GPIO number</param>
        /// <param name="pin">Pin object</param>
        /// <returns>'true' if pin registered successfully</returns>
        public bool TryRegisterPin(int number, out GpioPin pin)
        {
            pin = null;
            try
            {
                pin = RegisterPin(number);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Removes pin registration
        /// </summary>
        /// <param name="pin">GPIO pin object</param>
        /// <returns>'true' if unregistering success</returns>
        public bool UnRegisterPin(GpioPin pin)
        {
            if (pin == null)
                return false;

            pin.Free();
            return Pins.Remove(pin);
        }
    }
}
