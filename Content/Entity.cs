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
        public bool damaged = false;
        public int damageTimer = 0;

        int attackTimer = 0;
        public int health = 8;
        public int select = 0;
        public Color color = Color.White;
        public int sight = 12;
        public List<string> inventory = new List<string>();

        public Player(int[] coords, Scene scene)
        {
            inventory.Add("fist");
            this.coords = coords;
            this.scene = scene;
        }
        public void update(GameTime gameTime)
        {
            if (damaged) { damageTimer += gameTime.ElapsedGameTime.Milliseconds; }
            if (damageTimer > 180) { this.color = Color.White; damageTimer = 0; damaged = false; }
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
                case "sword": { if (!inventory.Contains("sword")) { inventory.Add("sword");} select = inventory.IndexOf("sword");  break; }
                case "long sword": { if (!inventory.Contains("long sword")) { inventory.Add("long sword");} select = inventory.IndexOf("long sword");  break;}
                case "bow": { if (!inventory.Contains("bow")) { inventory.Add("bow"); } select = inventory.IndexOf("bow"); break; }
                case "food": { inventory.Add("food"); break; }
                default: { inventory.Add(item.getTag()); break; }
            }
        }
        public void hit(int damage, int[] direction)
        {
            damaged = true;
            this.color = Color.Red;
            this.health -= damage;
            this.coords[0] += direction[0];
            this.coords[1] += direction[1];
        }
    }

    class Projectile : Drawable
    {
        //\u2736
        Scene scene;
        public int[] direction;
        double[] velocity;
        public int[] pixMod = new int[] { 0, 0 };
        public Color color;
        public string icon;

        public Projectile(int[] coords, int[] direction, string type, Scene scene)
        {
            this.coords = coords;
            this.direction = direction;
            switch (type)
            {
                case "arrow": { this.velocity = new double[] { 3, 3 }; this.icon = "\u2727"; this.color = Color.Crimson; break; }
            }
            this.coords = coords;
            this.scene = scene;
        }
        public void update(GameTime gameTime)
        {
            this.time += gameTime.ElapsedGameTime.Milliseconds;
            this.pixMod = new int[] { (int)(4 * direction[0] * velocity[0]) + pixMod[0], (int)(4 * direction[1] * velocity[1]) + pixMod[1] };

        }
    }
    class Particle : Drawable
    {
        int[] direction;
        double[] velocity;
        int[] location;
        public int[] pixMod = new int[] { 0, 0 };
        public Color color;
        string[] icon;
        int lifespan;

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        public Particle(int[] start, int[] direction, int lifespan, string type, Color color)
        {
            this.lifespan = lifespan;
            this.level = 1;
            this.color = color; 
            switch (type)
            {
                case "stars": { 
                    this.icon = new string[] { "\u273A", "\u2739", "\u2738", "\u2737", "\u2736" }; 
                    this.setTag(icon[0]); 
                    this.direction = new int[] { rand.Next(-1, 2), rand.Next(-1, 2) };
                    break; }
                case "blood": { 
                    this.icon = new string[] { "\u273F", "\u273F", "\u273F", "\u273E", "\u273E" }; 
                    this.setTag(icon[0]); 
                    this.direction = direction;
                    break; }
                default: {
                    this.icon = new string[] { type, type, type, type, type };
                    this.setTag(icon[0]);
                    this.direction = new int[] { rand.Next(-1, 2), rand.Next(-1, 2) };
                    break;}
            }
            location = start;
            velocity = new double[] { rand.NextDouble()+.5, rand.NextDouble()+.5 };
        }
        public void update(GameTime gameTime)
        {
            this.time += gameTime.ElapsedGameTime.Milliseconds;
            if (this.time > lifespan) { this.toBeDeleted = true; }
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
            switch (tag)
            {
                case "food": { uniValue = "\u2668"; this.color = Color.Cornsilk; break; }
                case "sword": { uniValue = "\u2628"; this.color = Color.CadetBlue; break; }
                case "bow": { uniValue = "\u2608"; this.color = Color.BurlyWood; break; }
                case "long sword": { uniValue = "\u2627"; this.color = Color.SlateBlue; break; }
                case "life": { uniValue = "\u2665"; this.color = Color.Red; break; }
                case "key": { uniValue = "\u2669"; this.color = Color.Gold; break; }
                default: { uniValue = "\u2639"; this.color = Color.Purple; break; }
            }
        }
        public string getUniVal() { return uniValue; }

        public static Color getColor(string id)
        {
            switch (id)
            {
                case "fist": { return Color.SandyBrown; }
                case "food": { return Color.Cornsilk;  }
                case "sword": { return Color.CadetBlue;  }
                case "long sword": { return Color.SlateBlue; }
                case "bow": { return Color.BurlyWood; }
                case "life": { return Color.Red;  }
                case "key": { return Color.Gold; }
                default: { return Color.Purple;  }
            }
        }
        public static string getUniVal(string id)
        {
            switch (id)
            {
                case "fist": { return "\u2666"; }
                case "food": { return "\u2668"; }
                case "sword": { return "\u2628"; }
                case "long sword": { return "\u2627"; }
                case "bow": { return "\u2608"; }
                case "life": { return "\u2665"; }
                case "key": { return "\u2669"; }
                default: { return "\u2639"; }
            }
        }
        public static int getDamage(string id)
        {
            switch (id)
            {
                case "fist": { return 5; }
                case "sword": { return 10; }
                case "long sword": { return 8; }
                case "bow": { return 4; }
                default: { return 0; }
            }
        }
        public static string getAttackSymbol(string id)
        {
            switch (id)
            {
                case "sword": { return "\u266E"; }
                case "long sword": { return "\u266F"; }
                case "bow": { return "\u2625"; }
                case "fist": { return "\u2666"; }
                default: { return "?"; }
            }
        }
    }
    class Enemy : Drawable
    {
        Scene scene;
        int dialogTimer = 0;
        int damageTimer = 0;
        int attackTimer = 0;

        public Color color;

        public bool speaking = false;
        public bool damaged = false;
        public bool dying;
        public bool attacking;
        
        public int health;

        public string uniVal;
        public string dialog;
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
        public void update(GameTime gameTime)
        {
            if (this.damaged == true) { damageTimer += gameTime.ElapsedGameTime.Milliseconds; }
            if (damageTimer >= 180) { color = Color.White; damageTimer = 0; this.damaged = false; }
        }
        public string getDialog() 
        {
            return dialog;
        }

        public void attack(GameTime gameTime)
        {
            attackTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (attackTimer > 80) { attacking = false; attackTimer = 0; }
        }
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
            this.damaged = true;
            this.color = Color.Red;
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
