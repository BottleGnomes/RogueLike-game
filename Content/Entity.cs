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

        public bool onFire = false;
        public bool attacking = false;
        public bool damaged = false;
        public int damageTimer = 0;

        int attackTimer = 0;
        public int health = 4;
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

        public bool moveLeft() { facing = new int[] { -1, 0 }; if (scene.collides(new int[] { this.coords[0] + facing[0], this.coords[1] + facing[1] })) { return false; } return true; }
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
                case "sword": { if (!inventory.Contains("sword")) { inventory.Add("sword"); } select = inventory.IndexOf("sword"); break; }
                case "long sword": { if (!inventory.Contains("long sword")) { inventory.Add("long sword"); } select = inventory.IndexOf("long sword"); break; }
                case "bow": { if (!inventory.Contains("bow")) { inventory.Add("bow"); } select = inventory.IndexOf("bow"); break; }
                case "copper tome": { if (!inventory.Contains("copper tome")) { inventory.Add("copper tome"); } select = inventory.IndexOf("copper tome"); break; }
                case "malachite tome": { if (!inventory.Contains("malachite tome")) { inventory.Add("malachite tome"); } select = inventory.IndexOf("malachite tome"); break; }
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

    class Box : Drawable
    {
        Scene scene;
        public string uniVal;
        public string drop;
        public int eventId;

        public Box(int[] coords, Scene scene, string uniVal, string drop, int eventId)
        {
            this.scene = scene;
            this.coords = coords;
            this.uniVal = "\u25A6";
            this.drop = drop;
            this.eventId = eventId;
        }
    }
    class StaticObject : Drawable
    {
        public string icon;
        public string type;
        public bool collision;
        public string particles;
        public int frequency;
        public int particleTimer = 0;

        public StaticObject(int[] coords, string icon, string type, string collision, string particles, int frequency) 
        {
            this.coords = coords;
            this.icon = "\u25B2";
            this.type = type;
            this.collision = collision == "true";
            this.particles = particles;
            this.frequency = frequency;
        }
        public void update(GameTime gameTime) 
        {
            particleTimer += gameTime.ElapsedGameTime.Milliseconds;
        }
    }
    class Projectile : Drawable
    {
        //\u2736
        Scene scene;
        int decayTimer = 0;
        public int[] direction;
        public int[] velocity;
        int[] decay = new int[] { 0, 0 };
        public int[] pixMod = new int[] { 0, 0 };
        public Color color;
        public string icon;
        public string type;

        public Projectile(int[] coords, int[] direction, string type, Scene scene)
        {
            this.coords = coords;
            this.direction = direction;
            this.type = type;
            switch (type)
            {
                case "arrow": { this.velocity = new int[] { 3 * direction[0], 3 * direction[1] }; this.icon = "\u2727"; this.color = Color.Crimson; break; }
                case "copper": { this.velocity = new int[] { 3 * direction[0], 3 * direction[1] }; this.decay = new int[] { direction[0] * (-1), direction[1] * (-1) }; this.icon = "\u2600"; this.color = Color.Orange; break; }
                case "malachite": { this.velocity = new int[] { 3 * direction[0], 3 * direction[1] }; this.icon = "\u2600"; this.color = Color.MediumAquamarine; break; }
            }
            this.coords = coords;
            this.scene = scene;
        }
        public void update(GameTime gameTime)
        {
            this.time += gameTime.ElapsedGameTime.Milliseconds;
            this.decayTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (decayTimer > 150) { this.velocity = new int[] { velocity[0] + decay[0], velocity[1] + decay[1] }; decayTimer = 0; }
            this.pixMod = new int[] { (int)(4 * velocity[0]) + pixMod[0], (int)(4 * velocity[1]) + pixMod[1] };

        }
        public static int getDamage(string type)
        {
            switch (type)
            {
                case "arrow": { return 4; }
                case "copper": { return 18; }
                case "malachite": { return 18; }
                default: { return 0; }
            }
        }
    }
    class Particle : Drawable
    {
        int[] direction;
        double[] velocity;
        int[] decay = new int[] { 0, 0 };
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
                case "stars":
                    {
                        this.icon = new string[] { "\u273A", "\u2739", "\u2738", "\u2737", "\u2736" };
                        this.setTag(icon[0]);
                        this.direction = new int[] { rand.Next(-1, 2), rand.Next(-1, 2) };
                        break;
                    }
                case "blood":
                    {
                        this.icon = new string[] { "\u273F", "\u273F", "\u273F", "\u273E", "\u273E" };
                        this.setTag(icon[0]);
                        this.direction = direction;
                        break;
                    }
                case "magic":
                    {
                        this.icon = new string[] { "\u25CB", "\u25E6", "\u25CC", "\u25E6", "\u25E6" };
                        this.setTag(icon[0]);
                        this.direction = new int[] { direction[0] + rand.Next(-1, 2), direction[1] + rand.Next(-1, 2) };
                        break;
                    }
                case "fire":
                    {
                        this.icon = new string[] { "\u25BC", "\u2666", "\u25BE", "\u25BD", "\u25CF", "\u25BF" };
                        this.setTag(icon[0]);
                        this.direction = new int[] { 0, -1 };
                        Color[] colors = new Color[] { Color.Red, Color.Orange, Color.DarkOrange, Color.Yellow };
                        this.color = colors[rand.Next(colors.Length)];
                        this.pixMod[0] = rand.Next(-10, 11);
                        break;
                        //FIRE: 25BD  25CF 25BF
                        //      25BC  2666 25BE
                    }
                default:
                    {
                        this.icon = new string[] { type, type, type, type, type };
                        this.setTag(icon[0]);
                        this.direction = new int[] { rand.Next(-1, 2), rand.Next(-1, 2) };
                        break;
                    }
            }
            this.coords = start;
            velocity = new double[] { rand.NextDouble() + .5, rand.NextDouble() + .5 };
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
        public int[] getLocation() { return coords; }
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
            this.uniValue = Item.getUniVal(tag);
            this.color = Item.getColor(tag);
        }
        public string getUniVal() { return uniValue; }

        public static Color getColor(string id)
        {
            switch (id)
            {
                case "fist": { return Color.SandyBrown; }
                case "food": { return Color.Cornsilk; }
                case "sword": { return Color.CadetBlue; }
                case "long sword": { return Color.SlateBlue; }
                case "bow": { return Color.BurlyWood; }
                case "life": { return Color.Red; }
                case "key": { return Color.Gold; }
                case "copper tome": { return Color.Tomato; }
                case "malachite tome": { return Color.SeaGreen; }
                default: { return Color.Purple; }
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
                case "copper tome": { return "\u25C8"; }
                case "malachite tome": { return "\u25CD"; }
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
                case "copper tome": { return "\u25CC"; }
                case "malachite tome": { return "\u25CD"; }
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
        int moveTimer = 0;

        public Color color;

        public bool speaking = false;
        public bool damaged = false;
        public bool dying;
        public bool attacking;
        public bool aggressive;
        public bool moving = false;

        public int health;
        public int eventId;
        public int[] direction = new int[] { 0, 1 };
        public int[] destination;
        public Stack<int[]> path;

        public string uniVal;
        public Queue<TextLine> dialog = new Queue<TextLine>();
        Playing playing;
        public string drop;

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        public Enemy(int[] coords, Scene scene, int health, string drop, int eventId, string tag, Playing playing, bool aggressive)
        {
            //Debug.Print(uniVal);
            this.setTag(tag);
            this.coords = coords;
            this.scene = scene;
            this.playing = playing;
            this.color = Color.White;
            this.health = health;
            this.uniVal = "\u2646";
            this.drop = drop;
            this.eventId = eventId;
            this.aggressive = aggressive;
            dying = false;
        }
        public void speak(TextLine dialog) { this.dialog.Enqueue(dialog); this.speaking = true; }
        public void speak(Queue<TextLine> dialog) { this.dialog = dialog; this.speaking = true; }
        public void update(GameTime gameTime)
        {
            //if (!moving) { moving = true; path = playing.AstarSearch(coords, new int[] { rand.Next(2,29),rand.Next(2,20) }); }
            if (moving)
            {
                if (path.Count == 0) { moving = false; }
                else
                {
                    moveTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (moveTimer > 240 && !scene.collides(new int[]{path.Peek()[0] - playing.currentCorner[0],path.Peek()[1] - playing.currentCorner[1]})) { moveTimer = 0; coords = path.Pop(); }
                }
            }
            if (this.speaking) { dialogTimer += gameTime.ElapsedGameTime.Milliseconds; }

            if (dialog.Count > 0 && dialogTimer > dialog.Peek().time) { dialog.Dequeue(); dialogTimer = 0; }
            if (dialog.Count == 0) { speaking = false; }

            if (this.damaged) { damageTimer += gameTime.ElapsedGameTime.Milliseconds; }
            if (damageTimer >= 180) { color = Color.White; damageTimer = 0; this.damaged = false; }

            if (attacking) { attackTimer += gameTime.ElapsedGameTime.Milliseconds; }
            if (attackTimer > 1600) { attacking = false; attackTimer = 0; }
        }
        public TextLine getDialog()
        {
            return dialog.Peek();
        }

        public void attack(GameTime gameTime, int[] playerLocation)
        {
            if (!attacking)
            {
                attacking = true;
                if (coords[1] - playerLocation[1] == 0) { direction = new int[] {(playerLocation[0] - coords[0]) / (Math.Abs(playerLocation[0] - coords[0])),0 }; }
                if (coords[0] - playerLocation[0] == 0) { direction = new int[] { 0, (playerLocation[1] - coords[1]) / (Math.Abs(playerLocation[1] - coords[1])) }; }
            }

        }
        public void death(GameTime gameTime)
        {
            this.time += gameTime.ElapsedGameTime.Milliseconds;
            if (time > 200) { this.toBeDeleted = true; if (this.eventId > 0 && scene.events.Contains(scene.events.Find(a => a.id == this.eventId))) { scene.events.Find(a => a.id == this.eventId).trigger(); } }
            else
            {
                if (time > 40) { this.color = Color.Red; }
                if (time > 80) { this.color = Color.White; }
                if (time > 120) { this.color = Color.Red; }
                if (time > 160) { this.color = Color.White; }
            }
        }
        public void hit(int damage, int[] facing)
        {
            this.damaged = true;
            this.color = Color.Red;
            this.health -= damage;
            if (health <= 0) { this.dying = true; }
            else if(!scene.collides(new int[] {coords[0] - playing.currentCorner[0] + facing[0], coords[1] - playing.currentCorner[1] + facing[1]}))
            {
                this.coords[0] += facing[0];
                this.coords[1] += facing[1];
            }
        }
        public void setDestination(int[] destination)
        {
            this.destination = destination;
            this.path = playing.AstarSearch(coords, destination);
            moving = true;
        }
    }

    class Drawable
    {
        private string tag;
        public double time = 0;
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
