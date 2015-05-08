using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueLikeGame
{
    class Scene
    {
        public int[] dimensions = { 20, 20 };

        public Scene() { }

        public bool collides(int[] coordinates) { return false; /* check map to find collisions */ }
    }
}
