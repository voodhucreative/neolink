using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using neolink.Helpers;
using neolink.Layouts;
using neolink.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace neolink.Pages
{
    public class DownloadsPage
    {

        public Grid Content;

        Label DownloadListLabel;

        List<string> NeoLinkDefaultFiles;

        string LoadedFile;

        Entry SelectedFile;

        Label SystemDataFiles;

        List<Label> SystemDataFileList;

        ScrollView FileScroller;

        Label UpdateDataStream;

        public DownloadsPage()
        {
            NeoLinkDefaultFiles = FileManager.GetLocalNeoLinkFiles();

            Content = new Grid
            {
                BackgroundColor = Color.FromHex("#000000"),
                WidthRequest = Units.ScreenWidth,
                HeightRequest = Units.ScreenHeight,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            DownloadListLabel = new Label
            {
                FontSize = Units.DynamicFontSizeXXL,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "LOAD DATA",
                TextColor = Color.White,
                WidthRequest = Units.ScreenWidth,
                Margin = new Thickness(8, 16, 0, 0)
            };

            DownloadListLabel.GestureRecognizers.Add(
                 new TapGestureRecognizer()
                 {
                     Command = new Command(() =>
                     {
                         Device.BeginInvokeOnMainThread(async () =>
                         {
                             try
                             {
                                 DownloadListLabel.TextColor = Color.DarkCyan;
                                 DownloadListLabel.Text = "Scanning for data files...";

                                 FileResult res = await FileManager.Pick(Xamarin.Essentials.PickOptions.Default);

                                 LoadedFile = FileManager.LoadLocalFile(res.FullPath);

                                 if (LoadedFile != null)
                                 {
                                     DownloadListLabel.TextColor = Color.White;
                                     DownloadListLabel.Text = " DATA LOADED : " + res.FullPath;
                                 }
                                 else
                                 {
                                     DownloadListLabel.TextColor = Color.Red;
                                     DownloadListLabel.Text = "No data files found";
                                     await Task.Delay(2000);
                                     DownloadListLabel.TextColor = Color.White;
                                     DownloadListLabel.Text = "CLICK TO LOAD DATA";
                                 }
                             }
                             catch (Exception e)
                             {
                                 DownloadListLabel.TextColor = Color.Red;
                                 DownloadListLabel.Text = "ERROR";
                                 await Task.Delay(2000);
                                 DownloadListLabel.TextColor = Color.White;
                                 DownloadListLabel.Text = "CLICK TO LOAD DATA";
                             }

                            // App.LoadDataStream(LoadedFile);

                             
                         });

                     })

                 }
             );

            SelectedFile = new Entry
            {
                BackgroundColor = Color.FromHex("#eeeeee"),
                TextColor = Color.FromHex("#111111"),
                FontSize = 16,
                WidthRequest = Units.ScreenWidth,
                HeightRequest = 40,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Placeholder = "Selected data stream",
                FlowDirection = FlowDirection.LeftToRight,
                Text = ""
            };

            SystemDataFiles = new Label
            {

                FontSize = Units.DynamicFontSizeXXL,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "NeoLink Data Files",
                TextColor = Color.White,
                WidthRequest = Units.ScreenWidth
            };

            SystemDataFileList = new List<Label>();

            foreach (string localFile in NeoLinkDefaultFiles)
            {
                Label newLabel = new Label
                {
                    FontSize = Units.DynamicFontSizeL,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = localFile.Replace("neolink.", ""),
                    TextColor = Color.Yellow,
                    WidthRequest = Units.ScreenWidth
                };

                newLabel.GestureRecognizers.Add(
                     new TapGestureRecognizer()
                     {
                         Command = new Command(() =>
                         {
                             Device.BeginInvokeOnMainThread(async () =>
                             {
                                 SelectedFile.Text = localFile.Replace("neolink.", "");
                             });

                         })
                     }
                 );

                SystemDataFileList.Add(newLabel);
            }

            StackLayout localListContainer = new StackLayout
            {
                Orientation = StackOrientation.Vertical
            };

            foreach (Label localfile in SystemDataFileList)
            {
                localListContainer.Children.Add(localfile);
            }

            FileScroller = new ScrollView
            {
                Content = localListContainer
            };

            UpdateDataStream = new Label
            {
                FontSize = Units.DynamicFontSizeXXL,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "UPDATE DATA",
                TextColor = Color.Cyan,
                WidthRequest = Units.ScreenWidth,
                Margin = new Thickness(8, 16, 0, 0)
            };

            UpdateDataStream.GestureRecognizers.Add(
                 new TapGestureRecognizer()
                 {
                     Command = new Command(() =>
                     {
                         Device.BeginInvokeOnMainThread(async () =>
                         {
                             //App.LoadLocalDataStream("neodata251022.json");
                             //App.LoadDataStream(DataReader.ReadJsonFile(SelectedFile.Text));


                             string tits = DataReader.ReadJsonFile(SelectedFile.Text);
                             App.LoadDataStream(tits);

                             App.GoToPage(Globals.DATA_INPUT_PAGE);
                         });

                     })

                 }
             );

            StackLayout ContentContainer = new StackLayout
            {

                Orientation = StackOrientation.Vertical,

                Children =
                {
                   SelectedFile,
                   SystemDataFiles,
                   FileScroller,
                   DownloadListLabel,
                   UpdateDataStream
                },
                Margin = new Thickness(0, 32, 0, 16)

            };

            Content.Children.Add(ContentContainer, 0, 0);


            Grid.SetRowSpan(ContentContainer, 5);
        }
    }
}







