using System;
using Terminal.Gui;

namespace wintop.Widgets.Common
{
    public class Bar : Component<float>
    {
        Label percentText;

        Label percentBar;

        char barChar = 'I';

        public Bar(Color BarColor, Color BackgroundColor)
        {
            DrawFrame(Bounds, 0, false);

            Label openingBraquet = new Label("[")
            {
                X = 0,
                Y = 0
            };

            Add(openingBraquet);

            Label closingBraquet = new Label("]")
            {
                X = Pos.AnchorEnd(7),
                Y = 0
            };

            Add(closingBraquet);

            percentBar = new Label(string.Empty)
            {
                X = Pos.Right(openingBraquet),
                Y = 0,
                Width = Dim.Fill(7)
            };

            percentBar.ColorScheme = new ColorScheme() { Normal = Terminal.Gui.Attribute.Make(BarColor, BackgroundColor) };

            Add(percentBar);

            percentText = new Label(string.Empty)
            {
                X = Pos.Right(closingBraquet),
                Y = 0,
            };

            Add(percentText);
        }

        protected override void UpdateAction(float newValue)
        {
            percentText.Text = $" {newValue} % ";
            var labelWidth = percentBar.Bounds.Width;
            int charCount = (int)Math.Floor(newValue * labelWidth / 100);
            percentBar.Text = new string(barChar, charCount);
        }
    }
}