using System;
using neolink.Helpers;
using Xamarin.Forms;

namespace neolink.Layouts
{
    public class Header
    {
        public StackLayout Content;

        Image TopLogo;

        Label TitleLabel;

        public Header(string title)
        {
            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0
            };

            TopLogo = new Image
            {
                Source = "neolink.png",
                WidthRequest = Units.ScreenWidth * 0.65,

                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = 8
            };

            TitleLabel = new Label
            {
                //FontFamily = Fonts.GetRegularAppFont(),
                Text = title,
                TextColor = Color.Yellow,
                FontAttributes = FontAttributes.Bold,
                FontSize = Units.DynamicFontSizeXXL,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            };

            Content.Children.Add(TopLogo);
            Content.Children.Add(TitleLabel);
        }

        public void SetTitle(string title)
        {
            Content.Children.Remove(TitleLabel);

            TitleLabel.Text = title;

            Content.Children.Add(TitleLabel);
        }
    }
}


