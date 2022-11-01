using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using neolink.Helpers;
using neolink.Layouts;
using neolink.Pages;
using neolinke.Services;
using Plugin.DeviceInfo;
using Plugin.SimpleAudioPlayer;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace neolink
{
    public partial class MainPage : ContentPage
    {
        Grid PageContent;
        DataInputPage DataInputPage;
        DictionaryPage DictionaryPage;
        DownloadsPage DownloadsPage;
        DevicesPage DevicesPage;

        Header Header;
        Footer Footer;

        StackLayout WordList;
        StackLayout WordListContent;
        StackLayout WordListContainer;

        StackLayout InfoContent;
        ScrollView InfoScroller;

        Label SequenceString;
        Label MatchedPhrase;
        Label MatchedIndex;
        Label NeoActivityStatus;

        int CurrentEvent;
        int NextEvent;
        int LastEvent;

        DateTime StartTime;
        DateTime NextEventTime;
        DateTime CurrentTime;

        StoppableTimer BrainTimer;
        StoppableTimer ScanTimer;

        string connectedStatus = "DISCONNECTED";
        string streamStatus = "STANDING BY";

        ISimpleAudioPlayer AudioPlayer;

        List<string> LoggedWordsList;

        int WordMemory = 4;

        int effectAnimDelay = 50;
        int startDelay = 0;

        
        bool AllowSpecialMessage = false;

        static int EVENT_SPEED_FAST = 500;
        static int EVENT_SPEED_MEDIUM_FAST = 750;
        static int EVENT_SPEED_MEDIUM = 1250;
        static int EVENT_SPEED_MEDIUM_SLOW = 2000;
        static int EVENT_SPEED_SLOW = 2500;
        static int EVENT_SPEED_VERY_SLOW = 3000;

        int EventDelay = EVENT_SPEED_MEDIUM;
        
        int RunningSpeed = 10; // between 10 and 90
        int DelayBetweenEvents = 2000;

        // MAIN DELAY!! INCREASE TO SLOW DOWN

        bool showThinking = false;
        int thinkIndicatorCount = 0;
        int scanSkipCount = 0;

        public enum neolinkEvents
        {
            neolinkLoad,
            neolinkStartUp,
            neolinkStandBy,
            neolinkSwitchOn,
            neolinkBegin,
            neolinkRestart,
            neolinkSwitchOff,
            neolinkEvent,
            neolinkSpoken,
            neolinkEndOfStream
        };

        public string[] neolinkEventNames =
        {
            "neolinkLoad",
            "neolinkStartUp",
            "neolinkStandBy",
            "neolinkSwitchOn",
            "neolinkBegin",
            "neolinkRestart",
            "neolinkSwitchOff",
            "neolinkEvent"
        };

        NeoPhraseManager PhraseManager;

        Label SpeechToggle;
        Label VibrationToggle;
        Label SoundToggle;

        Label SaveHistory;
        Label CloseHistory;

        
        bool PerformingEvent;

        int SafeArea;

        int WORD_HISTORY = 0;
        int GENERAL_INFO = 1;

        int InfoType;

        Label WordHistory;
        Label WordHistoryLabel;

        List<int> VisibleSequence;
        Grid VisibleSequenceHolder;
        int VisibleSequenceLength = 5;




        Grid MainContent;
        Grid ContentContainer;

        public MainPage()
        {
            Header = new Header("Data Activity");
            Footer = new Footer();

            MainContent = new Grid { };

            CurrentEvent = (int)neolinkEvents.neolinkLoad;

            PhraseManager = new NeoPhraseManager();
            PhraseManager.PopulateDictionary();

            Globals.FoundPhrases = new List<string>();

            // build pages
            DataInputPage = new DataInputPage(NeoPhraseManager.GetTriggeredIndexStream());
            DictionaryPage = new DictionaryPage(NeoPhraseManager.GetAvailablePhrases());
            DownloadsPage = new DownloadsPage();
            DevicesPage = new DevicesPage();

            //DataInputPage.Footer = Footer;
            //DictionaryPage.Footer = Footer;
            //DownloadsPage.Footer = Footer;
            //DevicesPage.Footer = Footer;

            VisibleSequence = new List<int>();

            for(int i=0; i<VisibleSequenceLength; i++)
            {
                VisibleSequence.Add(-1);
            }

            startDelay = 100;

            Globals.Initialised = false;
            Globals.NeolinkOn = false;
            Globals.IsPaused = false;
            Globals.ShowDataInput = false;
            Globals.SpeechOn = true;
            Globals.VibrateOn = true;
            Globals.SoundOn = false;

            NeoActivityStatus = new Label
            {
                TextColor = Color.Cyan,
                FontFamily = Fonts.GetRegularAppFont(),
                FontSize = Units.DynamicFontSizeXXL,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.End,
                Text = "",
                Margin = new Thickness(0, 0, 0, 24)
            };

            InfoType = WORD_HISTORY;

            // iOS
            SafeArea = 24;

            if (Device.RuntimePlatform == Device.iOS)
            {
                if (CrossDeviceInfo.Current.VersionNumber.Major <= 10)
                {
                    SafeArea = 24;
                }
                else
                {
                    SafeArea = 44;
                }
            }


            AudioPlayer = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

            PageContent = new Grid
            {
                BackgroundColor = Color.FromHex("#000000"),
                WidthRequest = Units.ScreenWidth,
                HeightRequest = Units.ScreenHeight,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            BrainTimer = new StoppableTimer(TimeSpan.FromMilliseconds(500), TimedUpdate, true);

            SequenceString = new Label
            {
                TextColor = Color.White,
                FontFamily = Fonts.GetRegularAppFont(),
                FontSize = Units.DynamicFontSizeXXXXL,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            MatchedIndex = new Label
            {
                TextColor = Color.Red,
                FontFamily = Fonts.GetRegularAppFont(),
                FontSize = Units.DynamicFontSizeXXXXL,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness (Units.ScreenUnitL * 4, 0)
            };

            MatchedPhrase = new Label
            {
                TextColor = Color.Yellow,
                FontFamily = Fonts.GetRegularAppFont(),
                FontSize = Units.DynamicFontSizeXXXXL,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            VisibleSequenceHolder = new Grid();

            PageContent.Padding = new Thickness(Units.ScreenUnitL, 64);


            PageContent.Children.Add(Header.Content, 0, 0);





            MainContent.Children.Add(VisibleSequenceHolder, 0, 1);
            MainContent.Children.Add(MatchedPhrase, 0, 2);
            MainContent.Children.Add(MatchedIndex, 0, 3);
            MainContent.Children.Add(NeoActivityStatus, 0, 4);


            //PageContent.Children.Add(VisibleSequenceHolder, 0, 2);
            //PageContent.Children.Add(MatchedPhrase, 0, 3);
            //PageContent.Children.Add(MatchedIndex, 0, 4);
            //PageContent.Children.Add(NeoActivityStatus, 0, 5);

            ContentContainer = MainContent;

            PageContent.Children.Add(ContentContainer, 0, 1);

            PageContent.Children.Add(Footer.Content, 0, 6);

            Grid.SetRowSpan(ContentContainer, 5);


            Footer.Update();

            this.Content = PageContent;
        }

        public void SaveHistoryList()
        {
            CreateLog();
            DisplayAlert("Session Logged", "Your session has been saved to your device's clipboard. You can now paste this into an email, message, document or wherever you choose. Simply open up the app you'd like to use, then press and hold in a text area to bring up the option to paste your session.", "Ok");
        }

        public void SetMode(int mode)
        {
            EVENT_SPEED_FAST = mode;

            EVENT_SPEED_MEDIUM_FAST = EVENT_SPEED_FAST * 2;
            EVENT_SPEED_MEDIUM = EVENT_SPEED_FAST * 3;
            EVENT_SPEED_MEDIUM_SLOW = EVENT_SPEED_FAST * 4;
            EVENT_SPEED_SLOW = EVENT_SPEED_FAST * 5;
            EVENT_SPEED_VERY_SLOW = EVENT_SPEED_FAST * 6;
        }

        public void SetState(int state)
        {
            CurrentEvent = (int)state;
            Console.WriteLine("STATE: " + neolinkEventNames[CurrentEvent]);
        }

        public void SkipIntro()
        {

        }


        string wordsSpoken = "";
        string CurrentSequence = "";

        bool connectedToSouce = true;

        int getSequenceCalls = 0;
        int getSequenceSuccess = 0;
        int getSequenceMatches = 0;

        public void Reset()
        {
            wordsSpoken = "";
            CurrentSequence = "";

            VisibleSequence.Clear();

            for (int i = 0; i < VisibleSequenceLength; i++)
            {
                VisibleSequence.Add(-1);
            }

            connectedToSouce = true;

            getSequenceCalls = 0;
            getSequenceSuccess = 0;
            getSequenceMatches = 0;

            Globals.IsPaused = false;
        }

        public void SwitchOn()
        {
            Globals.IsPaused = false;
            PlaySound("click");
            startDelay = 100;
            StartTime = DateTime.Now;
            CurrentTime = DateTime.Now;
            NextEventTime = StartTime.AddMilliseconds(startDelay);
            
            Console.WriteLine("StartTime: " + StartTime.ToLongTimeString());

            BrainTimer.Start();
            //ScanTimer.Start();
            PerformingEvent = false;
            
            SetState((int)neolinkEvents.neolinkStartUp);


            UpdateElements();
            Restart();
            AddNumberToVisible(-1);
        }

        
        public void SwitchOff()
        {
            Reset();
            Speak("Off", false);

            VisibleSequenceHolder.Children.Clear();

            SequenceString.Text = "";
            MatchedIndex.Text = "";
            MatchedPhrase.Text = "";

            if (LoggedWordsList != null)
            {
                LoggedWordsList.Clear();
            }

            SwitchOffLights();
            PerformingEvent = false;
            BrainTimer.Stop();
            //ScanTimer.Stop();
            Globals.Initialised = false;
            UpdateElements();
        }

        public void Pause()
        {
            Globals.IsPaused = true;
            UpdateElements();
        }

        public void UnPause()
        {
            Globals.IsPaused = false;
            UpdateElements();
        }


        public void SwitchOnLights()
        {
            //LightStatus = LIGHTS_ON;
            UpdateLights();
        }

        public void SwitchOffLights()
        {
            //LightStatus = LIGHTS_OFF;
            UpdateLights();
        }

        public float GetRandomEffectOpacity()
        {
            var randEffectOpacity = new Random().NextDouble() * 1;

            if (randEffectOpacity > 0.4)
            {
                randEffectOpacity = 0.4;
            }
            else
            {
                randEffectOpacity = 0.2;
            }
            return (float)randEffectOpacity;
        }

        public void TimedUpdate()
        {
            if (Globals.IsPaused) return;

            if (PerformingEvent) return;

            Tick();
        }

        public void Restart()
        {
            // BACK TO THE BEGINNING
            Console.WriteLine("BACK TO THE BEGINNING");
            SetState((int)neolinkEvents.neolinkBegin);
        }

        public bool Tick()
        {
            if (Globals.IsPaused)
            {
                return false;
            }

            Console.WriteLine("Tick");

            CurrentTime = DateTime.Now;

            try
            {
                switch (CurrentEvent)
                {
                    case (int)neolinkEvents.neolinkLoad:
                        break;
                    case (int)neolinkEvents.neolinkStartUp:
                        break;
                    case (int)neolinkEvents.neolinkStandBy:
                        break;
                    case (int)neolinkEvents.neolinkSwitchOn:
                        break;
                    case (int)neolinkEvents.neolinkBegin:
                        DelayBetweenEvents = 5000;
                        NextEventTime = CurrentTime.AddMilliseconds(DelayBetweenEvents);
                        SetState((int)neolinkEvents.neolinkEvent);
                        PerformingEvent = false;
                        break;
                    case (int)neolinkEvents.neolinkEvent:
                        PerformAnEvent();
                        break;
                    case (int)neolinkEvents.neolinkSpoken:
                        SetState((int)neolinkEvents.neolinkBegin);
                        break;
                    case (int)neolinkEvents.neolinkEndOfStream:
                        SetState((int)neolinkEvents.neolinkStandBy);
                        break;
                    case (int)neolinkEvents.neolinkRestart:
                        break;
                    case (int)neolinkEvents.neolinkSwitchOff:
                        break;
                }
            }
            catch(Exception e)
            {

            }

            return true;
        }

        public int GetSequence()
        {
            if (connectedToSouce)
            {
                return GetConnectedSourceSequence();
            }

            return GetSimulatedSequence();
        }

        public int GetConnectedSourceSequence()
        {
            int sequence = PhraseManager.GetStreamIndex(getSequenceSuccess);
            getSequenceSuccess++;
            return sequence;
        }

        public int GetSimulatedSequence()
        {
            getSequenceCalls++;

            if (Numbers.GetNextRandom(0, 100) > 50)
            {
                return -1; // simulate no new number available
            }

            if (Numbers.GetNextRandom(0, 100) > 90)
            {
                return -1; // 
            }

            getSequenceSuccess++;

            return Numbers.GetNextRandom(0, 1024);
        }

        public void AddNumberToVisible(int num)
        {
            List<int> currentSeq = VisibleSequence;
            List<int> newSeq = new List<int>();

            newSeq.AddRange(currentSeq.GetRange(1, currentSeq.Count - 1));
            newSeq.Add(num);
            VisibleSequence = newSeq;
            Console.WriteLine("");

            VisibleSequenceHolder.Children.Clear();

            int count = 0;
            foreach(int number in VisibleSequence)
            {
                Console.WriteLine("NUM " + number);
                string numText = "-";
                if (number > -1)
                {
                    numText = $"{number}";
                }

                Color textColor = Color.White;
                int fontSize = Units.DynamicFontSizeXXXXL;
                string font = Fonts.GetRegularAppFont();

                if (PhraseManager.IsTriggerIndex(number))
                {
                    textColor = Color.Red;
                    fontSize = (int)(Units.DynamicFontSizeXXXXL * 1.5);
                    font = Fonts.GetBoldAppFont();
                }

                VisibleSequenceHolder.Children.Add(new Label
                {
                    TextColor = textColor,
                    FontFamily = font,
                    FontSize = fontSize,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = numText
                }, count, 0);

                count++;
            }
        }



        public void PerformAnEvent()
        {
            Console.WriteLine("Event");
            Console.WriteLine("GetSequence calls: " + getSequenceCalls);
            Console.WriteLine("GetSequence succsess: " + getSequenceSuccess);
            Console.WriteLine("GetSequence matches: " + getSequenceMatches);

            int triggerIndex = GetSequence();

            //if (triggerIndex > -1)
            //{

                AddNumberToVisible(triggerIndex);

                if (triggerIndex < 0)
                {
                    if (getSequenceSuccess > PhraseManager.GetStreamLength())
                    {
                        Globals.IsPaused = true;
                        DisplayAlert("DONE", "Reached the end of the data stream for this session", "Ok");

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            Console.WriteLine("Pause before reset...");
                            await Task.Delay(5000);
                            Console.WriteLine("... and reset");
                            //Reset();
                        });
                    }
                    
                    return;
                }

                //CurrentSequence = Numbers.GetRandomSequence(0, 1024, 3);
                CurrentSequence = "" + triggerIndex;

                //Console.WriteLine("Tick");
                Console.WriteLine(CurrentSequence);

                SequenceString.Text = CurrentSequence;

                string triggeredPhrase = "";

                if (PhraseManager.IsTriggerIndex(triggerIndex))
                {
                    getSequenceMatches++;
                    PerformingEvent = true; // stop any more events being fired

                    triggeredPhrase = PhraseManager.GetAvailablePhrase(triggerIndex);
                    MatchedIndex.Text = "MATCH FOUND\n" + triggerIndex;

                    MatchedPhrase.Text = triggeredPhrase;

                //SpeechOn = false;

                    Globals.FoundPhrases.Add(triggeredPhrase);
                    DictionaryPage.HighlightFoundPhrases();

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Speak(triggeredPhrase, false);
                        await Task.Delay(5000);
                        PerformingEvent = false;
                    });



                }
                else
                {
                    if (Numbers.GetNextRandom(0, 100) > 50)
                    {
                        streamStatus = "LISTENING";
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Task.Delay(Numbers.GetNextRandom(2000, 8000));

                        });
                    }
                    else
                    {
                        streamStatus = "RECEVING";
                    }
                }
            //}
            //else
            //{
            //    Console.WriteLine("Nothing new");
            //}
        }

        public string GetLoggedWords(int limit)
        {
            string loggedWords = "";
            int start = 0;
            int end = 0;

            if (LoggedWordsList == null)
            {
                LoggedWordsList = new List<string>();
            }

            if (LoggedWordsList.Count <= limit)
            {
                foreach (string word in LoggedWordsList)
                {
                    loggedWords += word + "\n";
                }
            }
            else
            {
                end = LoggedWordsList.Count - 1;
                start = end - (limit - 1);

                int count = 0;
                foreach (string word in LoggedWordsList)
                {
                    if (count >= start && count <= end)
                    {
                        loggedWords += word + "\n";
                    }
                    count++;
                }
            }

            return loggedWords;
        }

        public void Speak(string word, bool allowBeep)
        {
            if (CurrentEvent == (int)neolinkEvents.neolinkSpoken)
            {
                return; // already spoken, wait...
            }
            else
            {

                if (LoggedWordsList == null)
                {
                    LoggedWordsList = new List<string>();
                }
                string toSpeak = word.ToUpper();
                LoggedWordsList.Add(toSpeak);


                SwitchOnLights();
                UpdateLights();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    //SequenceString.Text = GetLoggedWords(WordMemory);

                    //SequenceString.Text = toSpeak.ToLower().Replace(" ", "\n");
                    //GetLoggedWords(WordMemory);

                    if (allowBeep)
                    {
                        PlaySound("beep");
                        if (Globals.SoundOn)
                        {
                            await Task.Delay(1750);
                        }
                    }


                    //SequenceString.Text = toSpeak.ToLower().Replace(" ", "\n"); GetLoggedWords(WordMemory);

                    if (Globals.SpeechOn)
                    {
                        await TextToSpeech.SpeakAsync(toSpeak.ToLower());
                    }

                    Console.WriteLine(toSpeak.ToLower());
                });




                Console.WriteLine("=========== WORD LIST ===========");
                string allWords = "";
                foreach (string w in LoggedWordsList)
                {
                    //Console.WriteLine(w);

                    allWords += w;// + ", ";
                }
                Console.WriteLine(allWords);





                UpdateElements();
                CurrentEvent = (int)neolinkEvents.neolinkSpoken;
            }

            //WordHistory.Text = GetLog();
        }

        public void ToggleMainOnOff()
        {
            Globals.NeolinkOn = !Globals.NeolinkOn;
            PlaySound("switch");

            if (Globals.NeolinkOn)
            {
                SwitchOn();
            }
            else
            {
                SwitchOff();
            }

            UpdateElements();
        }

        public void ToggleStartStop()
        {
            Globals.IsPaused = !Globals.IsPaused;
            PlaySound("switch");

            if (Globals.IsPaused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }

            UpdateElements();
        }

        public void ToggleLogOnOff()
        {

        }

        public void ToggleViewOnOff()
        {

        }

        public void ToggleSpeechOnOff()
        {

        }

        public void UpdateElements()
        {
            try
            {
                Footer.Update();

                if (Globals.NeolinkOn)
                {
                    connectedStatus = "CONNECTED";
                    streamStatus = "RECEIVING DATA";

                    NeoActivityStatus.TextColor = Color.Cyan;
                }
                else
                {
                    connectedStatus = "CONNECTED";
                    streamStatus = "WAITING";

                    NeoActivityStatus.TextColor = Color.DarkCyan; 
                }
            }
            catch(Exception e)
            {
                NeoActivityStatus.TextColor = Color.DarkRed;
                connectedStatus = "ERROR";
                streamStatus = "STAND BY";
            }

            NeoActivityStatus.Text = connectedStatus + "\n" + streamStatus;

        }



        public void UpdateLights()
        {
            
        }

        public void PlaySound(string file, int volume)
        {
            if (!Globals.SoundOn) { return; }

            try
            {
                //AudioPlayer.Volume = volume;
                AudioPlayer.Load(file + ".mp3");
                AudioPlayer.Play();
            }
            catch (Exception e)
            {

            }
        }

        public void PlaySound(string file)
        {
            if (!Globals.SoundOn) { return; }

            try
            {
                AudioPlayer.Load(file + ".mp3");
                AudioPlayer.Play();
            }
            catch (Exception e)
            {

            }
        }

        public void CreateLog()
        {
            Clipboard.SetTextAsync(GetLog());
        }

        public string GetLog()
        {
            string wordList = "";

            wordList +=
                "neolink Session Log\n\n" +
                "Date: " +
                DateTime.Now.ToLongDateString() +
                "\n" +
                "Time: " +
                DateTime.Now.ToShortTimeString() +
                "\n\n" + "neolink received the following:\n\n";


            if (LoggedWordsList != null)
            {
                foreach (string word in this.LoggedWordsList)
                {
                    wordList += word + "\n";
                }
            }
            return wordList;
        }


        public void GoToPage(int page)
        {
            Globals.CURRENT_PAGE = page;
            Globals.IsPaused = true;

            switch (Globals.CURRENT_PAGE)
            {
                case Globals.DATA_INPUT_PAGE:
                    //this.Content = DataInputPage.Content;
                    ContentContainer = DataInputPage.Content;
                    Header.SetTitle("Data Input");
                    //DataInputPage.UpdateStreamData();
                    break;
                case Globals.DICTIONARY_PAGE:
                    //this.Content = DictionaryPage.Content;
                    ContentContainer = DictionaryPage.Content;
                    Header.SetTitle("NeoLink Dictionary");
                    break;
                case Globals.DEVICES_PAGE:
                    //this.Content = DevicesPage.Content;
                    ContentContainer = DevicesPage.Content;
                    Header.SetTitle("NeoLinked Devices");
                    break;
                case Globals.DOWNLOADS_PAGE:
                    //this.Content = DownloadsPage.Content;
                    ContentContainer = DownloadsPage.Content;
                    Header.SetTitle("Downloadable Data");
                    break;
                default:
                    LoadDataStream(DataInputPage.GetInput());
                    ContentContainer = MainContent;
                    Header.SetTitle("Data Activity");
                    break;

            }

            //ContentContainer = MainContent;


            ContentContainer.WidthRequest = Units.ScreenWidth;
            ContentContainer.HeightRequest = Units.ScreenHeight;
            ContentContainer.VerticalOptions = LayoutOptions.StartAndExpand;
            ContentContainer.HorizontalOptions = LayoutOptions.CenterAndExpand;


            PageContent.Children.Clear();
            
            PageContent.Children.Add(ContentContainer, 0, 1);
            PageContent.Children.Add(Header.Content, 0, 0);
            PageContent.Children.Add(Footer.Content, 0, 6);

            Grid.SetRowSpan(ContentContainer, 5);


            Footer.Update();
        }



        public bool LoadDataStream(string dataStream)
        {
            if (PhraseManager.UpdateDataStream(dataStream))
            {
                DataInputPage.UpdateStreamData(dataStream);
                SwitchOff();
                Reset();
                SwitchOn();
                return true;
            }
            return false;
        }
    }
}
