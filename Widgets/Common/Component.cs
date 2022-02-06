using Terminal.Gui;

namespace wintop.Widgets.Common
{
    public abstract class Component<T> : View
    {
        protected abstract void UpdateAction(T newValue);

        public bool Update(MainLoop MainLoop, T newValue)
        {
            MainLoop.Invoke(() =>
            {
                UpdateAction(newValue);
            });
            return true;
        }
    }
}