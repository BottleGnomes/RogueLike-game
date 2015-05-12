using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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
    class TextBox : Drawable
    {
        int textTimer = 0;
        int waitTimer = 0;
        public Queue<TextLine> waitOutput = new Queue<TextLine>();
        public Queue<TextLine> output = new Queue<TextLine>();
        public Queue<TextLine> text = new Queue<TextLine>();
        Scene scene;

        public TextBox(Scene scene) 
        {
            this.scene = scene;
        }
        public void addLine(string line, int timer, string color) { output.Enqueue(new TextLine(line,timer,color)); }
        public void addWaitLine(string line, int timer, string color) { waitOutput.Enqueue(new TextLine(line,timer,color)); }
        public void update(GameTime gameTime)
        {
            textTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (output.Count > 0 && textTimer > output.Peek().time) { text.Enqueue(output.Dequeue()); textTimer = 0; }
            else 
            {
                waitTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (waitOutput.Count > 0 && waitTimer >= waitOutput.Peek().time) { text.Enqueue(waitOutput.Dequeue()); waitTimer = 0; }
            }
        }
        public Queue<TextLine> getText()
        {
            if (text.Count > 8) { text.Dequeue(); }
            return text;
        }
    }
    class TextLine
    {
        public string text;
        public int time;
        public string color;
        public TextLine(string text, int time, string color)
        {
            this.text = text;
            this.time = time;
            this.color = color;
        }
    }
}
