using System.IO;
using SFML.Graphics;

namespace Collective_learning.GUI.BasicControllers
{
    internal class ButtonSettings
    {
        public const uint TextSize = 24;
        public static readonly Font DefaultFont = new Font(Path.Combine("Assets", "Arial.ttf"));
        public static readonly Color TextColor = new Color(128, 128, 128);
        public static readonly Text.Styles TextStyle = Text.Styles.Regular;

        public static readonly Color ShapeColor = new Color(128, 128, 128);
        public static readonly Color ShapeClickColor = new Color(64, 64, 64);
        public static readonly Color ShapeOutlineColor = new Color(115, 124, 138);
        public static readonly float TextWidthMargin = TextSize/2f;
        public static readonly float TextHeightMargin = TextSize;


        public static readonly Texture Texture = new Texture((Path.Combine("Assets", "buttonOutM.png")));
        public static readonly int TextureWidth = 106;
        public static readonly int TextureHeight = 36;
        public static readonly Texture TextureOver = new Texture((Path.Combine("Assets", "buttonM.png")));
    }
}