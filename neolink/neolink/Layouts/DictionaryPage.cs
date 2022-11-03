using System;
using System.Collections.Generic;
using neolink.Helpers;
using neolink.Layouts;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Entry = Xamarin.Forms.Entry;
using Picker = Xamarin.Forms.Picker;
using ScrollView = Xamarin.Forms.ScrollView;

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

        int smallIconSize;

        int dataInputFontSize;

        int dataInputHeight;

        int labelFontSize;

        int keyWidth;
        int phraseWidth;


        CheckBox UseWords;
        CheckBox UsePhrases;
        CheckBox UseSentences;

        int Selected = 1;


        Dictionary<string, int> ComplexityLevels;

        Picker ComplexityPicker;

        public DictionaryPage(List<string> phrases)
        {
            smallIconSize = Units.ScreenWidth10Percent;
            if (smallIconSize > 48) { smallIconSize = 48; };

            dataInputFontSize = Units.DynamicFontSizeXL;
            if (dataInputFontSize > 16) { dataInputFontSize = 16; }

            labelFontSize = Units.DynamicFontSizeXL;
            if (labelFontSize > 16) { labelFontSize = 16; }

            keyWidth = Units.ScreenWidth15Percent;

            dataInputHeight = dataInputFontSize * 2;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                dataInputHeight = dataInputFontSize * 3;
            }

            phraseWidth = Units.ScreenWidth - (keyWidth + (Units.ScreenUnitL + Units.ScreenUnitL));




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
                WidthRequest = smallIconSize,
                HeightRequest = smallIconSize,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = 8
            };


            ComplexityLevels = new Dictionary<string, int>
            {
                { "Lowest", 1 },
                { "Low", 2 },
                { "Average", 3 },
                { "Above Average", 4 },
                { "Smart", 5 },
                { "Very Smart", 6},
                { "Genius", 7 },
            };

            ComplexityPicker = new Picker
            {
                Title = "",
                FontSize = 12,
                HeightRequest = 24,
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                SelectedIndex = 0,
                WidthRequest = Units.HalfScreenWidth
            };

            foreach (string colorName in ComplexityLevels.Keys)
            {
                ComplexityPicker.Items.Add(colorName);
            }

            ComplexityPicker.SelectedIndex = 0;
            

            UseWords = new CheckBox
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsChecked = Globals.UseSingleWords,
                IsEnabled = false,
                Opacity = 0.5
            };

            UsePhrases = new CheckBox
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsChecked = Globals.UsePhrases
            };

            UseSentences = new CheckBox
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsChecked = Globals.UseSentences
            };

            UseWords.CheckedChanged += (sender, e) =>
            {
                //UpdateCheckBoxes();
                
            };

            UsePhrases.CheckedChanged += (sender, e) =>
            {
                UpdateCheckBoxes();
            };

            UseSentences.CheckedChanged += (sender, e) =>
            {
                UpdateCheckBoxes();
            };

            StackLayout VocabularyOptions = new StackLayout
            {

                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    /*
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Padding = 4,
                        Children = {
                            UseWords,
                            new Label { TextColor = Color.White, FontSize = 10, Text = "Words", HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center}
                        }
                    },*/
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Padding = 4,
                        Children = {
                            UsePhrases,
                            new Label { TextColor = Color.White, FontSize = 10, Text = " + Phrases", HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Padding = 4,
                        Children = {
                            UseSentences,
                            new Label { TextColor = Color.White, FontSize = 10, Text = " + Sentences", HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center} 
                        }
                    }
                }
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
                //Margin = new Thickness(8, 8, 8, 8)
            };

            Globals.UseSingleWords = true;
            UpdateCheckBoxes();

            CreateNumberedDictionary();
 
            RandomiseData.GestureRecognizers.Add(
                 new TapGestureRecognizer()
                 {
                     Command = new Command(() =>
                     {
                         if (Selected >= 1)
                         {
                             GenerateNewPhrases();
                         }
                     })
                 }
             );

            KeysLabel = new Label
            {
                FontSize = labelFontSize,
                FontAttributes = FontAttributes.Bold,
                Text = "Keys",
                TextColor = Color.Cyan,
                WidthRequest = keyWidth,
                Margin = new Thickness(0, 16)
                //Margin = new Thickness(0, 0, 0, 0)
            };

            PhrasesLabel = new Label
            {
                FontSize = labelFontSize,
                FontAttributes = FontAttributes.Bold,
                Text = "Session Phrases",
                TextColor = Color.Cyan,
                WidthRequest = phraseWidth,
                Margin = new Thickness(0, 16)
                //Margin = new Thickness(24, 0, 0, 0)
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
                            new Label
                            {
                                TextColor = Color.White,
                                Text = "Complexity",
                                FontSize = 12,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                VerticalTextAlignment = TextAlignment.Center
                            },
                            ComplexityPicker,
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
                //Margin = new Thickness(0, 16, 0, 8)

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
                    BackgroundColor = Color.FromHex("#eeeeee"),
                    TextColor = Color.FromHex("#111111"),
                    FontSize = dataInputFontSize,
                    WidthRequest = phraseWidth,
                    HeightRequest = dataInputHeight,
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FlowDirection = FlowDirection.LeftToRight,
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
                            FontSize = labelFontSize,
                            Text = pair.Key.ToString("D4")+":",
                            WidthRequest = keyWidth,
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
            int complexity = ComplexityPicker.SelectedIndex+1;


            DataInputScroller.Opacity = 0.2;
            NeoPhraseManager.SetVocabularyComplexity(complexity);
            NeoPhraseManager.BuildAllData();
            CreateNumberedDictionary();
        }

        public void UpdateCheckBoxes()
        {

            Selected = 0;

            Globals.UseSingleWords = UseWords.IsChecked;
            Globals.UsePhrases = UsePhrases.IsChecked;
            Globals.UseSentences = UseSentences.IsChecked;

            //UseWords.IsChecked = Globals.UseSingleWords;
            //UsePhrases.IsChecked = Globals.UsePhrases;
            //UseSentences.IsChecked = Globals.UseSentences;

            if (Globals.UseSingleWords)
            {
                Selected++;
            }

            if (Globals.UsePhrases)
            {
                Selected++;
            }

            if (Globals.UseSentences)
            {
                Selected++;
            }

            RandomiseData.IsEnabled = true;
            RandomiseData.Opacity = 1.0;

            if (Selected == 0)
            {
                RandomiseData.IsEnabled = false;
                RandomiseData.Opacity = 0.5;
            }


        }
    }
}





