using System;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;
using wintop.Common.Helpers;
using wintop.Widgets.Common;

namespace wintop.Widgets
{
    public class DiskWidget : FramedWidget
    {
        int counter = 0;
        Label writeValueLabel;
        Label readValueLabel;
        long writeValue = 0;
        long readValue = 0;
        View disksView;
        Label[] disksNames = null;
        Bar[] disksUsage = null;

        public DiskWidget(IServiceProvider serviceProvider) : base(serviceProvider) { }

        protected override void DrawWidget()
        {
            Title = settings.DiskWidgetTitle;

            Label titleWrite = new Label("Write (kB/s)    :  ")
            {
                X = 2,
                Y = 1
            };

            Add(titleWrite);

            writeValueLabel = new Label(string.Empty)
            {
                X = Pos.Right(titleWrite),
                Y = titleWrite.Y
            };

            writeValueLabel.ColorScheme = new ColorScheme() { Normal = Terminal.Gui.Attribute.Make(settings.DiskWidgetWriteTextColor, settings.MainBackgroundColor) };

            Add(writeValueLabel);

            Label titleRead = new Label("Read (kB/s)     :  ")
            {
                X = titleWrite.X,
                Y = Pos.Bottom(titleWrite) + 1
            };

            Add(titleRead);

            readValueLabel = new Label(string.Empty)
            {
                X = Pos.Right(titleRead),
                Y = Pos.Bottom(writeValueLabel) + 1
            };

            readValueLabel.ColorScheme = new ColorScheme() { Normal = Terminal.Gui.Attribute.Make(settings.DiskWidgetReadTextColor, settings.MainBackgroundColor) };

            Add(readValueLabel);

            var diskCount = systemInfo.DiskCount;

            disksView = new View()
            {
                X = 1,
                Y = 4,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            Add(disksView);

            disksNames = new Label[diskCount];
            disksUsage = new Bar[diskCount];
            int offset = 1;

            for (var i = 0; i < diskCount; i++)
            {
                disksUsage[i] =
                    new Bar(settings.DiskWidgetUsageBarColor, settings.MainBackgroundColor)
                    {
                        X = 1,
                        Y = offset,
                        Width = Dim.Percent(65),
                        Height = Dim.Sized(1)
                    };

                disksNames[i] = new Label(string.Empty)
                {
                    X = Pos.Right(disksUsage[i]),
                    Y = offset
                };

                offset++;
            }

            disksView.Add(disksNames);
            disksView.Add(disksUsage);
        }

        protected override async Task Update(MainLoop MainLoop)
        {
            var disks = await systemInfo.GetDisksUsage();

            if (writeValue == 0)
            {
                writeValue = disks.BytesWritten;
                readValue = disks.BytesRead;
            }
            else
            {
                var write = (disks.BytesWritten - writeValue) / RefreshTimeSeconds;
                writeValueLabel.Text = $"{SizeConversionHelper.BytesToKB(write)}";
                writeValue = disks.BytesWritten;

                var read = (disks.BytesRead - readValue) / RefreshTimeSeconds;
                readValueLabel.Text = $"{SizeConversionHelper.BytesToKB(read)}";
                readValue = disks.BytesRead;
            }

            if (counter == 0 || counter == 59)
            {
                var storageInfo = await systemInfo.GetStorageUsage();
                for (var i = 0; i < disksUsage.Length; i++)
                {
                    disksNames[i].Text = string.Format("used on '{0} ({1})'", storageInfo.ElementAt(i).Name, storageInfo.ElementAt(i).VolumeLabel);
                    disksUsage[i].Update(MainLoop, storageInfo.ElementAt(i).PercentageUsed);
                };

                counter = counter == 59 ? 0 : counter++;
            }
        }
    }
}