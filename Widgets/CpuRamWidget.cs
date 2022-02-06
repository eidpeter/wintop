using System;
using System.Threading.Tasks;
using Terminal.Gui;
using wintop.Widgets.Common;
using wintop.Common.Helpers;

namespace wintop.Widgets
{

    public class CpuRamWidget : FramedWidget
    {
        Bar CPUBar;
        Bar RAMBar;
        Label RAMDetails;

        public CpuRamWidget(IServiceProvider serviceProvider) : base(serviceProvider) { }

        protected override void DrawWidget()
        {
            Title = settings.CpuRamWidgetTitle;

            Label cpuTitle = new Label("CPU : ")
            {
                X = 2,
                Y = 1
            };

            Add(cpuTitle);

            CPUBar = new Bar(settings.CpuRamWidgetBarColor, settings.MainBackgroundColor)
            {
                X = Pos.Right(cpuTitle),
                Y = cpuTitle.Y,
                Width = Dim.Percent(65),
                Height = Dim.Sized(1)
            };

            Add(CPUBar);

            Label ramTitle = new Label("RAM : ")
            {
                X = cpuTitle.X,
                Y = Pos.Bottom(cpuTitle) + 1
            };

            Add(ramTitle);

            RAMBar = new Bar(settings.CpuRamWidgetRamBarColor, settings.MainBackgroundColor)
            {
                X = Pos.Right(ramTitle),
                Y = Pos.Bottom(cpuTitle) + 1,
                Width = Dim.Percent(65),
                Height = Dim.Sized(1)
            };

            Add(RAMBar);

            RAMDetails = new Label(string.Empty)
            {
                X = Pos.Right(RAMBar),
                Y = 3
            };

            Add(RAMDetails);

        }

        protected override async Task Update(MainLoop MainLoop)
        {
            var cpusUsage = await systemInfo.GetCPUUsage();
            CPUBar.Update(MainLoop, cpusUsage.UsagePercentage);

            var memoryUsage = await systemInfo.GetMemoryUsage();
            RAMBar.Update(MainLoop, memoryUsage.PhysicalMemoryUsed);
            RAMDetails.Text = $"({SizeConversionHelper.BytesToGB(memoryUsage.PhysicalMemoryAvailable)} GB available)";

        }
    }
}