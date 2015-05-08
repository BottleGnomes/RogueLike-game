using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RogueLikeGame
{
    class Unpaused : GameState
    {
        private string tag = "Unpaused";
        public string getTag()
        {
            return tag;
        }

        public void update(GameTime gameTime)
        {
            
        }

        public void draw()
        {
            
        }

        public void entering()
        {
            
        }

        public void leaving()
        {
            
        }
    }
    class Paused : GameState
    {
        private string tag = "Paused";
        public string getTag()
        {
            return tag;
        }

        public void update(GameTime gameTime)
        {

        }

        public void draw()
        {

        }

        public void entering()
        {

        }

        public void leaving()
        {

        }
    }
}
