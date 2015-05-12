using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueLikeGame
{
    class Event
    {
        Scene scene;
        public int id;

        public Event(Scene scene, int id) 
        {
            this.id = id;
            this.scene = scene;
        }
    }
}
