using System;
using neolink.Helpers;
using neolink.Pages;
using Xamarin.Forms;

namespace neolink.Layouts
{
    public class Footer
    {
        public StackLayout Content;

        Image StartStopImage;
        Image WordsImage;
        Image NumbersImage;
        Image ConnectImage;
        Image DownloadImage;


        public Footer()
        {
            int smallIconSize = Units.ScreenWidth10Percent;
            if (smallIconSize > 48) { smallIconSize = 48; };

            int largeIconSize = Units.ScreenWidth20Percent;
            if (largeIconSize > 64) { largeIconSize = 64; };


            Content = new StackLayout
            {

            };

            StartStopImage = new Image
            {
                Source = "start.png",
                WidthRequest = largeIconSize,
                HeightRequest = largeIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 2
            };

            StartStopImage.GestureRecognizers.Add(
                  new TapGestureRecognizer()
                  {
                      Command = new Command(() =>
                      {
                          App.GoToPage(Globals.MAIN_PAGE);
                          App.ToggleOnOff();    
                      })
                  }
              );

            /*UpdateDataStream = new Image
            {
                Source = "edit.png",
                WidthRequest = Units.ScreenUnitM * 2,
                HeightRequest = Units.ScreenUnitM * 2,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 12
            };

            UpdateDataStream.GestureRecognizers.Add(
                  new TapGestureRecognizer()
                  {
                      Command = new Command(() =>
                      {
                          //App.GoToPage(Globals.DATA_INPUT_PAGE);
                          App.GoToPage(Globals.DICTIONARY_PAGE);

                      })
                  }
              );*/


            WordsImage = new Image
            {
                Source = "words.png",
                WidthRequest = smallIconSize,
                HeightRequest = smallIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 12
            };

            WordsImage.GestureRecognizers.Add(
                  new TapGestureRecognizer()
                  {
                      Command = new Command(() =>
                      {
                          //App.GoToPage(Globals.DATA_INPUT_PAGE);
                          App.GoToPage(Globals.DICTIONARY_PAGE);

                      })
                  }
              );

            NumbersImage = new Image
            {
                Source = "numbers.png",
                WidthRequest = smallIconSize,
                HeightRequest = smallIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 12
            };

            NumbersImage.GestureRecognizers.Add(
                  new TapGestureRecognizer()
                  {
                      Command = new Command(() =>
                      {
                          App.GoToPage(Globals.DATA_INPUT_PAGE);
                          //App.GoToPage(Globals.DICTIONARY_PAGE);

                      })
                  }
              );

            ConnectImage = new Image
            {
                Source = "connect.png",
                WidthRequest = smallIconSize,
                HeightRequest = smallIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 12
            };

            ConnectImage.GestureRecognizers.Add(
                  new TapGestureRecognizer()
                  {
                      Command = new Command(() =>
                      {
                          //App.DisplayAlert("UNAVAILABLE", "Connecting to external devices not available", "OK");
                          App.GoToPage(Globals.DEVICES_PAGE);
                          //App.GoToPage(Globals.DICTIONARY_PAGE);

                      })
                  }
              );

            DownloadImage = new Image
            {
                Source = "download.png",
                WidthRequest = smallIconSize,
                HeightRequest = smallIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 12
            };

            DownloadImage.GestureRecognizers.Add(
                  new TapGestureRecognizer()
                  {
                      Command = new Command(() =>
                      {
                          //App.GoToPage(Globals.DATA_INPUT_PAGE);
                          App.GoToPage(Globals.DOWNLOADS_PAGE);
                          //App.DisplayAlert("UNAVAILABLE", "Data downloading not yet available", "OK");
                      })
                  }
              );

            StackLayout footer = new StackLayout
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children = {
                    ConnectImage,
                    WordsImage,
                    StartStopImage,
                    NumbersImage,
                    DownloadImage
                }

            };

            Content.Children.Add(footer);
        }

        public void Update()
        {
            try
            {
                StartStopImage.Opacity = 0.5;
                WordsImage.Opacity = 0.5;
                NumbersImage.Opacity = 0.5;
                ConnectImage.Opacity = 0.5;
                DownloadImage.Opacity = 0.5;

                switch (Globals.CURRENT_PAGE)
                {
                    case Globals.DATA_INPUT_PAGE:
                        NumbersImage.Opacity = 1.0;
                        break;
                    case Globals.DICTIONARY_PAGE:
                        WordsImage.Opacity = 1.0;
                        break;
                    case Globals.DEVICES_PAGE:
                        ConnectImage.Opacity = 1.0;
                        break;
                    case Globals.DOWNLOADS_PAGE:
                        DownloadImage.Opacity = 1.0;
                        break;
                    default:
                        StartStopImage.Opacity = 1.0;
                        break;

                }

                if (Globals.NeolinkOn)
                {
                    StartStopImage.Source = "stop.png";
                }
                else
                {
                    StartStopImage.Source = "start.png";
                }


                if (Globals.CURRENT_PAGE == Globals.MAIN_PAGE)
                {
                    StartStopImage.Opacity = 1.0;
                    if (Globals.IsPaused)
                    {
                        //StartStopImage.Source = "paused.png";
                    }
                    else
                    {
                        //StartStopImage.Source = "start.png";
                    }
                }
                else
                {
                    StartStopImage.Opacity = 0.5;
                }




                
            }
            catch(Exception e)
            {

            }
        }

    }
}


