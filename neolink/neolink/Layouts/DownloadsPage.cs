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

        int smallIconSize;

        int dataInputFontSize;

        int dataInputHeight;

        int labelFontSize;

        int buttonLabelSize;

        int dataListFontSize;

        bool isLocalFile;

        public DownloadsPage()
        {

            isLocalFile = false;
            smallIconSize = Units.ScreenWidth10Percent;
            if (smallIconSize > 48) { smallIconSize = 48; };

            dataInputFontSize = Units.DynamicFontSizeXL;
            if (dataInputFontSize > 16) { dataInputFontSize = 16; }

            labelFontSize = Units.DynamicFontSizeXL;
            if (labelFontSize > 16) { labelFontSize = 16; }

            buttonLabelSize = Units.DynamicFontSizeXXXL;
            if (buttonLabelSize > 13) { buttonLabelSize = 13; }

            dataListFontSize = buttonLabelSize*2;

            dataInputHeight = dataInputFontSize * 2;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                dataInputHeight = dataInputFontSize * 3;
            }


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
                BackgroundColor = Color.Cyan,
                FontSize = buttonLabelSize,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "IMPORT DATA FROM FILE",
                TextColor = Color.Black,
                WidthRequest = buttonLabelSize * 16,
                HeightRequest = buttonLabelSize * 4,
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
                                 //DownloadListLabel.TextColor = Color.DarkCyan;
                                 //DownloadListLabel.Text = "Scanning for data files...";

                                 FileResult res = await FileManager.Pick(Xamarin.Essentials.PickOptions.Default);

                                 LoadedFile = FileManager.LoadLocalFile(res.FullPath);

                                 SelectedFile.Text = res.FullPath;

                                 isLocalFile = true;

                                 if (LoadedFile != null)
                                 {
                                     //DownloadListLabel.TextColor = Color.White;
                                    // DownloadListLabel.Text = " DATA LOADED : " + res.FullPath;
                                 }
                                 else
                                 {
                                    // DownloadListLabel.TextColor = Color.Red;
                                    // DownloadListLabel.Text = "No data files found";
                                     //await Task.Delay(2000);
                                     //DownloadListLabel.TextColor = Color.White;
                                     //DownloadListLabel.Text = "IMPORT DATA FROM FILE";
                                 }
                             }
                             catch (Exception e)
                             {
                                 DownloadListLabel.TextColor = Color.Red;
                                 DownloadListLabel.Text = "ERROR";
                                 await Task.Delay(2000);
                                 DownloadListLabel.TextColor = Color.White;
                                 DownloadListLabel.Text = "IMPORT DATA FROM FILE";
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
                FontSize = dataInputFontSize,
                WidthRequest = Units.ScreenWidth,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = dataInputHeight,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Placeholder = "Selected data stream",
                FlowDirection = FlowDirection.LeftToRight,
                Text = "",
                Margin = new Thickness(Units.ScreenWidth15Percent, 0)
            };

            SystemDataFiles = new Label
            {

                FontSize = buttonLabelSize,
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
                    FontSize = dataListFontSize,
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
                                 isLocalFile = false;
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
                BackgroundColor = Color.Cyan,
                FontSize = buttonLabelSize,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "UPDATE NEOLINK DATA",
                TextColor = Color.Black,
                WidthRequest = buttonLabelSize * 16,
                HeightRequest = buttonLabelSize * 4,
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


                             string fileContent = DataReader.ReadJsonFile(SelectedFile.Text);

                             if (isLocalFile)
                             {
                                 fileContent = FileManager.LoadLocalFile(SelectedFile.Text);
                             }
                             

                             App.LoadDataStream(fileContent);

                             App.GoToPage(Globals.DATA_INPUT_PAGE);
                         });

                     })

                 }
             );

            StackLayout ContentContainer = new StackLayout
            {
                BackgroundColor = Color.Transparent,
                Orientation = StackOrientation.Vertical,

                Children =
                {
                   SelectedFile,
                   SystemDataFiles,
                   FileScroller,
                   DownloadListLabel,
                   UpdateDataStream
                },
                //Margin = new Thickness(0, 32, 0, 16)

            };

            Content.Children.Add(ContentContainer, 0, 1);


            Grid.SetRowSpan(ContentContainer, 5);
        }
    }
}







