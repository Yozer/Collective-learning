using System;
using System.IO;
using SFML.Graphics;
using SFML.System;


namespace Collective_learning.GUI.BasicControllers{

    internal class SliderSettings{
        public const uint TextSize = 24;
        public static readonly Font DefaultFont = new Font(Path.Combine("Assets","Arial.ttf"));
        public static readonly Color TextColor = new Color(128,128,128);
        public static readonly Text.Styles TextStyle = Text.Styles.Regular;

        public static readonly Color BarColor = new Color(112,112,112);
        public static readonly Color ScrollColor = new Color(104,106,101);

        public static readonly float TextHeightMargin = TextSize*1.5f;
        public static readonly int BarWidth = 220;
        public static readonly int BarHeight = 10;
        public static readonly int ScrollWidth = BarHeight;
        public static readonly int ScrollHeight = 3*BarHeight;

        


    }

}
