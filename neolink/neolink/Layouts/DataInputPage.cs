using System;
using neolink.Helpers;
using neolink.Layouts;
using neolink.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace neolink.Pages
{
    public class DataInputPage
    {
        Image RandomiseData;
        Image ClearData;

        Label DataInputLabel;
        Entry DataInput;

        Label DataStreamLabel;
        Label DataStream;

        string RandomInput = "";

        public Grid Content;


        int smallIconSize;

        int dataInputFontSize;

        int dataInputHeight;

        int labelFontSize;

        public DataInputPage(string dataStream)
        {

            smallIconSize = Units.ScreenWidth10Percent;
            if (smallIconSize > 48) { smallIconSize = 48; };

            dataInputFontSize = Units.DynamicFontSizeXL;
            if (dataInputFontSize > 16) { dataInputFontSize = 16; }

            labelFontSize = Units.DynamicFontSizeXL;
            if (labelFontSize > 16) { labelFontSize = 16; }

            dataInputHeight = dataInputFontSize * 2;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                dataInputHeight = dataInputFontSize * 3;
            }


            Content = new Grid
            {
                BackgroundColor = Color.FromHex("#000000"),
                WidthRequest = Units.ScreenWidth,
                HeightRequest = Units.ScreenHeight,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                //Padding = new Thickness(Units.ScreenUnitL, 64),
            };

            RandomiseData = new Image
            {
                Source = "random.png",
                WidthRequest = smallIconSize,
                HeightRequest = smallIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 8
            };

            ClearData = new Image
            {
                Source = "edit.png",
                WidthRequest = smallIconSize,
                HeightRequest = smallIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 8
            };

            DataInputLabel = new Label
            {
                FontSize = labelFontSize,
                FontAttributes = FontAttributes.Bold,
                Text = "Editable Input Stream",
                TextColor = Color.Cyan,
                Margin = new Thickness(0, 16, 0, 0)
            };

            DataInput = new Entry
            {
                BackgroundColor = Color.FromHex("#eeeeee"),
                TextColor = Color.FromHex("#111111"),
                FontSize = dataInputFontSize,
                WidthRequest = Units.ScreenWidth,
                HeightRequest = dataInputHeight,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Placeholder = "Paste or type your data here",
                FlowDirection = FlowDirection.LeftToRight,
                Text = dataStream
            };

            DataInput.TextChanged += UpdateStream;

            DataStreamLabel = new Label
            {
                FontSize = labelFontSize,
                Text = "Data Input Stream",
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Cyan,
                Margin = new Thickness(0, 8, 0, 8)

            };

            DataStream = new Label
            {
                FontSize = labelFontSize,
                Text = dataStream,
                TextColor = Color.White
            };

            ScrollView streamScroller = new ScrollView
            {
                Content = DataStream
            };

            RandomiseData.GestureRecognizers.Add(
                 new TapGestureRecognizer()
                 {
                     Command = new Command(() =>
                     {
                         CreateRandomInput();
                     })
                 }
             );

            ClearData.GestureRecognizers.Add(
                 new TapGestureRecognizer()
                 {
                     Command = new Command(() =>
                     {
                         ClearInput();
                     })
                 }
             );

            StackLayout contentContainer = new StackLayout
            {
                BackgroundColor = Color.Transparent,
                Orientation = StackOrientation.Vertical,
                
                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            ClearData,
                            RandomiseData
                        }
                    },
                    DataInputLabel,
                    DataInput,
                    DataStreamLabel,
                    streamScroller
                },
                Margin = new Thickness(0, 32, 0, 16)
            };

            Content.Children.Add(contentContainer, 0, 0); 
            
            Grid.SetRowSpan(contentContainer, 5);

        }

        
        public void CreateRandomInput()
        {
            RandomInput = "";
            for (int i=0; i<Numbers.GetNextRandom(100, 10000); i++)
            {
                RandomInput += Numbers.GetNextRandom(0, 1024) + ", ";
                
            }
            Console.WriteLine(RandomInput);

            RandomInput = RandomInput.Substring(0, RandomInput.Length - 2);
            DataInput.Text = RandomInput;
            DataStream.Text = RandomInput;
        }

        void UpdateStream(object sender, TextChangedEventArgs e)
        {
            UpdateStreamData();
            
        }

        public void ClearInput()
        {
            DataInput.Text = "";
            //DataStream.Text = "";
            UpdateStreamData();
        }

        public void UpdateStreamData(string dataStream)
        {
            Globals.FoundPhrases.Clear();

            string cleanedInput = DataReader.SanitizeToString(dataStream, 4);

            DataInput.Text = cleanedInput;
            DataStream.Text = cleanedInput;
        }

        public void UpdateStreamData()
        {
            Globals.FoundPhrases.Clear();

            DataStream.Text = DataReader.SanitizeToString(DataInput.Text, 4);

            string cleanedInput = DataInput.Text;

            DataInput.Text = cleanedInput;
            DataStream.Text = DataReader.SanitizeToString(cleanedInput, 4);
        }


        public string GetInput()
        {
            return DataStream.Text;
        }  
    }
}


