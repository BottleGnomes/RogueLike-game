﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace RogueLikeGame
{
    class Scene
    {
        int[] dimensions;
        Constructor constructor;
        Playing playing;
        int[,] eventArray;

        public Scene(Playing playing) 
        {
            this.playing = playing;
            this.constructor = new Constructor();
            dimensions = constructor.getDimensions();

            string[] ioArray = File.ReadAllText("Content//Event.csv").Split('\n');
            eventArray = new int[(ioArray[0].Length + 1) / 2, ioArray.GetLength(0)];
            for (int i = 0; i < ioArray.Length; i++)
            {
                string[] lineSplit = ioArray[i].Split(',');
                for (int j = 0; j < lineSplit.Length; j++)
                {
                    eventArray[j, i] = Convert.ToInt16(lineSplit[j]);
                }
            }
        }
        public Tile[,] getArray() { return constructor.getTileArray(); }
        public int[] getDimensions() { return dimensions; }
        public bool collides(int[] coordinates) { if (constructor.getTile(new int[] { coordinates[0] + playing.currentCorner[0], coordinates[1] + playing.currentCorner[1] }).getNum() == 1) { return true; } return false; }
        public bool collides(int x, int y) { if (constructor.getTile(new int[] {x + playing.currentCorner[0], y + playing.currentCorner[1] }).getNum() == 1) { return true; } return false; }
        public bool trigger(int[] coordinates) { return eventArray[coordinates[0], coordinates[1]] != 0; }
        public string getEvent(int[] coordinates) 
        {
            int num = eventArray[coordinates[0], coordinates[1]];
            switch (num)
            {
                case 1: { return "exit"; break; }
                default: { return "error"; break; }
            }
        }
        public bool includesTile(int[] coordinates) { return constructor.includesTile(coordinates); }
        public Tile getTile(int[] coordinates) { return constructor.getTile(coordinates); }
        public Tile getTile(int x, int y) { return constructor.getTile(x,y); }
    }


    class Constructor
    {
        Tile[,] tileArray;
        string currentLevel;
        string[] tileTypes = { "floor", "wall","hatch" };

        public Constructor()
        {
            this.build();
            currentLevel = "Collision.csv";
        }

        public void build()
        {
            string[] ioArray = File.ReadAllText(string.Format("Content//Collision.csv",this.currentLevel)).Split('\n');
            tileArray = new Tile[(ioArray[0].Length + 1) / 2, ioArray.GetLength(0)];
            for (int i = 0; i < ioArray.Length; i++)
            {
                string[] lineSplit = ioArray[i].Split(',');
                for (int j = 0; j < lineSplit.Length; j++)
                {
                    int tileNumber = Convert.ToInt16(lineSplit[j]);
                    tileArray[j, i] = new Tile(tileNumber, tileTypes[tileNumber], j, i);
                    tileArray[j, i].setTag(tileTypes[tileNumber]);
                }
            }
        }
        
        public int[] getDimensions()
        {
            int[] temp = new int[2];
            temp[0] = this.tileArray.GetLength(0);
            temp[1] = this.tileArray.GetLength(1);
            return temp;
        }

        public bool includesTile(int x, int y)
        {
            //Debug.Print("x:"+Convert.ToString(x)+"\ny:"+Convert.ToString(y)+"\ndim0:"+Convert.ToString(tileArray.GetLength(0))+"\ndim1:"+Convert.ToString(tileArray.GetLength(1)));
            if (x < 0 || x >= this.tileArray.GetLength(0)) { return false; }
            if (y < 0 || y >= this.tileArray.GetLength(1)) { return false; }
            return true;
        }
        public bool includesTile(int[] coords)
        {
            int x = coords[0];
            int y = coords[1];
            //Debug.Print("x:"+Convert.ToString(x)+"\ny:"+Convert.ToString(y)+"\ndim0:"+Convert.ToString(tileArray.GetLength(0))+"\ndim1:"+Convert.ToString(tileArray.GetLength(1)));
            if (x < 0 || x >= this.tileArray.GetLength(0)) { return false; }
            if (y < 0 || y >= this.tileArray.GetLength(1)) { return false; }
            return true;
        }
        public Tile[,] getTileArray() { return this.tileArray; }
        public Tile getTile(int x, int y) { return tileArray[x, y]; }
        public Tile getTile(int[] coordinates) { return tileArray[coordinates[0], coordinates[1]]; }

    }

    class Tile : Drawable
    {
        string tileType;
        int tileNum;
        public Color color;
        public Tile(int tileNum, string tileType, int x, int y)
        {
            this.tileNum = tileNum;
            this.tileType = tileType;
            this.coords = new int[] { x, y }; 
        }
        public int[] getCoords() { return coords; }
        public int getNum() { return this.tileNum; }
        public string getType() { return this.tileType; }
    }
}
