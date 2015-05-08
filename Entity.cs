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
        int[] facing = { 1, 0 };

        public Player(int[] coords, Scene scene)
        {
            this.coords = coords;
            this.scene = scene;
        }
        public void moveLeft() { facing = new int[] { -1, 0 }; this.coords[0]--; if (this.coords[0] < 0 || scene.collides(this.coords)) { this.coords[0]++; } }
        public void moveRight() { facing = new int[] { 1, 0 }; this.coords[0]++; if (this.coords[0] > scene.dimensions[0] || scene.collides(this.coords)) { this.coords[0]--; } }
        public void moveUp() { facing = new int[] { 0, 1 }; this.coords[1]--; if (this.coords[1] < 0 || scene.collides(this.coords)) { this.coords[1]++; } }
        public void moveDown() { facing = new int[] { 0, -1 }; this.coords[1]++; if (this.coords[1] > scene.dimensions[1] || scene.collides(this.coords)) { this.coords[1]--; } }

    }


    class Drawable
    {
        private string tag { get { return tag; } set { tag = value; } }
        private double time = 0;
        public int[] coords;
        public int level;
        public double rotation;
        //use level for animated sprites

        public bool toBeDeleted = false;
        public bool escaped = false;

        Rectangle spriteRectangle { get { return spriteRectangle; } set { spriteRectangle = value; } }
        Rectangle locationRectangle { get { return locationRectangle; } set { locationRectangle = value; } }

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
