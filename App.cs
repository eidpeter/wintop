using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Terminal.Gui;
using Microsoft.Extensions.DependencyInjection;
using wintop.Widgets;
using wintop.Core;
using wintop.Core.Windows;

namespace wintop
{
    public class App
    {
        public static void Main(string[] args)
        {
            StartApp();
        }

        static Dialog AboutDialog()
        {
            var about = new Dialog("wintop: htop for the Windows Command Line", 60, 10, new Button("OK"));
            about.Add(
                new Label("Version: 0.3.0")
                {
                    X = 2,
                    Y = 1
                },
                new Label("Author: Peter Eid (https://github.com/EidPeter)")
                {
                    X = 2,
                    Y = 2
                },
                new Label("Licence: MIT")
                {
                    X = 2,
                    Y = 3
                }
            );
            return about;
        }

        public static void StartApp()
        {
            // Use injection to send the driver implementation to the core
            ServiceCollection serviceCollection = new ServiceCollection();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                serviceCollection.AddSingleton<ISystemInfo, WindowsInfo>();
            }
            else
            {
                // Other drivers not implemented yet
                throw new NotImplementedException("This wintop version only supports Windows. Linux & OSX will come.");
            }

            // Add the settings configuration
            serviceCollection.AddSingleton<Settings>();
            // Build the provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Get the settings for what can be done here
            var settings = new Settings();

            try
            {
                Application.Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - could not initialize the application. The inner exception is {ex}");
            }

            // Build the application UI with widgets
            var top = Application.Top;

            // Main color schema
            var mainColorScheme = new ColorScheme(){
                Normal = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor),
                Focus = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor),
                HotFocus = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor),
                HotNormal = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor),
                Disabled = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor),

            };
            // mainColorScheme.SetColorsForAllStates(settings.MainForegroundColor, settings.MainBackgroundColor);

            // top.ColorScheme = new ColorScheme() { Normal = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor) };

            // Creates the top-level window to show
            var win = new AppMainWindow(settings.MainAppTitle)
            {
                X = 0,
                Y = 0,

                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            win.ColorScheme = mainColorScheme;
            top.Add(win);

            var osInfoWidget = new SystemInfoWidget(serviceProvider)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Sized(3),
                CanFocus = false
            };

            win.Add(osInfoWidget);

            var systemTimeWidget = new SystemTimeWidget(serviceProvider)
            {
                X = 0,
                Y = 2,
                Width = Dim.Fill(),
                Height = Dim.Sized(3),
                CanFocus = false
            };

            win.Add(systemTimeWidget);

            var cpuRamWidget = new CpuRamWidget(serviceProvider)
            {
                X = 0,
                Y = Pos.Bottom(systemTimeWidget),
                Width = Dim.Percent(50),
                Height = Dim.Sized(18),
                CanFocus = false
            };

            win.Add(cpuRamWidget);

            var viewTopRight = new View()
            {
                X = Pos.Right(cpuRamWidget),
                Y = Pos.Bottom(systemTimeWidget),
                Width = Dim.Fill(),
                Height = Dim.Sized(18),
                CanFocus = false
            };

            win.Add(viewTopRight);

            var networkWidget = new NetworkWidget(serviceProvider)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Sized(7),
                CanFocus = false
            };

            var diskWidget = new DiskWidget(serviceProvider)
            {
                X = 0,
                Y = Pos.Bottom(networkWidget),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = false
            };

            viewTopRight.Add(networkWidget);
            viewTopRight.Add(diskWidget);

            var processListWidget = new ProcessListWidget(serviceProvider, new OrderProcessesByName())
            {
                X = 0,
                Y = Pos.Bottom(cpuRamWidget),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = false
            };

            win.Add(processListWidget);

            var nameOrdering = new OrderProcessesByName();
            var pidOrdering = new OrderProcessesByPID();
            var cpuOrdering = new OrderProcessesByCPU();
            var ramOrdering = new OrderProcessesByRAM();

            var statusBar = new StatusBar(new StatusItem[] {

                new StatusItem(Key.F1, "~F1~ About", () => Application.Run(AboutDialog())),
                new StatusItem(Key.F2, "~F2~ Sort by Name", () => processListWidget.SetOrdering(nameOrdering)),
                new StatusItem(Key.F3, "~F3~ Sort by PID", () => processListWidget.SetOrdering(pidOrdering)),
                new StatusItem(Key.F4, "~F4~ Sort by CPU", () => processListWidget.SetOrdering(cpuOrdering)),
                new StatusItem(Key.F5, "~F5~ Sort by RAM", () => processListWidget.SetOrdering(ramOrdering)),
                new StatusItem(Key.F6, "~F6~ Quit", () => Application.RequestStop())
            })
            {
                ColorScheme = new ColorScheme
                {
                    Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Green),
                }
            };

            top.Add(statusBar);


            // Refresh section. Every second, update on all listed widget will be called
            // Each seconds the UI refreshs, but a frequency can be set by overridind the property RefreshTimeSeconds for each widdget
            int tick = 0;
            var token = Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), (MainLoop) =>
            {
                // List all component to refresh
                Task.Run(async () =>
                {
                    await osInfoWidget.RefreshIfNeeded(MainLoop, tick);
                    await processListWidget.RefreshIfNeeded(MainLoop, tick);
                    await systemTimeWidget.RefreshIfNeeded(MainLoop, tick);
                    await cpuRamWidget.RefreshIfNeeded(MainLoop, tick);
                    await networkWidget.RefreshIfNeeded(MainLoop, tick);
                    await diskWidget.RefreshIfNeeded(MainLoop, tick);
                });
                tick++;
                // Every hour put it back to 0
                if (tick > 360) tick = 1;

                return true;
            });

            try
            {
                Application.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - could not launch the application. The inner exception is {ex}");
            }
        }

    }
}
