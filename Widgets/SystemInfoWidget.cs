using System;
using System.Threading.Tasks;
using Terminal.Gui;
using wintop.Widgets.Common;

namespace wintop.Widgets
{
    public class SystemInfoWidget : FramelessWidget
    {
        const string headerText = "System Info : ";
        const string contentTemplate = "{0} on {1} (version {2})";

        Label value;

        protected override int RefreshTimeSeconds => 60;

        public SystemInfoWidget(IServiceProvider serviceProvider) : base(serviceProvider) { }

        protected override void DrawWidget()
        {
            var headerLabel = new Label(headerText)
            {
                X = 4,
                Y = 1
            };

            headerLabel.ColorScheme = new ColorScheme() { Normal = Terminal.Gui.Attribute.Make(settings.LabelWidgetHeaderColor, settings.MainBackgroundColor) };

            Add(headerLabel);

            value = new Label()
            {
                X = Pos.Right(headerLabel),
                Y = 1
            };

            Add(value);
        }

        protected override async Task Update(MainLoop MainLoop)
        {
            var osInfo = await systemInfo.GetDeviceInformation();
            value.Text = string.Format(contentTemplate, osInfo.DeviceName, osInfo.OSName, osInfo.OSVersion);
        }
    }
}