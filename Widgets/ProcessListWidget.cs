using System;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;
using wintop.Widgets.Common;
using wintop.Common.Helpers;
using System.Data;

namespace wintop.Widgets
{
    public class ProcessListWidget : FramedWidget
    {
        TableView table;

        // public ProcessListOrder Order { get; set; } = ProcessListOrder.CPU;

        public ProcessOrder _processOrder;

        protected override int RefreshTimeSeconds => 2;

        public ProcessListWidget(IServiceProvider serviceProvider, ProcessOrder processOrder) : base(serviceProvider)
        {
            _processOrder = processOrder;
        }

        protected override void DrawWidget()
        {
            Title = settings.ProcessesListWidgetTitle;

            var data = DataTableHelper.ListToDataTable(systemInfo.GetProcesses().Result.AllProcesses);

            // foreach (DataRow row in data.Rows)
            // {
            //     row[3] = SizeConversionHelper.BytesToMB((long)row[3]);
            // }

            table = new TableView(data)
            {
                X = 2,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                FullRowSelect = true,
                TextAlignment = TextAlignment.Centered,
                Style = new TableView.TableStyle()
                {
                    ShowHorizontalHeaderOverline = false,
                    ShowVerticalCellLines = false,
                    ShowVerticalHeaderLines = false,
                    AlwaysShowHeaders = true
                }
            };

            Add(table);
        }

        public void SetOrdering(ProcessOrder processOrder)
        {
            _processOrder = processOrder;
            _processOrder.ToggleOrdering();
        }

        protected override async Task Update(MainLoop MainLoop)
        {
            var processList = await systemInfo.GetProcesses();
            // var data = DataTableHelper.ListToDataTable(processList.OrderByDescendingCPUUsage().ToList());
            // foreach (DataRow row in data.Rows)
            // {
            //     row[3] = SizeConversionHelper.BytesToMB((long)row[3]);
            // }
            // table.Table = data;

            table.Table = DataTableHelper.ListToDataTable(_processOrder.OrderProcesses(processList).ToList());

            // switch (Order)
            // {
            //     case ProcessListOrder.CPU:
            //         table.Table = DataTableHelper.ListToDataTable(processList.OrderByDescendingCPUUsage().ToList());
            //         break;

            //     case ProcessListOrder.Memory:
            //         table.Table = DataTableHelper.ListToDataTable(processList.OrderByDescendingMemoryUsage().ToList());
            //         break;
            // }
        }
    }
}