using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace zeldaish
{
    static class Functions
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
    }
    public class Level
    {
        public int[,] data, col;
        public int[] destcoords;
        public int width, height;
        public String[] dests;
        private String levelname;
        private Texture2D tex;
        public int X, Y;
        private int walltile;
        public Level(String name, int defaultx, int defaulty, int width, int height, Texture2D tileset, int walltile)
        {
            levelname = name;
            this.width = width;
            this.height = height;
            data = new int[width, height];
            col = new int[width, height];
            X = defaultx;
            Y = defaulty;
            dests = new String[6];
            destcoords = new int[12];
            tex = tileset;
            this.walltile = walltile;
            using (BinaryReader br = new BinaryReader(File.Open(name + ".dat", FileMode.Open)))
            {
                width = br.ReadInt32();
                height = br.ReadInt32();
                for (int n = 0; n < dests.Length; n++) dests[n] = br.ReadString();
                for (int i = 0; i < destcoords.Length; i++) destcoords[i] = br.ReadInt32();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++) data[x, y] = br.ReadInt32();
                }
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++) col[x, y] = br.ReadInt32();
                }
            }
        }
        public int Update(Rectangle player, ref Level buffer)
        {
            int width2, height2;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Rectangle cur = new Rectangle(x * 48 + X, y * 48 + Y, 48, 48);
                    if (col[x, y] == 1)
                    {
                        if (cur.Contains(player.Right, player.Y + player.Height / 2)) X += 3;
                        if (cur.Contains(player.Left, player.Y + player.Height / 2)) X -= 3;
                        if (cur.Contains(player.X + player.Width / 2, player.Top)) Y -= 3;
                        if (cur.Contains(player.X + player.Width / 2, player.Bottom)) Y += 3;
                    }
                    if (col[x, y] == 2) { if (cur.Intersects(player)) { Functions.loadheader(dests[0], out width2, out height2); buffer = new Level(dests[0], destcoords[0], destcoords[1], width2, height2, this.tex, 36); return 1; } }
                    if (col[x, y] == 3) { if (cur.Intersects(player)) { Functions.loadheader(dests[1], out width2, out height2); buffer = new Level(dests[1], destcoords[2], destcoords[3], width2, height2, this.tex, 36); return 1; } }
                    if (col[x, y] == 4) { if (cur.Intersects(player)) { Functions.loadheader(dests[2], out width2, out height2); buffer = new Level(dests[2], destcoords[4], destcoords[5], width2, height2, this.tex, 36); return 1; } }
                    if (col[x, y] == 5) { if (cur.Intersects(player)) { Functions.loadheader(dests[3], out width2, out height2); buffer = new Level(dests[3], destcoords[6], destcoords[7], width2, height2, this.tex, 36); return 1; } }
                    if (col[x, y] == 6) { if (cur.Intersects(player)) { Functions.loadheader(dests[4], out width2, out height2); buffer = new Level(dests[4], destcoords[8], destcoords[9], width2, height2, this.tex, 36); return 1; } }
                    if (col[x, y] == 7) { if (cur.Intersects(player)) { Functions.loadheader(dests[5], out width2, out height2); buffer = new Level(dests[5], destcoords[10], destcoords[11], width2, height2, this.tex, 36); return 1; } }
                }
            }
            return 0;
        }
        public void Draw(SpriteBatch spriteBatch, bool editor)
        {
            Rectangle spriteRect = new Rectangle(), cur = new Rectangle();
            if (!editor)
            {
                for (int x = -14; x < width + 14; x++)
                {
                    for (int y = -8; y < height + 5; y++)
                    {
                        spriteRect = new Rectangle((walltile % 10) * 48, (walltile / 10) * 48, 48, 48);
                        if ((x >= 0 && y >= 0) &&
                            (x <= width - 1 && y <= height - 1)) spriteRect = new Rectangle((data[x, y] % 10) * 48, (data[x, y] / 10) * 48, 48, 48);
                        cur = new Rectangle(x * 48 + X, y * 48 + Y, 48, 48);
                        spriteBatch.Draw(tex, cur, spriteRect, Color.White);
                    }
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        spriteRect = new Rectangle((data[x, y] % 10) * 48, (data[x, y] / 10) * 48, 48, 48);
                        cur = new Rectangle(x * 48 + X, y * 48 + Y, 48, 48);
                        spriteBatch.Draw(tex, cur, spriteRect, Color.White);
                    }
                }
            }
        }
        public void Save()
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(levelname + ".dat", FileMode.Open)))
            {
                bw.Write(width);
                bw.Write(height);
                for (int n = 0; n < dests.Length; n++) bw.Write(dests[n]);
                for (int i = 0; i < destcoords.Length; i++) bw.Write(destcoords[i]);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++) bw.Write(data[x, y]);
                }
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++) bw.Write(col[x, y]);
                }
            }
        }
    }
}
