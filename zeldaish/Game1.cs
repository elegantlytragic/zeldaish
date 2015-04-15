using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace zeldaish
{
    enum GameState
    {
        editorselect, editor, menu, play, destselect
    }
    enum EditorState
    {
        tiles, grid, col
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Sprite link;
        Level level, buffer, editor;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        GameState gameState;
        EditorState editorState;
        InputHelper help;
        Textbox[] selects;
        int levelselect = 1, tileselected = 0, destselect = 0, hearts = 3;
        bool destselected = false;
        Vector2 gridselect = new Vector2(0, 0);
        String levelselectname;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
            gameState = GameState.menu;
            editorState = EditorState.tiles;
            help = new InputHelper();
            selects = new Textbox[20];
        }
        protected override void LoadContent()
        {
            int width2, height2;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Rectangle col = new Rectangle(690, 424, 31, 15);
            link = new Sprite(6, 3, 2, 75f, 48, 60, 683, 384, Content.Load<Texture2D>("link"), col);
            Functions.loadheader("dungeon1-001", out width2, out height2);
            level = new Level("dungeon1-001", 235, -42, width2, height2, Content.Load<Texture2D>("temple"), 36);
            font = Content.Load<SpriteFont>("dialogue");
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();
            help.Update();
            MouseState mouse = Mouse.GetState();
            int speed = 3, width2, height2;
            #region play
            if (gameState == GameState.play)
            {
                if (keys.IsKeyDown(Keys.W)) { link.Animation = 3; link.Static = false; level.Y += speed; }
                if (keys.IsKeyDown(Keys.S)) { link.Animation = 2; link.Static = false; level.Y -= speed; }
                if (keys.IsKeyDown(Keys.A)) { link.Animation = 1; link.Static = false; level.X += speed; }
                if (keys.IsKeyDown(Keys.D)) { link.Animation = 0; link.Static = false; level.X -= speed; }
                if (keys.IsKeyUp(Keys.W) && 
                    keys.IsKeyUp(Keys.S) && 
                    keys.IsKeyUp(Keys.A) &&
                    keys.IsKeyUp(Keys.D)) link.Static = true;
                link.Update(gameTime);
                if (level.Update(link.Collision, ref buffer) == 1) level = buffer;
            }
            #endregion
            #region menu
            if (gameState == GameState.menu)
            {
                if (keys.IsKeyDown(Keys.Space)) gameState = GameState.play;
                if (keys.IsKeyDown(Keys.E)) gameState = GameState.editorselect;
            }
            #endregion
            #region editorselect
            if (gameState == GameState.editorselect)
            {
                if (help.IsNewPress(Keys.Up)) levelselect++;
                if (help.IsNewPress(Keys.Down)) levelselect--;
                if (keys.IsKeyDown(Keys.Enter))
                {
                    levelselectname = "dungeon1-00" + levelselect;
                    if (File.Exists(levelselectname + ".dat"))
                    {
                        Functions.loadheader(levelselectname, out width2, out height2);
                        editor = new Level(levelselectname, 550, 0, width2, height2, Content.Load<Texture2D>("temple"), 36);
                    }
                    else
                    {
                        Functions.loadheader(levelselectname, out width2, out height2);
                        Functions.createlevel(levelselectname, 10, 10); editor = new Level(levelselectname, 550, 0, width2, height2, Content.Load<Texture2D>("temple"), 36);
                    }
                    gameState = GameState.editor;
                    Functions.inittexts(editor, selects, font);
                }
            }
            #endregion
            #region editor
            if (gameState == GameState.editor)
            {
                if (editorState == EditorState.tiles)
                {
                    if (help.IsNewPress(Keys.Down)) { if (tileselected <= 69) tileselected += 10; }
                    if (help.IsNewPress(Keys.Up)) { if (tileselected >= 10) tileselected -= 10; }
                    if (help.IsNewPress(Keys.Right)) { if (tileselected <= 78) tileselected++; }
                    if (help.IsNewPress(Keys.Left)) { if (tileselected >= 1) tileselected--; }
                    if (help.IsNewPress(Keys.Tab)) editorState = EditorState.grid;
                    if (help.IsNewPress(Keys.D)) gameState = GameState.destselect;
                }
                else if (editorState == EditorState.grid)
                {
                    if (keys.IsKeyDown(Keys.W)) editor.Y += 5;
                    if (keys.IsKeyDown(Keys.A)) editor.X += 5;
                    if (keys.IsKeyDown(Keys.S)) editor.Y -= 5;
                    if (keys.IsKeyDown(Keys.D)) editor.X -= 5;
                    if (help.IsNewPress(Keys.Down)) { if (gridselect.Y <= editor.height - 2) gridselect.Y++; }
                    if (help.IsNewPress(Keys.Up)) { if (gridselect.Y >= 1) gridselect.Y--; }
                    if (help.IsNewPress(Keys.Right)) { if (gridselect.X <= editor.width - 2) gridselect.X++; }
                    if (help.IsNewPress(Keys.Left)) { if (gridselect.X >= 1) gridselect.X--; }
                    if (help.IsNewPress(Keys.Tab)) editorState = EditorState.tiles;
                    if (help.IsNewPress(Keys.C)) editorState = EditorState.col;
                    if (help.IsNewPress(Keys.Enter)) editor.data[(int)gridselect.X, (int)gridselect.Y] = tileselected;
                }
                else if (editorState == EditorState.col)
                {
                    if (keys.IsKeyDown(Keys.W)) editor.Y += 5;
                    if (keys.IsKeyDown(Keys.A)) editor.X += 5;
                    if (keys.IsKeyDown(Keys.S)) editor.Y -= 5;
                    if (keys.IsKeyDown(Keys.D)) editor.X -= 5;
                    if (help.IsNewPress(Keys.Down)) { if (gridselect.Y <= editor.height - 2) gridselect.Y++; }
                    if (help.IsNewPress(Keys.Up)) { if (gridselect.Y >= 1) gridselect.Y--; }
                    if (help.IsNewPress(Keys.Right)) { if (gridselect.X <= editor.width - 2) gridselect.X++; }
                    if (help.IsNewPress(Keys.Left)) { if (gridselect.X >= 1) gridselect.X--; }
                    if (help.IsNewPress(Keys.C)) editorState = EditorState.grid;
                    if (help.IsNewPress(Keys.D0)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 0;
                    if (help.IsNewPress(Keys.D1)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 1;
                    if (help.IsNewPress(Keys.D2)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 2;
                    if (help.IsNewPress(Keys.D3)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 3;
                    if (help.IsNewPress(Keys.D4)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 4;
                    if (help.IsNewPress(Keys.D5)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 5;
                    if (help.IsNewPress(Keys.D6)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 6;
                    if (help.IsNewPress(Keys.D7)) editor.col[(int)gridselect.X, (int)gridselect.Y] = 7;
                }
                if (help.IsNewPress(Keys.L)) editor.Save();
            }
            #endregion
            #region destselect
            if (gameState == GameState.destselect)
            {
                for (int n = 0; n < 19; n++)
                {
                    if (n == destselect) selects[n].color = Color.Red;
                    else selects[n].color = Color.Blue;
                }
                if (!destselected)
                {
                    if (help.IsNewPress(Keys.Up)) { if (destselect >= 1) destselect--; }
                    if (help.IsNewPress(Keys.Down)) { if (destselect <= 17) destselect++; }
                    if (help.IsNewPress(Keys.Enter)) destselected = true;
                    if (help.IsNewPress(Keys.Tab)) gameState = GameState.editor;
                }
                else
                {
                    selects[destselect].focused = true;
                    selects[destselect].Update(keys.GetPressedKeys());
                    if (help.IsNewPress(Keys.Enter)) { selects[destselect].focused = false; destselected = false; Functions.savedests(editor, selects); }
                }
            }
            #endregion
            if (help.IsNewPress(Keys.M)) gameState = GameState.menu;
            if (keys.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) this.Exit();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            Vector2 selectpos = new Vector2((tileselected % 10) * 48 - 5, (tileselected / 10) * 48 - 5),
                gridpos = new Vector2(gridselect.X * 48 + 495, gridselect.Y * 48 - 5);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            #region play
            if (gameState == GameState.play)
            {
                level.Draw(spriteBatch, false);
                link.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("item"), new Vector2(100, 75), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("life"), new Vector2(1065, 25), Color.White);
                for (int n = 0; n < hearts; n++)
                {
                    if (n <= 9) spriteBatch.Draw(Content.Load<Texture2D>("heart1"), new Vector2(986 + (n * 33), 75), Color.White);
                    else spriteBatch.Draw(Content.Load<Texture2D>("heart1"), new Vector2(986 + ((n - 10) * 33), 108), Color.White);
                }
            }
            #endregion
            #region menu
            if (gameState == GameState.menu)
            {
                Functions.DrawBorderedText(font, spriteBatch, "Press Space to play, and E to edit levels.", new Vector2(683, 384),
                    Color.Blue, Color.White);
            }
            #endregion
            #region editorselect
            if (gameState == GameState.editorselect)
            {
                Functions.DrawBorderedText(font, spriteBatch, "Level " + levelselect, new Vector2(320, 320),
                    Color.Blue, Color.White);
            }
            #endregion
            #region editor
            if (gameState == GameState.editor)
            {
                editor.Draw(spriteBatch, false);
                spriteBatch.Draw(Content.Load<Texture2D>("editorui"), Vector2.Zero, Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("temple"), new Vector2(0, 0), Color.White);
                if (editorState == EditorState.tiles)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("select2"), selectpos, Color.White);
                    spriteBatch.Draw(Content.Load<Texture2D>("gridselect"), new Vector2(gridpos.X + editor.X - 500, gridpos.Y + editor.Y), Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "Arrow Keys - Move Selection, Tab - Switch to Grid Editing,",
                        new Vector2(50, 600), Color.Blue, Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "D - Switch to Destination Editing",
                        new Vector2(50, 620), Color.Blue, Color.White);
                }
                if (editorState == EditorState.grid)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("select"), selectpos, Color.White);
                    spriteBatch.Draw(Content.Load<Texture2D>("gridselect2"), new Vector2(gridpos.X + editor.X - 500, gridpos.Y + editor.Y), Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "Arrow Keys - Move Selection, Enter - Place Tile,",
                        new Vector2(50, 600), Color.Blue, Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "Tab - Switch to Tile Selecting, C - Switch to Colission Editing",
                        new Vector2(50, 620), Color.Blue, Color.White);
                }
                if (editorState == EditorState.col)
                {
                    for (int x = 0; x < editor.width; x++)
                    {
                        for (int y = 0; y < editor.height; y++)
                        {
                            Rectangle destrect = new Rectangle(x * 48 + editor.X, y * 48 + editor.Y, 48, 48);
                            if (editor.col[x, y] == 0) spriteBatch.Draw(Content.Load<Texture2D>("notsolid"), destrect, Color.White);
                            if (editor.col[x, y] == 1) spriteBatch.Draw(Content.Load<Texture2D>("solid"), destrect, Color.White);
                            if (editor.col[x, y] == 2) spriteBatch.Draw(Content.Load<Texture2D>("left"), destrect, Color.White);
                            if (editor.col[x, y] == 3) spriteBatch.Draw(Content.Load<Texture2D>("up"), destrect, Color.White);
                            if (editor.col[x, y] == 4) spriteBatch.Draw(Content.Load<Texture2D>("right"), destrect, Color.White);
                            if (editor.col[x, y] == 5) spriteBatch.Draw(Content.Load<Texture2D>("down"), destrect, Color.White);
                        }
                    }
                    spriteBatch.Draw(Content.Load<Texture2D>("select"), selectpos, Color.White);
                    spriteBatch.Draw(Content.Load<Texture2D>("gridselect2"), new Vector2(gridpos.X + editor.X - 500, gridpos.Y + editor.Y), Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "Arrow Keys - Move Selection, C - Switch to Grid Editing,",
                        new Vector2(50, 600), Color.Blue, Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "1 - Make Solid, 0 - Make Non-Solid, 2 - West Exit,",
                        new Vector2(50, 620), Color.Blue, Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "3 - North Exit, 4 - East Exit, 5 - South Exit,",
                        new Vector2(50, 640), Color.Blue, Color.White);
                    Functions.DrawBorderedText(font, spriteBatch, "6 - Up Exit, 7 - Down Exit",
                        new Vector2(50, 660), Color.Blue, Color.White);
                }
                #region texts
                Functions.DrawBorderedText(font, spriteBatch, "Width - " + editor.width, new Vector2(50, 390), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "Height - " + editor.height, new Vector2(50, 410), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "West Dest - " + editor.dests[0] + " - " + editor.destcoords[0] + ", " + editor.destcoords[1],
                    new Vector2(50, 430), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "North Dest - " + editor.dests[1] + " - " + editor.destcoords[2] + ", " + editor.destcoords[3],
                    new Vector2(50, 450), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "East Dest - " + editor.dests[2] + " - " + editor.destcoords[4] + ", " + editor.destcoords[5],
                    new Vector2(50, 470), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "South Dest - " + editor.dests[3] + " - " + editor.destcoords[6] + ", " + editor.destcoords[7],
                    new Vector2(50, 490), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "Up Dest - " + editor.dests[4] + " - " + editor.destcoords[8] + ", " + editor.destcoords[9],
                    new Vector2(50, 510), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "Down Dest - " + editor.dests[5] + " - " + editor.destcoords[10] + ", " + editor.destcoords[11],
                    new Vector2(50, 530), Color.Blue, Color.White);
                #endregion
            }
            #endregion
            #region destselect
            if (gameState == GameState.destselect)
            {
                for (int n = 0; n < 19; n++) selects[n].Draw(spriteBatch);
                Functions.DrawBorderedText(font, spriteBatch, "West  Dest: ", new Vector2(175, 100), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "North Dest: ", new Vector2(175, 120), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "East  Dest: ", new Vector2(175, 140), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "South Dest: ", new Vector2(175, 160), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "Up    Dest: ", new Vector2(175, 180), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "Down  Dest: ", new Vector2(175, 200), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "West  Coord: ", new Vector2(165, 220), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "North Coord: ", new Vector2(165, 260), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "East  Coord: ", new Vector2(165, 300), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "South Coord: ", new Vector2(165, 340), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "Up    Coord: ", new Vector2(165, 380), Color.Blue, Color.White);
                Functions.DrawBorderedText(font, spriteBatch, "Down  Coord: ", new Vector2(165, 420), Color.Blue, Color.White);
            }
            #endregion
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
