using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace neolink
{
    public static class Globals
    {
        public const int MAIN_PAGE = 0;
        public const int DATA_INPUT_PAGE = 1;
        public const int DICTIONARY_PAGE = 2;
        public const int DEVICES_PAGE = 3;
        public const int DOWNLOADS_PAGE = 4;

        public static int CURRENT_PAGE;

        public static bool Initialised;
        public static bool NeolinkOn;
        public static bool IsPaused;
        public static bool ShowDataInput;
        public static bool SpeechOn;
        public static bool VibrateOn;
        public static bool SoundOn;

        public static List<string> FoundPhrases = new List<string>();
    }
}


