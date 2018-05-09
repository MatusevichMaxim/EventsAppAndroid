using System;
using Android.Graphics;

namespace EventApp.Helpers
{
    public class Constants
    {
        private static Constants thisRef;

        public static Constants Instance
        {
            get
            {
                if (thisRef == null)
                    thisRef = new Constants();

                return thisRef;
            }
        }

        public Color MainRed;
        public Color MainCyan;
        public Color MainGrey;
        public Typeface AEH;

        public Constants() { }

        public void InitializeConstants(Android.Content.Res.AssetManager assets)
        {
            AEH = Typeface.CreateFromAsset(assets, "AEH.ttf");

            MainRed = new Color(192, 46, 50);
            MainCyan = new Color(5, 194, 208);
            MainGrey = new Color(48, 64, 63);
        }
    }
}
