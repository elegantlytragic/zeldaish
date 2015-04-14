using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace zeldaish
{
    class Sprite
    {
        private float timer, interval;
        private int curFrame, maxFrames, idleFrame;
        private Texture2D tex;
        public bool Static = false;
        public int Animation;
        public Rectangle SpriteRect, Bounds, Collision;
        public Sprite(int maxFrames, int idleFrame, int defaultFrame, float interval, int width, int height, int x, int y, Texture2D tex, Rectangle col)
        {
            timer = 0f;
            Animation = defaultFrame;
            curFrame=0;
            this.maxFrames = maxFrames;
            this.idleFrame = idleFrame;
            this.interval = interval;
            Bounds = new Rectangle(x, y, width, height);
            Collision = col;
            this.tex = tex;
            SpriteRect = new Rectangle(curFrame * Bounds.Width, Animation * Bounds.Height, Bounds.Width, Bounds.Height);
        }
        public void Update(GameTime gameTime)
        {
            if (!Static)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timer >= interval)
                {
                    timer = 0;
                    curFrame++;
                    if (curFrame >= maxFrames) curFrame = 0;
                }
                SpriteRect.X = curFrame * Bounds.Width;
                SpriteRect.Y = Animation * Bounds.Height;
            }
            else SpriteRect = new Rectangle(idleFrame * Bounds.Width, Animation * Bounds.Height, Bounds.Width, Bounds.Height);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, Bounds, SpriteRect, Color.White);
        }
    }
}
