using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;
using wintop.Core;

namespace wintop.Widgets.Common
{
    public abstract class FramedWidget : FrameView
    {
        protected ISystemInfo systemInfo;
        protected Settings settings;
        protected virtual int RefreshTimeSeconds => 1;

        public FramedWidget(IServiceProvider serviceProvider) : base()
        {
            systemInfo = serviceProvider.GetService<ISystemInfo>();
            settings = serviceProvider.GetService<Settings>();
            Draw();
        }

        protected abstract void DrawWidget();

        private void Draw()
        {
            DrawWidget();
            Task.Run(async () => await Update(Application.MainLoop));
        }

        public async Task RefreshIfNeeded(MainLoop MainLoop, int tick)
        {
            if ((tick % RefreshTimeSeconds) == 0 || tick == 0)
            {
                await Update(MainLoop);
            }
        }

        protected abstract Task Update(MainLoop MainLoop);
    }
}