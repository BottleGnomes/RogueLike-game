using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        int tileWidth = 32;

        Player player;
        List<Drawable> units = new List<Drawable>();

        public Playing(SpriteBatch spriteBatch, RogueLike ingame) 
        {
            paused = new Paused(this);
            this.ingame = ingame;
            scene = new Scene();
            chess = ingame.Content.Load<SpriteFont>("Chess2");
            output = ingame.Content.Load<SpriteFont>("Output18pt");
            this.spriteBatch = spriteBatch;
            menu  = new ingameMenu(ingame, output);

            player = new Player(new int[] { 10, 10 }, scene);
            units.Add(player);

            //pass in enemies and objects later
            unpaused = new Unpaused(this, player);

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
            state.update(gameTime);
        }

        public void draw()
        {
            spriteBatch.DrawString(chess, "\u265E", new Vector2(player.coords[0] * tileWidth, player.coords[1]* tileWidth), Color.White);

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
