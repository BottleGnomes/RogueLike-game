using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueLikeGame
{
    

    class UI : Drawable
    {
        int maxHealth = 10;
        Player player;
        // empty = 2661
        //full = 2665
        public UI(Player player) 
        {
            this.player = player;
        }
        public string getHealth() { return String.Concat(Enumerable.Repeat("\u2665", Math.Abs(player.health))); }
        public string getMissingHealth() { return String.Concat(Enumerable.Repeat("\u2665", maxHealth - player.health)); }
        public int getMaxHealth() { return maxHealth; }

    }
}
