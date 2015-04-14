using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace zeldaish
{
    class Textbox
    {
        public String value;
        public Color color;
        public bool focused;
        private Vector2 position;
        private SpriteFont font;
        private InputHelper help;
        public Textbox(SpriteFont font, String defval, Color color, Vector2 pos)
        {
            this.font = font;
            value = defval;
            this.color = color;
            position = pos;
            help = new InputHelper();
            focused = false;
        }
        public void Update(Keys[] keys)
        {
            if (focused)
            {
                help.Update();
                foreach (Keys key in keys)
                {
                    if (help.IsNewPress(key))
                    {
                        if (key == Keys.Back) value = value.Remove(value.Length - 1, 1);
                        else if (key == Keys.Enter || key == Keys.Escape) focused = false;
                        else if (key == Keys.OemMinus) value += "-";
                        else if (key == Keys.D0) value += "0";
                        else if (key == Keys.D1) value += "1";
                        else if (key == Keys.D2) value += "2";
                        else if (key == Keys.D3) value += "3";
                        else if (key == Keys.D4) value += "4";
                        else if (key == Keys.D5) value += "5";
                        else if (key == Keys.D6) value += "6";
                        else if (key == Keys.D7) value += "7";
                        else if (key == Keys.D8) value += "8";
                        else if (key == Keys.D9) value += "9";
                        else value += key.ToString().ToLower();
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Functions.DrawBorderedText(font, spriteBatch, value, position, color, Color.White);
        }
    }
}
