using System;
using neolink.Helpers;
using neolink.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace neolink
{
    public partial class App : Application
    {
        static MainPage mainPage;
        public App(int width, int height, int pixelWidth, int pixelHeight, int statusBarHeight)
        {
            InitializeComponent();

            DeviceDisplay.KeepScreenOn = true;
            Fonts.Init();
            Units.Init(width, height, pixelWidth, pixelHeight, statusBarHeight);

            mainPage = new MainPage();
            MainPage = mainPage;
        }

        public static void GoToPage(int page)
        {
            mainPage.GoToPage(page);
        }


        public static void ToggleOnOff()
        {
            mainPage.ToggleMainOnOff();
        }


        public static bool LoadDataStream(string dataStream)
        {
            if (mainPage.LoadDataStream(dataStream))
            {
                return true;
            }
            return false;

        }

        public static void DisplayAlert(string title, string message, string cancel)
        {
            mainPage.DisplayAlert(title, message, cancel);
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}

