using Terminal.Gui;

namespace wintop.Core
{
    public class AppMainWindow : Window
    {
        public AppMainWindow(string Title) : base(Title)
        { }

        public override bool ProcessKey(KeyEvent keyEvent)
        {
            return base.ProcessKey(keyEvent);
        }
    }
}