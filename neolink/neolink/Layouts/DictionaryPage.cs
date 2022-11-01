using System;
using System.Collections.Generic;
using neolink.Helpers;
using neolink.Layouts;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace neolink.Pages
{
    public class DictionaryPage
    {
        Image RandomiseData;

        List<Entry> PhraseList;

        ScrollView DataInputScroller;
        StackLayout PhraseListContainer;

        public Grid Content;

        Label KeysLabel;

        Label PhrasesLabel;

        public DictionaryPage(List<string> phrases)
        {
            Content = new Grid
            {
                BackgroundColor = Color.FromHex("#000000"),
                WidthRequest = Units.ScreenWidth,
                HeightRequest = Units.ScreenHeight,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                // = new Thickness(Units.ScreenUnitL, 64),
            };

            RandomiseData = new Image
            {
                Source = "random.png",
                WidthRequest = Units.ScreenUnitM * 2,
                HeightRequest = Units.ScreenUnitM * 2,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 8
            };

            PhraseList = new List<Entry>();

            PhraseListContainer = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 2
            };

            DataInputScroller = new ScrollView
            {
                Content = PhraseListContainer,
                Margin = new Thickness(8, 8, 8, 8)
            };

            CreateNumberedDictionary();
 
            RandomiseData.GestureRecognizers.Add(
                 new TapGestureRecognizer()
                 {
                     Command = new Command(() =>
                     {
                         GenerateNewPhrases();
                     })
                 }
             );

            KeysLabel = new Label
            {
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                Text = "Keys",
                TextColor = Color.Cyan,
                WidthRequest = 64,
                Margin = new Thickness(8, 16, 0, 0)
            };

            PhrasesLabel = new Label
            {
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                Text = "Session Phrases",
                TextColor = Color.Cyan,
                WidthRequest = Units.ScreenWidth - 150,
                Margin = new Thickness(0, 16, 0, 0)
            };

            StackLayout ContentContainer = new StackLayout
            {

                Orientation = StackOrientation.Vertical,

                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Grid{ },
                            RandomiseData
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            KeysLabel,
                            PhrasesLabel
                        }
                    },
                    DataInputScroller
                },
                Margin = new Thickness(0, 32, 0, 16)

            };

            Content.Children.Add(ContentContainer, 0, 0);

            Grid.SetRowSpan(ContentContainer, 5);
        }

        public void CreateDictionary(List<string> phrases)
        {
            PhraseList.Clear();

            foreach (string phrase in phrases)
            {
                Color bgColor = Color.FromHex("#eeeeee");


                if (Globals.FoundPhrases.Contains(phrase))
                {
                    bgColor = Color.Red;
                }

                PhraseList.Add(new Entry
                {
                    BackgroundColor = bgColor,
                    TextColor = Color.FromHex("#111111"),
                    FontSize = 16,
                    WidthRequest = Units.ScreenWidth - 150,
                    HeightRequest = 40,
                    Margin = 2,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = phrase
                });
            }
        }

        public void HighlightFoundPhrases()
        {

            foreach (Entry phrase in PhraseList)
            {
                Color bgColor = Color.FromHex("#eeeeee");
                Color txtColor = Color.FromHex("#111111");

                if (Globals.FoundPhrases.Contains(phrase.Text))
                {
                    bgColor = Color.Red;
                    txtColor = Color.White;
                }

                phrase.BackgroundColor = bgColor;
                phrase.TextColor = txtColor;
            }
        }

        public void CreateNumberedDictionary()
        {
            DataInputScroller.Opacity = 0.2;
            NeoPhraseManager.BuildAllData();
            CreateDictionary(NeoPhraseManager.GetAvailablePhrases());

            PhraseListContainer.Children.Clear();

            int count = 0;
            foreach(KeyValuePair<int, string> pair in NeoPhraseManager.GetPhrasePairs())
            {


                PhraseListContainer.Children.Add(new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label
                        {
                            TextColor = Color.White,
                            FontSize = 12,
                            Text = pair.Key.ToString("D4")+":",
                            WidthRequest = 64,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            VerticalTextAlignment = TextAlignment.Center
                        },
                        PhraseList[count]

                    }
                });
                count++;
            }

            //Header.SetTitle("Vocabulary - " + PhraseList.Count + " phrases");
            DataInputScroller.Opacity = 1.0;
        }

        public void GenerateNewPhrases()
        {
            DataInputScroller.Opacity = 0.2;
            NeoPhraseManager.BuildAllData();
            CreateNumberedDictionary();
        }
    }
}





