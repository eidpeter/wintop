using System;
using System.Threading.Tasks;
using Terminal.Gui;
using wintop.Common.Helpers;
using wintop.Widgets.Common;

namespace wintop.Widgets
{
    public class NetworkWidget : FramedWidget
    {
        Label downloadValueLabel;
        Label uploadValueLabel;
        long downloadValue = 0;
        long uploadValue = 0;

        public NetworkWidget(IServiceProvider serviceProvider) : base(serviceProvider) { }

        protected override void DrawWidget()
        {
            Title = settings.NetworkWidgetTitle;

            Label downloadHeader = new Label("Download (kB/s) :  ")
            {
                X = 2,
                Y = 1
            };

            Add(downloadHeader);

            downloadValueLabel = new Label(string.Empty)
            {
                X = Pos.Right(downloadHeader),
                Y = downloadHeader.Y
            };

            downloadValueLabel.ColorScheme = new ColorScheme() { Normal = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor) };

            Add(downloadValueLabel);

            Label uploadHeader = new Label("Upload (kB/s)   :  ")
            {
                X = downloadHeader.X,
                Y = Pos.Bottom(downloadHeader) + 1
            };

            Add(uploadHeader);

            uploadValueLabel = new Label(string.Empty)
            {
                X = Pos.Right(uploadHeader),
                Y = Pos.Bottom(downloadValueLabel) + 1
            };

            uploadValueLabel.ColorScheme = new ColorScheme() { Normal = Terminal.Gui.Attribute.Make(settings.MainForegroundColor, settings.MainBackgroundColor) };

            Add(uploadValueLabel);

        }
        protected override async Task Update(MainLoop MainLoop)
        {
            var network = await systemInfo.GetNetworkUsage();

            if (downloadValue == 0)
            {
                downloadValue = network.BytesReceived;
                uploadValue = network.BytesSent;
            }
            else
            {
                var up = (network.BytesSent - uploadValue) / RefreshTimeSeconds;
                uploadValueLabel.Text = $"{SizeConversionHelper.BytesToKB(up)}";
                uploadValue = network.BytesSent;

                var down = (network.BytesReceived - downloadValue) / RefreshTimeSeconds;
                downloadValueLabel.Text = $"{SizeConversionHelper.BytesToKB(down)}";
                downloadValue = network.BytesReceived;
            }
        }
    }
}