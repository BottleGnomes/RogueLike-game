using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace RogueLikeGame
{
    class Player : Drawable
    {
        Scene scene;
        public int[] facing = { 0, 1 };
        public bool attacking = false;
        int attackTimer = 0;
        public int health = 8;
        public string attackSymbol = "\u2666";
        public int damage = 5;
        public Color color = Color.White;
        public int sight = 20;

        public Player(int[] coords, Scene scene)
        {
            this.coords = coords;
            this.scene = scene;
        }
        public bool moveLeft() { facing = new int[] { -1, 0 }; if (scene.collides(new int[] {this.coords[0] + facing[0], this.coords[1] + facing[1]})) {return false; } return true; }
        public bool moveRight() { facing = new int[] { 1, 0 }; if (scene.collides(new int[] { this.coords[0] + facing[0], this.coords[1] + facing[1] })) { return false; } return true; }
        public bool moveUp() { facing = new int[] { 0, -1 }; if (scene.collides(new int[] { this.coords[0] + facing[0], this.coords[1] + facing[1] })) { return false; } return true; }
        public bool moveDown() { facing = new int[] { 0, 1 }; if (scene.collides(new int[] { this.coords[0] + facing[0], this.coords[1] + facing[1] })) { return false; } return true; }

        public void attack(GameTime gameTime) 
        {
            attackTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (attackTimer > 80) { attacking = false; attackTimer = 0; }
        }
        public void process(Item item) 
        {
            switch (item.getTag())
            {
                case "life": { this.health++; break; }
                case "sword": { this.attackSymbol = "\u266E"; this.damage = 10; break; }
            }
        }
        public void hit(int damage, int[] direction)
        {
            this.health -= damage;
            this.coords[0] += direction[0];
            this.coords[1] += direction[1];
        }
    }

    class Projectile : Drawable
    {
        Scene scene;

        public Projectile(int[] coords, Scene scene)
        {
            this.coords = coords;
            this.scene = scene;
        }
    }

    class Item : Drawable
    {
        Scene scene;
        string uniValue;
        public Color color;

        public Item(int[] coords, Scene scene, string tag)
        {
            this.coords = coords;
            this.setTag(tag);
            this.setUniVal(this.getTag());
        }
        public void setUniVal(string tag)
        {
            Debug.Print(tag);
            switch (tag)
            {
                case "food": { uniValue = "\u2668"; this.color = Color.Indigo; break; }
                case "sword": { uniValue = "\u2628"; this.color = Color.LightSteelBlue; break; }
                case "bow": { uniValue = "\u269E"; this.color = Color.Gold; break; }
                case "life": { uniValue = "\u2665"; this.color = Color.Red; break; }
                default: { uniValue = "\u2639"; this.color = Color.Purple; break; }
            }
        }
        public string getUniVal() { return uniValue; }
    }
    class Particle : Drawable 
    {
        int[] direction;
        double[] velocity;
        int[] location;
        public int[] pixMod = new int[] {0,0};
        public Color color;
        string[] icon;
        string[] stars = new string[] { "\u273A", "\u2739", "\u2738", "\u2737", "\u2736" };

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        public Particle(int[] start, string type) 
        {
            this.color = Color.PowderBlue;
            this.level = 1;
            switch (type)
            {
                case "stars": { this.setTag(stars[0]); this.icon = stars; break; }
                default: { this.setTag(stars[0]); this.icon = stars; break; }
            }
            location = start;
            direction = new int[] { rand.Next(-1, 2), rand.Next(-1, 2) };
            Debug.Print("dire: "+Convert.ToString(direction[0]) + "," + Convert.ToString(direction[1]));
            velocity = new double[] { rand.NextDouble(),rand.NextDouble()};
        }
        public void update(GameTime gameTime)
        {
            this.time += gameTime.ElapsedGameTime.Milliseconds;
            if (this.time > 250) { this.toBeDeleted = true;}
            else 
            {
                if (this.time > 60) { this.setTag(icon[1]); }
                if (this.time > 130) { this.setTag(icon[2]); }
                if (this.time > 190) { this.setTag(icon[3]); }
                if (this.time > 230) { this.setTag(icon[4]); }
                this.pixMod = new int[] { (int)(4 * direction[0] * velocity[0]) + pixMod[0], (int)(4 * direction[1] * velocity[1]) + pixMod[1] };
            }
        }
        public int[] getLocation() { return location; }
    }
    class Enemy : Drawable
    {
        Scene scene;
        public string dialog;
        int dialogTimer = 0;
        public bool speaking = false;
        public Color color;
        public bool dying;
        public int health;
        public string uniVal;
        public string drop;

        public Enemy(int[] coords, Scene scene, int health, string uniVal,string dialog,string drop)
        {
            this.coords = coords;
            this.scene = scene;
            this.color = Color.White;
            this.dialog = dialog;
            this.health = health;
            this.uniVal = uniVal;
            this.drop = drop;
            dying = false;
        }
        public string getDialog() 
        {
            return dialog;
        }
        public void setColor(Color color) { this.color = color; }
        public void death(GameTime gameTime)
        {
            this.time += gameTime.ElapsedGameTime.Milliseconds;
            if (time > 200) { this.toBeDeleted = true; }
            else 
            {
                if (time > 40) { this.color = Color.Red; }
                if (time > 80) { this.color = Color.White; }
                if (time > 120) { this.color = Color.Red; }
                if (time > 160) { this.color = Color.White; }
            }
        }
        public void hit(int damage, int[] direction)
        {
            this.health -= damage;
            if (health <= 0) { this.dying = true; }
            else
            {
                int[] oldcoords = coords;
                this.coords[0] += direction[0];
                this.coords[1] += direction[1];
            }
        }
    }

    class Drawable
    {
        private string tag;
        public  double time = 0;
        public int[] coords;
        public int level;
        public double rotation;
        //use level for animated sprites

        public bool toBeDeleted = false;
        public bool escaped = false;

        public void setTag(string tag) { this.tag = tag; }
        public string getTag() { return this.tag; }
        public Rectangle spriteRectangle { get { return spriteRectangle; } set { spriteRectangle = value; } }
        public Rectangle locationRectangle { get { return locationRectangle; } set { locationRectangle = value; } }

        public bool collides(Drawable other)
        {
            return this.locationRectangle.Intersects(other.locationRectangle);
        }
        public bool collides(Rectangle other)
        {
            return this.locationRectangle.Intersects(other);
        }
    }


    //for A*
    public class PrioQueue
    {
        int total_size;
        SortedDictionary<int, Queue> storage;

        public PrioQueue()
        {
            this.storage = new SortedDictionary<int, Queue>();
            this.total_size = 0;
        }

        public int Size() { return total_size; }

        public bool IsEmpty()
        {
            return (total_size == 0);
        }

        public object Dequeue()
        {
            if (IsEmpty())
            {
                throw new Exception("Please check that priorityQueue is not empty before dequeing");
            }
            else
                foreach (Queue q in storage.Values)
                {
                    // we use a sorted dictionary
                    if (q.Count > 0)
                    {
                        total_size--;
                        return q.Dequeue();
                    }
                }

            Debug.Assert(false, "not supposed to reach here. problem with changing total_size");

            return null; // not supposed to reach here.
        }

        // same as above, except for peek.

        public object Peek()
        {
            if (IsEmpty())
                throw new Exception("Please check that priorityQueue is not empty before peeking");
            else
                foreach (Queue q in storage.Values)
                {
                    if (q.Count > 0)
                        return q.Peek();
                }

            Debug.Assert(false, "not supposed to reach here. problem with changing total_size");

            return null; // not supposed to reach here.
        }

        public object Dequeue(int prio)
        {
            total_size--;
            return storage[prio].Dequeue();
        }

        public void Enqueue(object item, int prio)
        {
            if (!storage.ContainsKey(prio))
            {
                storage.Add(prio, new Queue());
            }
            storage[prio].Enqueue(item);
            total_size++;

        }
    }
}
