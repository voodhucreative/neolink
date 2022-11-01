using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace neolink.Helpers
{
    public static class Fonts
    {
        public enum FontName
        {
            TechnologyRegular,
            TechnologyBold
        }
        static Dictionary<FontName, string> fontDictionary;

        public static void Init()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                fontDictionary = new Dictionary<FontName, string>
                {
                    [FontName.TechnologyRegular] = "Technology.ttf#Technology",
                    [FontName.TechnologyBold] = "Technology-Bold.ttf#TechnologyBold"
                };
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                fontDictionary = new Dictionary<FontName, string>
                {
                    [FontName.TechnologyRegular] = "Technology",
                    [FontName.TechnologyBold] = "Technology-Bold"
                };
            }
        }

        public static string GetFont(FontName font)
        {
            return fontDictionary[font];
        }

        public static string GetRegularAppFont()
        {
            return GetFont(FontName.TechnologyRegular);
        }

        public static string GetBoldAppFont()
        {
            return GetFont(FontName.TechnologyBold);
        }
    }
}

