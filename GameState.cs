using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace RogueLikeGame
{
    class Playing : GameState 
    {
        private string tag = "Playing";
        Unpaused unpaused;
        Paused paused;
        RogueLike ingame;
        GameState state;
        Scene scene;
        SpriteFont chess;
        SpriteFont output;
        SpriteBatch spriteBatch;
        ingameMenu menu;

        public int[] currentCorner = { -15, 11 };
        int tileWidth = 28;
        int tileHeight = 46;
        int[] screenDim = { 50, 20 };

        int[] textScroll = new int[] { -4, -2, -1, 0, 0, 1, 2, 4, 2, 1, 0, 0, -1, -2 };
        int scrollTimer = 0;
        int textScrollIndex = 0;

        Player player;
        List<Enemy> enemies;
        List<Item> items;

        public Playing(SpriteBatch spriteBatch, RogueLike ingame) 
        {
            paused = new Paused(this);
            this.ingame = ingame;
            scene = new Scene(this);
            chess = ingame.Content.Load<SpriteFont>("Chess");
            output = ingame.Content.Load<SpriteFont>("Output18pt");
            this.spriteBatch = spriteBatch;
            menu = new ingameMenu(ingame, output);

            enemies = new List<Enemy> { new Enemy(new int[] {3,21},scene) };
            items =  new List<Item> { new Item(new int[] { 3,18 },scene,"life") };
            player = new Player(new int[] { 24, 8 }, scene);


            //pass in enemies and objects later
            unpaused = new Unpaused(this, player, enemies);

            state = unpaused;
            
        }
        public void changeState(string inState)
        {
            switch (inState)
            {
                case "Paused": 
                    {
                        state.leaving();
                        state = paused;
                        state.entering();
                        break;
                    }
                case "Unpaused":
                    {
                        state.leaving();
                        state = unpaused;
                        state.entering();
                        break;
                    }
            }
        }

        public string getTag()
        {
            return this.tag;
        }

        public void update(GameTime gameTime)
        {
            scrollTimer += gameTime.ElapsedGameTime.Milliseconds;
            state.update(gameTime);
        }

        public void draw()
        {
            //Debug.Print(Convert.ToString(currentCorner[0]) + "," + Convert.ToString(currentCorner[1]));

            this.drawTiles();
            //attack
            if (player.attacking) { spriteBatch.DrawString(chess, "\u2666", new Vector2((player.coords[0] + player.facing[0]) * tileWidth, (player.coords[1] + player.facing[1]) * tileHeight), Color.SandyBrown); }
            
            Vector2 textVector = new Vector2();
            if (textScrollIndex >= textScroll.Length - 1) { textScrollIndex = 0; } else if (scrollTimer > 168) { textScrollIndex++; scrollTimer = 0; }
            //enemies
            foreach (Enemy enemy in enemies) 
            { 
                spriteBatch.DrawString(chess, "\u2646", new Vector2((enemy.coords[0] - currentCorner[0])*tileWidth -5, (enemy.coords[1] - currentCorner[1])*tileHeight), Color.White);
                if (enemy.speaking)
                {
                    textVector = new Vector2(((enemy.coords[0] - currentCorner[0]) * tileWidth) - (tileWidth * 10), ((enemy.coords[1] - currentCorner[1]) * tileHeight) - (tileHeight) + textScroll[textScrollIndex]);
                    spriteBatch.DrawString(output, enemy.getDialog(), new Vector2(textVector.X - 2, textVector.Y - 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog(), new Vector2(textVector.X + 2, textVector.Y + 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog(), textVector, Color.White);

                }
            }
            foreach (Item item in items)
            {
                spriteBatch.DrawString(chess, item.getUniVal(), new Vector2(((item.coords[0] - currentCorner[0]) * tileWidth), ((item.coords[1] - currentCorner[1]) * tileHeight) + textScroll[textScrollIndex]), item.color);
            }

            //player on top during unpaused
            spriteBatch.DrawString(chess, "\u265E", new Vector2(player.coords[0] * tileWidth - 5, player.coords[1] * tileHeight), Color.White);

            if (state == paused) 
            {
                Texture2D dummyTexture = new Texture2D(ingame.GraphicsDevice, 1, 1);
                Texture2D dummyTexture2 = new Texture2D(ingame.GraphicsDevice, 1, 1);
                dummyTexture.SetData(new Color[] { Color.Gray });
                spriteBatch.Draw(dummyTexture, new Rectangle(200, 150, 900, 600), Color.Gray);
                dummyTexture2.SetData(new Color[] { Color.Black });
                spriteBatch.Draw(dummyTexture2, new Rectangle(215, 165, 870, 570), Color.Gray);

                menu.update();
                spriteBatch.DrawString(output, menu.draw(), new Vector2(225, 175), Color.White);
            }
        }

        public void drawTiles()
        {
            for (int i = 0; i < screenDim[0]; i++)
            {
                for (int j = 0; j < screenDim[1]; j++)
                {
                    if (scene.includesTile(new int[] { i + currentCorner[0], j + currentCorner[1] }))
                    {
                        if (scene.getTile(new int[] { i + currentCorner[0], j + currentCorner[1] }).getNum() == 1)
                        {
                            spriteBatch.DrawString(chess, "\u2589", new Vector2((i) * tileWidth, (j) * tileHeight), Color.Gray);
                        }
                        else if (scene.getTile(new int[] { i + currentCorner[0], j + currentCorner[1] }).getNum() == 0)
                        {
                            spriteBatch.DrawString(chess, "\u2591", new Vector2((i) * tileWidth, (j) * tileHeight), Color.Blue);
                        }
                    }
                }
            }
        }
        public int getScreenDimension(int dim) { return screenDim[dim]; }

        public void entering()
        {

        }

        public void leaving()
        {

        }
    }
    interface GameState
    {
        string getTag();
        void update(GameTime gameTime);
        void draw();
        void entering();
        void leaving();
    }
}
