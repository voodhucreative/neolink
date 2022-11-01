using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using neolink.Helpers;
using neolink.Layouts;
using Xamarin.Essentials;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace neolink.Pages
{
    public class DevicesPage
    {
        
        public Grid Content;

        Label DeviceListLabel;

        public DevicesPage()
        {

            Content = new Grid
            {
                BackgroundColor = Color.FromHex("#000000"),
                WidthRequest = Units.ScreenWidth,
                HeightRequest = Units.ScreenHeight,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            DeviceListLabel = new Label
            {
                FontSize = Units.DynamicFontSizeXXXL,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "CLICK TO SCAN",
                TextColor = Color.White,
                WidthRequest = Units.ScreenWidth,
                Margin = new Thickness(8, 16, 0, 0)
            };

            DeviceListLabel.GestureRecognizers.Add(
                 new TapGestureRecognizer()
                 {
                     Command = new Command(() =>
                     {
                         Device.BeginInvokeOnMainThread(async () =>
                         {
                             DeviceListLabel.TextColor = Color.DarkCyan;
                             DeviceListLabel.Text = "Scanning for devices...";
                             await Task.Delay(3000);
                             DeviceListLabel.TextColor = Color.Red;
                             DeviceListLabel.Text = "No devices found";
                             await Task.Delay(2000);
                             DeviceListLabel.TextColor = Color.White;
                             DeviceListLabel.Text = "CLICK TO SCAN"; 
                         });
                         
                     })

                 }
             );

            StackLayout ContentContainer = new StackLayout
            {

                Orientation = StackOrientation.Vertical,

                Children =
                {
                   DeviceListLabel
                },
                Margin = new Thickness(0, 32, 0, 16)

            };

            Content.Children.Add(ContentContainer, 0, 0);


            Grid.SetRowSpan(ContentContainer, 5);
        }
    }
}





