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
            int headerFontSize = Units.DynamicFontSizeXXL;
            if (headerFontSize > 32) { headerFontSize = 32; }

            //int topLogoSize = (int)(Units.ScreenWidth * 0.65);
            //if (topLogoSize > 32) { topLogoSize = 32; }

            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0
            };

            TopLogo = new Image
            {
                Source = "neolink.png",
                //WidthRequest = topLogoSize,
                
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness((Units.ScreenUnitL * 2), 0, (Units.ScreenUnitL * 2), Units.ScreenUnitS)
            };

            TitleLabel = new Label
            {
                //FontFamily = Fonts.GetRegularAppFont(),
                Text = title,
                TextColor = Color.Yellow,
                FontAttributes = FontAttributes.Bold,
                FontSize = headerFontSize,

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


