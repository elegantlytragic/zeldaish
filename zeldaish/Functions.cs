using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zeldaish
{
    class Functions
    {
        static public void loadheader(String name, out int width, out int height)
        {
            using (BinaryReader br = new BinaryReader(File.Open(name + ".dat", FileMode.Open)))
            {
                width = br.ReadInt32();
                height = br.ReadInt32();
            }
        }
        static public void DrawBorderedText(SpriteFont font, SpriteBatch spriteBatch, String text, Vector2 position,
            Color borderColor, Color color)
        {
            spriteBatch.DrawString(font, text, position + new Vector2(1, 1), borderColor);
            spriteBatch.DrawString(font, text, position + new Vector2(-1, -1), borderColor);
            spriteBatch.DrawString(font, text, position + new Vector2(-1, 1), borderColor);
            spriteBatch.DrawString(font, text, position + new Vector2(1, -1), borderColor);
            spriteBatch.DrawString(font, text, position, color);
        }
        static public void createlevel(String levelname, int width, int height)
        {
            #region header
            int width2 = width, height2 = height;
            String[] dest = new String[6] { "null", "null", "null", "null", "null", "null" };
            int[] destcoords = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            #endregion
            int[,] data = new int[width, height], coldata = new int[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    data[x, y] = 0;
                    coldata[x, y] = 0;
                }
            }
            using (BinaryWriter bw = new BinaryWriter(File.Open(levelname + ".dat", FileMode.OpenOrCreate)))
            {
                bw.Write(width);
                bw.Write(height);
                for (int n = 0; n < dest.Length; n++) bw.Write(dest[n]);
                for (int i = 0; i < destcoords.Length; i++) bw.Write(destcoords[i]);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++) bw.Write(data[x, y]);
                }
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++) bw.Write(coldata[x, y]);
                }
            }
        }
        static public void inittexts(Level level, Textbox[] selects, SpriteFont font)
        {
            for (int n = 0; n <= 19; n++)
            {
                if (n <= 5) selects[n] = new Textbox(font, level.dests[n], Color.Black, new Vector2(300, 100 + (n * 20)));
                else if (n >= 6 && n <= 17) selects[n] = new Textbox(font, level.destcoords[n - 6].ToString(), Color.Black, new Vector2(300, 100 + (n * 20)));
                else if (n == 18) selects[n] = new Textbox(font, level.width.ToString(), Color.Black, new Vector2(300, 100 + (n * 20)));
                else if (n == 19) selects[n] = new Textbox(font, level.height.ToString(), Color.Black, new Vector2(300, 100 + (n * 20)));
            }
        }
        static public void savedests(Level level, Textbox[] selects)
        {
            for (int n = 0; n < 19; n++)
            {
                if (n <= 5) level.dests[n] = selects[n].value;
                else if (n >= 5 && n <= 17) level.destcoords[n - 6] = Convert.ToInt32(selects[n].value, 10);
                else { level.width = Convert.ToInt32(selects[n].value, 10); level.height = Convert.ToInt32(selects[n].value, 10); }
            }
        }
        static public bool isbetween(int value, int val1, int val2, int value2, int val3, int val4)
        {
            if (value >= val1 && value <= val2 && value2 >= val3 && value2 <= val4) return true;
            else return false;
        }
    }
}
