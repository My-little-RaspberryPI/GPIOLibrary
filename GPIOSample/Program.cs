using System;
using System.Linq;

namespace GPIOSample
{
    class Program
    {
        static GPIOLibrary.GpioManager GpioManager = new GPIOLibrary.GpioManager();

        static void Main(string[] args)
        {
            var shouldWork = true;
            while (shouldWork)
            {
                var cmd = AskCmd("> ", ConsoleColor.Cyan, ConsoleColor.Gray).ToLower();
                switch (cmd)
                {
                    case "quit":
                    case "qqq":
                    case "exit":
                        PrintLn("Bye!", ConsoleColor.Green);
                        shouldWork = false;
                        return;
                    case "list":
                        OnList();
                        break;
                    case "add":
                        OnAdd();
                        break;
                    case "rm":
                        OnRm();
                        break;
                    case "set":
                        OnSet();
                        break;
                    case "help":
                        PrintLn("quit", ConsoleColor.Magenta);
                        PrintLn("qqq", ConsoleColor.Magenta);
                        Print("exit", ConsoleColor.Magenta); Print(" - "); PrintLn("Stop execution of this test app", ConsoleColor.Cyan);

                        Console.WriteLine();

                        Print("list", ConsoleColor.Magenta); Print(" - "); PrintLn("Active GPIO pins", ConsoleColor.Cyan);

                        Console.WriteLine();

                        Print("add", ConsoleColor.Magenta); Print(" - "); PrintLn("Register GPIO pin", ConsoleColor.Cyan);

                        Console.WriteLine();

                        Print("rm", ConsoleColor.Magenta); Print(" - "); PrintLn("Remove GPIO pin", ConsoleColor.Cyan);

                        Console.WriteLine();

                        Print("set", ConsoleColor.Magenta); Print(" - "); PrintLn("Update GPIO pin values", ConsoleColor.Cyan);

                        Console.WriteLine();

                        Print("help", ConsoleColor.Magenta); Print(" - "); PrintLn("this message", ConsoleColor.Cyan);
                        break;
                    default:
                        break;
                }
            }
        }

        static void OnList()
        {
            GpioManager.RefreshPins();

            for (int i = 0; i < GpioManager.Registered.Count; i++)
            {
                var pin = GpioManager.Registered[i];

                Print($"{i + 1}. ", ConsoleColor.Cyan);
                PrintLn($"{pin}", ConsoleColor.Magenta);
            }
        }

        static void OnAdd()
        {
            if (!int.TryParse(Ask("GPIO number", ConsoleColor.Cyan, ConsoleColor.Gray), out var number))
            {
                PrintLn("I need number. Try again!", ConsoleColor.Red);
                return;
            }

            if (number < 0)
            {
                PrintLn("Pin number cannot be less that 0 (i think...)", ConsoleColor.Red);
                return;
            }

            if (!GpioManager.TryRegisterPin(number, out var pin))
            {
                PrintLn($"Failed to register GPIO{number} pin!", ConsoleColor.Red);
                return;
            }

            PrintLn($"Success! {pin}", ConsoleColor.Green);
        }

        static void OnRm()
        {
            if (!int.TryParse(Ask("GPIO number", ConsoleColor.Cyan, ConsoleColor.Gray), out var number))
            {
                PrintLn("I need number. Try again!", ConsoleColor.Red);
                return;
            }

            if (number < 0)
            {
                PrintLn("Pin number cannot be less that 0 (i think...)", ConsoleColor.Red);
                return;
            }

            var pin = GpioManager.Registered.FirstOrDefault(x => x.Number == number);
            if (pin == null)
            {
                PrintLn($"GPIO{number} pin not found.", ConsoleColor.Red);
                return;
            }

            if (!GpioManager.UnRegisterPin(pin))
            {
                PrintLn($"Failed to remove GPIO{number} pin!", ConsoleColor.Red);
                return;
            }

            PrintLn($"GPIO{number} successfully removed!", ConsoleColor.Green);
        }

        static void OnSet()
        {
            if (!int.TryParse(AskCmd("set> GPIO number: ", ConsoleColor.Cyan, ConsoleColor.Gray), out var number))
            {
                PrintLn("I need number. Try again!", ConsoleColor.Red);
                return;
            }

            if (number < 0)
            {
                PrintLn("Pin number cannot be less that 0 (i think...)", ConsoleColor.Red);
                return;
            }

            var pin = GpioManager.Registered.FirstOrDefault(x => x.Number == number);
            if (pin == null)
            {
                PrintLn($"GPIO{number} pin not found.", ConsoleColor.Red);
                return;
            }

            var cmd = AskCmd($"set GPIO{number} (value|edge|direction|active_low)> ", ConsoleColor.Cyan, ConsoleColor.Gray).ToLower();
            switch (cmd)
            {
                case "v":
                case "value":
                    {
                        var subCmd = AskCmd($"set GPIO{number} {cmd} (true|false)> ", ConsoleColor.Cyan, ConsoleColor.Gray);
                        if (!bool.TryParse(subCmd.Trim(), out var state))
                        {
                            PrintLn("Not recognized. Try again.", ConsoleColor.Red);
                            return;
                        }

                        pin.Value = state;
                    }
                    break;
                case "e":
                case "edge":
                    {
                        var subCmd = AskCmd($"set GPIO{number} {cmd} (none|rising|falling|both)> ", ConsoleColor.Cyan, ConsoleColor.Gray);
                        if (!GPIOLibrary.GpioUtility.ParseEdge(subCmd, out var edge))
                        {
                            PrintLn("Not recognized. Try again.", ConsoleColor.Red);
                            return;
                        }

                        pin.Edge = edge;
                    }
                    break;
                case "d":
                case "direction":
                    {
                        var subCmd = AskCmd($"set GPIO{number} {cmd} (in|out)> ", ConsoleColor.Cyan, ConsoleColor.Gray);
                        if (!GPIOLibrary.GpioUtility.ParseDirection(subCmd, out var direction))
                        {
                            PrintLn("Not recognized. Try again.", ConsoleColor.Red);
                            return;
                        }

                        pin.Direction = direction;
                    }
                    break;
                case "a":
                case "al":
                case "active_low":
                    {
                        var subCmd = AskCmd($"set GPIO{number} {cmd} (true|false)> ", ConsoleColor.Cyan, ConsoleColor.Gray);
                        if (!bool.TryParse(subCmd.Trim(), out var state))
                        {
                            PrintLn("Not recognized. Try again.", ConsoleColor.Red);
                            return;
                        }

                        pin.ActiveLow = state;
                    }
                    break;
                default:
                    PrintLn($"Unknown sub-command \"{cmd}\".");
                    return;
            }

            PrintLn($"Pin new state: {pin}");
        }

        static void Print(string message)
            => Print(message, Console.ForegroundColor);
        static void Print(string message, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(message);

            Console.ForegroundColor = oldColor;
        }
        static void PrintLn(string message)
            => PrintLn(message, Console.ForegroundColor);
        static void PrintLn(string message, ConsoleColor color)
            => Print($"{message}{Environment.NewLine}", color);

        static string Ask(string message)
            => Ask(message, Console.ForegroundColor, Console.ForegroundColor);
        static string Ask(string message, ConsoleColor request, ConsoleColor response)
            => AskCmd($"[{message}]: ", request, response);

        static string AskCmd(string message, ConsoleColor request, ConsoleColor response)
        {
            Print(message, request);
            var oldCol = Console.ForegroundColor;
            Console.ForegroundColor = response;
            var resp = Console.ReadLine();
            Console.ForegroundColor = oldCol;
            return resp;
        }
    }
}
