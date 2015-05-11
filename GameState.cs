﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace RogueLikeGame
{
    class Playing : GameState
    {
        private string tag = "Playing";
        Unpaused unpaused;
        Paused paused;
        RogueLike ingame;
        UI ui;
        GameState state;
        Scene scene;
        SpriteFont symbols;
        SpriteFont output;
        SpriteBatch spriteBatch;
        ingameMenu menu;

        public int[] currentCorner = { -15, 11 };
        int tileWidth = 26;
        int tileHeight = 44;
        int[] screenDim = { 50, 20 };
        int[,] drawArray;
        int[,] seenArray;

        int[] textScroll = new int[] { -4, -2, -1, 0, 0, 1, 2, 4, 2, 1, 0, 0, -1, -2 };
        int scrollTimer = 0;
        int textScrollIndex = 0;

        int frameCount = 0;
        int frames = 0;
        int frameTimer = 0;

        Player player;
        List<Enemy> enemies;
        List<Item> items;
        List<Particle> particles;
        List<Projectile> projectiles;

        private bool collide;

        public Playing(SpriteBatch spriteBatch, RogueLike ingame)
        {
            paused = new Paused(this);
            this.ingame = ingame;
            scene = new Scene(this);
            symbols = ingame.Content.Load<SpriteFont>("symbols");
            output = ingame.Content.Load<SpriteFont>("Output18pt");
            this.spriteBatch = spriteBatch;
            menu = new ingameMenu(ingame, output);
            seenArray = new int[scene.getDimensions()[0], scene.getDimensions()[1]];

            enemies = new List<Enemy> { new Enemy(new int[] { 3, 21 }, scene, 20, "\u2646", "Your magic has no power in these lands, wizard!", "sword") };
            enemies.Add(new Enemy(new int[] { 3, 5 }, scene, 20, "\u2645", "I am DEATH, nigh on apocalypse! Die, cur!", "bow"));

            items = new List<Item> { new Item(new int[] { 3, 18 }, scene, "life") };
            items.Add(new Item(new int[] { 8, 18 }, scene, "bow"));
            items.Add(new Item(new int[] { 5, 17 }, scene, "food"));

            particles = new List<Particle>();
            projectiles = new List<Projectile>();
            player = new Player(new int[] { 24, 8 }, scene);
            ui = new UI(player);


            //pass in enemies and objects later
            unpaused = new Unpaused(this, scene, player, enemies, items, particles, projectiles);

            state = unpaused;

        }
        public void changeState(string inState)
        {
            switch (inState)
            {
                case "Paused":
                    {
                        state.leaving();
                        state = paused;
                        state.entering();
                        break;
                    }
                case "Unpaused":
                    {
                        state.leaving();
                        state = unpaused;
                        state.entering();
                        break;
                    }
            }
        }

        public string getTag()
        {
            return this.tag;
        }

        public void update(GameTime gameTime)
        {
            scrollTimer += gameTime.ElapsedGameTime.Milliseconds;
            frameTimer += gameTime.ElapsedGameTime.Milliseconds;

            particles.RemoveAll(a => a.toBeDeleted);
            projectiles.RemoveAll(a => a.toBeDeleted);
            enemies.RemoveAll(a => a.toBeDeleted);
            state.update(gameTime);

            frameCount++;
            if (frameTimer >= 1000) { frames = frameCount; frameCount = 0; frameTimer = 0; }
        }

        public void draw()
        {
            //Debug.Print(Convert.ToString(currentCorner[0]) + "," + Convert.ToString(currentCorner[1]));
            this.drawTiles();

            //projectiles
            foreach (Projectile projectile in projectiles)
            {
                spriteBatch.DrawString(symbols, projectile.icon, new Vector2((projectile.coords[0] - currentCorner[0])*tileWidth + projectile.pixMod[0], (projectile.coords[1] - currentCorner[1])*tileHeight + projectile.pixMod[1]), projectile.color);
            }

            //attack
            if (player.attacking) 
            {
                spriteBatch.DrawString(symbols, Item.getAttackSymbol(player.inventory[player.select]), new Vector2((player.coords[0] + player.facing[0]) * tileWidth, (player.coords[1] + player.facing[1]) * tileHeight), Item.getColor(player.inventory[player.select])); 
            }

            Vector2 textVector = new Vector2();
            if (textScrollIndex >= textScroll.Length - 1) { textScrollIndex = 0; } else if (scrollTimer > 168) { textScrollIndex++; scrollTimer = 0; }
            //enemies
            foreach (Enemy enemy in enemies)
            {
                if(drawArray[enemy.coords[0],enemy.coords[1]] == 1){
                spriteBatch.DrawString(symbols, enemy.uniVal, new Vector2((enemy.coords[0] - currentCorner[0]) * tileWidth - 5, (enemy.coords[1] - currentCorner[1]) * tileHeight), enemy.color);
                }
                if (enemy.speaking)
                {
                    textVector = new Vector2(((enemy.coords[0] - currentCorner[0]) * tileWidth) - (tileWidth * 10), ((enemy.coords[1] - currentCorner[1]) * tileHeight) - (tileHeight) + textScroll[textScrollIndex]);
                    spriteBatch.DrawString(output, enemy.getDialog(), new Vector2(textVector.X - 2, textVector.Y - 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog(), new Vector2(textVector.X + 2, textVector.Y + 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog(), textVector, Color.White);

                }
            } 

            //particles
            foreach (Particle particle in particles)
                {
                //Debug.Print(Convert.ToString(particle.getLocation()[0] - currentCorner[0]) + "," + Convert.ToString(particle.getLocation()[1] - currentCorner[1]));
                spriteBatch.DrawString(symbols, particle.getTag(), new Vector2((particle.getLocation()[0] - currentCorner[0]) * tileWidth + particle.pixMod[0], (particle.getLocation()[1] - currentCorner[1]) * tileHeight + particle.pixMod[1]), particle.color);
                }
            foreach (Item item in items)
            {
                if (drawArray[item.coords[0], item.coords[1]] == 1)
                {
                    spriteBatch.DrawString(symbols, item.getUniVal(), new Vector2(((item.coords[0] - currentCorner[0]) * tileWidth), ((item.coords[1] - currentCorner[1]) * tileHeight) + textScroll[textScrollIndex]), item.color);
                }
            }

            //UI
            for (int i = 0; i < player.health; i++) { spriteBatch.DrawString(symbols, "\u2665", new Vector2(10 + (i * 30), 10), Color.Red); }
            for (int i = 0; i < ui.getMaxHealth()- player.health; i++) { spriteBatch.DrawString(symbols, "\u2665", new Vector2(10+(30*player.health)+(i*30),10), Color.Gray); }
            foreach (string item in player.inventory)
            {
                    if (item == player.inventory[player.select])
                {
                    spriteBatch.DrawString(symbols, Item.getUniVal(item), new Vector2(10 + (40 * player.inventory.IndexOf(item)) - 2, 55), Color.Yellow);
                    spriteBatch.DrawString(symbols, Item.getUniVal(item), new Vector2(10 + (40 * player.inventory.IndexOf(item)) + 2, 55), Color.Yellow);
                    spriteBatch.DrawString(symbols, Item.getUniVal(item), new Vector2(10 + (40 * player.inventory.IndexOf(item)), 57), Color.Yellow);
                    spriteBatch.DrawString(symbols, Item.getUniVal(item), new Vector2(10 + (40 * player.inventory.IndexOf(item)), 53), Color.Yellow);
                }

                //player
                spriteBatch.DrawString(symbols, Item.getUniVal(item), new Vector2(10+( 40 * player.inventory.IndexOf(item) ), 55), Item.getColor(item));
            }

            spriteBatch.DrawString(symbols, "\u265E", new Vector2(player.coords[0] * tileWidth - 5, player.coords[1] * tileHeight+4), player.color);

            if (state == paused)
            {
                Texture2D dummyTexture = new Texture2D(ingame.GraphicsDevice, 1, 1);
                Texture2D dummyTexture2 = new Texture2D(ingame.GraphicsDevice, 1, 1);
                dummyTexture.SetData(new Color[] { Color.Gray });
                spriteBatch.Draw(dummyTexture, new Rectangle(200, 50, 900, 600), Color.Gray);
                dummyTexture2.SetData(new Color[] { Color.Black });
                spriteBatch.Draw(dummyTexture2, new Rectangle(215, 65, 870, 570), Color.Gray);

                menu.update();
                spriteBatch.DrawString(output, menu.draw(), new Vector2(225, 75), Color.White);
            }
            spriteBatch.DrawString(output, "FPS: " + Convert.ToString(frames), new Vector2(10, 800), Color.White);
            spriteBatch.DrawString(output, "Facing: " + Convert.ToString(player.facing[0]+","+player.facing[1]), new Vector2(10, 830), Color.White);

            Array.Clear(drawArray,0,drawArray.Length);
        }
        //gainsboro, gray, darkslategray, black
        //blue, midnightblue, darkslatgray, black
        
        public void drawTiles()
        {
            string[] tiles = { "\u2591", "\u2588", "\u2593" };
            Color[,] colors = { { Color.DimGray, Color.Gray, Color.DarkSlateGray, Color.Black }, { Color.DarkCyan, Color.MidnightBlue, Color.DarkSlateGray, Color.Black }, { Color.BurlyWood, Color.Gray, Color.DarkSlateGray, Color.Black } };
            int[] startingTile = new int[] { player.coords[0] + currentCorner[0], player.coords[1] + currentCorner[1] };
            List<List<int[]>> tileQuerry = new List<List<int[]>>();
            drawArray = new int[scene.getArray().GetLength(0), scene.getArray().GetLength(1)];

            int x = player.sight;
            int y = 0;
            int radiusError = 1 - x;
            int r = player.sight;
            while (x >= y)
            {
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + x, startingTile[1] + y }));
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + x, startingTile[1] + y+1 }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + y, startingTile[1] + x }));
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + y+1, startingTile[1] + x }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - x, startingTile[1] + y }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - y, startingTile[1] + x }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - x, startingTile[1] - y }));
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - x-1, startingTile[1] - y }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - y, startingTile[1] - x }));
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - y-1, startingTile[1] - x }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + x, startingTile[1] - y }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + y, startingTile[1] - x }));
                y++;
                if (radiusError < 0) { radiusError += 2 * y + 1; }
                else { x--; radiusError += 2 * (y - x) + 1; }
            }
            //foreach (int[] a in tileQuerry) { Debug.Print(Convert.ToString(a[0]) + "," + Convert.ToString(a[1])); }
            spriteBatch.DrawString(symbols, tiles[scene.getTile(new int[] { player.coords[0] + currentCorner[0], player.coords[1] + currentCorner[1] }).getNum()], new Vector2(player.coords[0] * tileWidth, player.coords[1] * tileHeight), colors[scene.getTile(new int[] { player.coords[0] + currentCorner[0], player.coords[1] + currentCorner[1] }).getNum(),0]);

            foreach (List<int[]> tileList in tileQuerry)
            {
                collide = false;
                List<int[]> list = tileList.OrderByDescending(f => Math.Abs(f[0] - startingTile[0]) + Math.Abs(f[1] - startingTile[1])).ToList();
                list.Reverse();
                //foreach (int[] whatever in listForward) { Debug.Print(Convert.ToString(whatever[0]) + "," + Convert.ToString(whatever[1])); }

                foreach (int[] lineTile in list)
                {
                    //if (Math.Abs(distance[0]) + Math.Abs(distance[1]) <= player.sight) 
                    //{
                    if (scene.includesTile(lineTile))
                    {
                        if (!scene.collides(new int[] { lineTile[0] - currentCorner[0], lineTile[1] - currentCorner[1] }) && collide) { break; }
                        if (drawArray[lineTile[0], lineTile[1]] != 1) { spriteBatch.DrawString(symbols, tiles[scene.getTile(lineTile).getNum()], new Vector2((lineTile[0] - currentCorner[0]) * tileWidth, (lineTile[1] - currentCorner[1]) * tileHeight), colors[scene.getTile(lineTile).getNum(), scene.getTile(lineTile).getNum() / 4]); }
                        drawArray[lineTile[0], lineTile[1]] = 1;
                        if (scene.collides(new int[] { lineTile[0] - currentCorner[0], lineTile[1] - currentCorner[1] })) { collide = true; }
                    }//}

                }

            }

        }
        public List<int[]> getTilesInbetween(int[] A, int[] B)
        {
            List<int[]> tileArray = new List<int[]>();
            if (((A[0] + B[0]) / 2 == A[0] && (A[1] + B[1]) / 2 == A[1]) || ((A[0] + B[0]) / 2 == B[0] && (A[1] + B[1]) / 2 == B[1]))
            { return tileArray; }
            tileArray.Add(new int[] { (A[0] + B[0]) / 2, (A[1] + B[1]) / 2 });
            tileArray.AddRange(getTilesInbetween(A, new int[] { (A[0] + B[0]) / 2, (A[1] + B[1]) / 2 }));
            tileArray.AddRange(getTilesInbetween(new int[] { (A[0] + B[0]) / 2, (A[1] + B[1]) / 2 }, B));
            return tileArray;

        }
        public List<Tile> getTilesInbetween(Tile A, Tile B)
        {
            List<Tile> tileArray = new List<Tile>();
            if (((A.coords[0] + B.coords[0]) / 2 == A.coords[0] && (A.coords[1] + B.coords[1]) / 2 == A.coords[1]) || ((A.coords[0] + B.coords[0]) / 2 == B.coords[0] && (A.coords[1] + B.coords[1]) / 2 == B.coords[1]))
            { return tileArray; }
            tileArray.Add(scene.getTile((A.coords[0] + B.coords[0]) / 2, (A.coords[1] + B.coords[1]) / 2));
            tileArray.AddRange(getTilesInbetween(A, scene.getTile((A.coords[0] + B.coords[0]) / 2, (A.coords[1] + B.coords[1]) / 2)));
            tileArray.AddRange(getTilesInbetween(scene.getTile((A.coords[0] + B.coords[0]) / 2, (A.coords[1] + B.coords[1]) / 2), B));
            return tileArray;
        }
        public int getScreenDimension(int dim) { return screenDim[dim]; }
        public void addParticle(Particle particle) { this.particles.Add(particle); }
        public void addProjectile(Projectile projectile) { this.projectiles.Add(projectile); }

        public void entering()
        {

        }

        public void leaving()
        {

        }
    }
    interface GameState
    {
        string getTag();
        void update(GameTime gameTime);
        void draw();
        void entering();
        void leaving();
    }
}
