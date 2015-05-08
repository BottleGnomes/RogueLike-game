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

    class Enemy : Drawable
    {
        Scene scene;
        public List<string> dialog = new List<string>() {"Your magic has no power in these lands, wizard!",
                                  "You're just a horse.",
                                  "I'm lost.",
                                  "I don't believe you!",
                                  "Forget the imitations"};
        int dialogTimer = 0;
        public bool speaking = false;

        public Enemy(int[] coords, Scene scene)
        {
            this.coords = coords;
            this.scene = scene;
        }
        public void speak(GameTime gameTime) { 
            dialogTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (dialogTimer > 2000) { speaking = false; dialogTimer = 0; }
        }
        public string getDialog() 
        {
            return dialog[0];
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
