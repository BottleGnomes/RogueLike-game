using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueLikeGame
{
    interface GameState
    {
        string getTag();
        void update(GameTime gameTime);
        void draw();
        void entering();
        void leaving();
    }
    class Playing : GameState 
    {
        private string tag = "Playing";

        public Playing(SpriteBatch spriteBatch, RogueLike ingame) 
        {
 
        }

        public string getTag()
        {
            return this.tag;
        }

        public void update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void draw()
        {
            throw new NotImplementedException();
        }

        public void entering()
        {
            throw new NotImplementedException();
        }

        public void leaving()
        {
            throw new NotImplementedException();
        }
    }
}
