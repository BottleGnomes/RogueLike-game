using System;
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
        TextBox textBox;
        Dictionary<string, Color> colorDict = new Dictionary<string, Color>();

        public int[] currentCorner = { -13, 12 };
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
        int swing = 0;
        int swingTimer = 0;

        public Player player;
        public List<Enemy> enemies = new List<Enemy>();
        public List<Item> items = new List<Item>();
        public List<Particle> particles = new List<Particle>();
        public List<Projectile> projectiles = new List<Projectile>();
        public List<Box> boxes = new List<Box>();
        public List<StaticObject> staticObjects = new List<StaticObject>();

        private bool collide;

        string[] tiles = { "\u2591", "\u2588", "\u2593", "\u2592" };

        public Playing(SpriteBatch spriteBatch, RogueLike ingame)
        {
            this.ingame = ingame;
            symbols = ingame.Content.Load<SpriteFont>("symbols");
            output = ingame.Content.Load<SpriteFont>("Output18pt");
            this.spriteBatch = spriteBatch;
            menu = new ingameMenu(ingame, output);
            this.textBox = new TextBox(scene);
            scene = new Scene(this, textBox);
            seenArray = new int[scene.getDimensions()[0], scene.getDimensions()[1]];

            colorDict["White"] = Color.White;
            colorDict["Red"] = Color.Red;
            colorDict["Blue"] = Color.Blue;
            colorDict["Yellow"] = Color.Yellow;
            colorDict["Brown"] = Color.Brown;
            colorDict["Gold"] = Color.Gold;
            colorDict["Salmon"] = Color.Salmon;

            player = new Player(new int[] { 24, 6 }, scene);
            ui = new UI(player);

            unpaused = new Unpaused(this, scene, player, enemies, items, particles, projectiles, textBox, boxes, staticObjects);
            paused = new Paused(this, menu);

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
            swingTimer += gameTime.ElapsedGameTime.Milliseconds;

            particles.RemoveAll(a => a.toBeDeleted);
            projectiles.RemoveAll(a => a.toBeDeleted);
            enemies.RemoveAll(a => a.toBeDeleted);
            boxes.RemoveAll(a => a.toBeDeleted);
            state.update(gameTime);

            frameCount++;
            if (frameTimer >= 1000) { frames = frameCount; frameCount = 0; frameTimer = 0; }
        }

        public void draw()
        {
            //Debug.Print(Convert.ToString(currentCorner[0]) + "," + Convert.ToString(currentCorner[1]));
            this.drawTiles();


            //attack
            if (player.attacking)
            {
                switch (player.inventory[player.select])
                {
                    case "long sword":
                        {
                            spriteBatch.DrawString(symbols, Item.getAttackSymbol(player.inventory[player.select]), new Vector2((player.coords[0] + player.facing[0] + player.facing[1]) * tileWidth, (player.coords[1] + player.facing[1] + player.facing[0]) * tileHeight), Item.getColor(player.inventory[player.select])); 
                            if (swingTimer >= 60) { swing++; spriteBatch.DrawString(symbols, Item.getAttackSymbol(player.inventory[player.select]), new Vector2((player.coords[0] + player.facing[0]) * tileWidth, (player.coords[1] + player.facing[1]) * tileHeight), Item.getColor(player.inventory[player.select])); }
                            if (swingTimer >= 90) { spriteBatch.DrawString(symbols, Item.getAttackSymbol(player.inventory[player.select]), new Vector2((player.coords[0] + player.facing[0] - player.facing[1]) * tileWidth, (player.coords[1] + player.facing[1] - player.facing[0]) * tileHeight), Item.getColor(player.inventory[player.select])); }
                            break;
                        }
                    default: { spriteBatch.DrawString(symbols, Item.getAttackSymbol(player.inventory[player.select]), new Vector2((player.coords[0] + player.facing[0]) * tileWidth, (player.coords[1] + player.facing[1]) * tileHeight), Item.getColor(player.inventory[player.select])); break; }
                }
            }
            else { swingTimer = 0; }

            Vector2 textVector = new Vector2();
            if (textScrollIndex >= textScroll.Length - 1) { textScrollIndex = 0; } else if (scrollTimer > 168) { textScrollIndex++; scrollTimer = 0; }
            //enemies
            foreach (Enemy enemy in enemies)
            {
                if (enemy.attacking) { spriteBatch.DrawString(symbols, Item.getAttackSymbol(enemy.getTag()), new Vector2((enemy.coords[0] - currentCorner[0] + enemy.direction[0])*tileWidth, (enemy.coords[1] - currentCorner[1] + enemy.direction[1])*tileHeight), Item.getColor(enemy.getTag())); }
                if (drawArray[enemy.coords[0], enemy.coords[1]] == 1)
                {
                    spriteBatch.DrawString(symbols, enemy.uniVal, new Vector2((enemy.coords[0] - currentCorner[0]) * tileWidth - 5, (enemy.coords[1] - currentCorner[1]) * tileHeight), enemy.color);
                }
                if (enemy.speaking)
                {
                    textVector = new Vector2(((enemy.coords[0] - currentCorner[0]) * tileWidth) - (enemy.getDialog().text.Length / 2 * 13), ((enemy.coords[1] - currentCorner[1]) * tileHeight) - (tileHeight) + textScroll[textScrollIndex]);
                    spriteBatch.DrawString(output, enemy.getDialog().text, new Vector2(textVector.X - 2, textVector.Y - 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog().text, new Vector2(textVector.X + 2, textVector.Y + 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog().text, textVector, Color.White);

                }
            }

            //static objects
            foreach (StaticObject staticObject in staticObjects)
            {
                if (drawArray[staticObject.coords[0], staticObject.coords[1]] == 1)
                {
                    spriteBatch.DrawString(symbols, staticObject.icon, new Vector2((staticObject.coords[0] - currentCorner[0]) * tileWidth, (staticObject.coords[1] - currentCorner[1]) * tileHeight), Color.Brown);
                }
            }
            //particles
            foreach (Particle particle in particles)
            {
                if (drawArray[particle.coords[0], particle.coords[1]] == 1)
                {
                    //Debug.Print(Convert.ToString(particle.getLocation()[0] - currentCorner[0]) + "," + Convert.ToString(particle.getLocation()[1] - currentCorner[1]));
                    spriteBatch.DrawString(symbols, particle.getTag(), new Vector2((particle.getLocation()[0] - currentCorner[0]) * tileWidth + particle.pixMod[0], (particle.getLocation()[1] - currentCorner[1]) * tileHeight + particle.pixMod[1]), particle.color);
                }
            }
            //projectiles
            foreach (Projectile projectile in projectiles)
            {
                if (drawArray[projectile.coords[0], projectile.coords[1]] == 1)
                {
                    spriteBatch.DrawString(symbols, projectile.icon, new Vector2((projectile.coords[0] - currentCorner[0]) * tileWidth + projectile.pixMod[0], (projectile.coords[1] - currentCorner[1]) * tileHeight + projectile.pixMod[1]), projectile.color);
                }
            }
            //items
            foreach (Item item in items)
            {
                if (drawArray[item.coords[0], item.coords[1]] == 1)
                {
                    spriteBatch.DrawString(symbols, item.getUniVal(), new Vector2(((item.coords[0] - currentCorner[0]) * tileWidth), ((item.coords[1] - currentCorner[1]) * tileHeight) + textScroll[textScrollIndex]), item.color);
                }
            }
            //boxes
            foreach (Box box in boxes)
            {
                spriteBatch.DrawString(symbols, box.uniVal, new Vector2((box.coords[0] - currentCorner[0]) * tileWidth, (box.coords[1] - currentCorner[1]) * tileHeight), Color.Beige);
            }

            //UI
            for (int i = 0; i < player.health; i++) { spriteBatch.DrawString(symbols, "\u2665", new Vector2(10 + (i * 30), 10), Color.Red); }
            for (int i = 0; i < ui.getMaxHealth() - player.health; i++) { spriteBatch.DrawString(symbols, "\u2665", new Vector2(10 + (30 * player.health) + (i * 30), 10), Color.Gray); }
            for (int i = 0; i < player.inventory.Count; i++)
            {
                if (i == player.select)
                {
                    spriteBatch.DrawString(symbols, Item.getUniVal(player.inventory[i]), new Vector2(10 + (40 * i) - 2, 55), Color.Yellow);
                    spriteBatch.DrawString(symbols, Item.getUniVal(player.inventory[i]), new Vector2(10 + (40 * i) + 2, 55), Color.Yellow);
                    spriteBatch.DrawString(symbols, Item.getUniVal(player.inventory[i]), new Vector2(10 + (40 * i), 57), Color.Yellow);
                    spriteBatch.DrawString(symbols, Item.getUniVal(player.inventory[i]), new Vector2(10 + (40 * i), 53), Color.Yellow);
                }

                //player
                spriteBatch.DrawString(symbols, Item.getUniVal(player.inventory[i]), new Vector2(10 + (40 * i), 55), Item.getColor(player.inventory[i]));
            }
            spriteBatch.DrawString(output, player.inventory[player.select], new Vector2(10, 105), Color.White);
            spriteBatch.DrawString(symbols, tiles[scene.getTile(new int[] { player.coords[0] + currentCorner[0] + player.facing[0], player.coords[1] + currentCorner[1] + player.facing[1] }).getNum()], new Vector2((player.coords[0] + player.facing[0]) * tileWidth, (player.coords[1] + player.facing[1]) * tileHeight), Color.Silver);

            spriteBatch.DrawString(symbols, "\u265E", new Vector2(player.coords[0] * tileWidth - 5, player.coords[1] * tileHeight + 4), player.color);

            //textBox
            Texture2D textBox1 = new Texture2D(ingame.GraphicsDevice, 1, 1);
            Texture2D textBox2 = new Texture2D(ingame.GraphicsDevice, 1, 1);
            textBox1.SetData(new Color[] { Color.Gray });
            spriteBatch.Draw(textBox1, new Rectangle(0, 618, 1380, 600), Color.Gray);
            textBox2.SetData(new Color[] { Color.Black });
            spriteBatch.Draw(textBox2, new Rectangle(0, 628, 1380, 600), Color.Gray);
            List<TextLine> lines = textBox.getText().ToList<TextLine>();
            for (int i = 0; i < lines.Count; i++)
            {
                spriteBatch.DrawString(output, lines[i].text, new Vector2(20, 633 + (20 * i)), colorDict[lines[i].color]);
            }
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
            spriteBatch.DrawString(output, "FPS: " + Convert.ToString(frames), new Vector2(1150, 5), Color.White);
            spriteBatch.DrawString(output, "Facing: " + Convert.ToString(player.facing[0] + "," + player.facing[1]), new Vector2(1150, 35), Color.White);
            spriteBatch.DrawString(output, "Corner: " + Convert.ToString(currentCorner[0] + "," + currentCorner[1]), new Vector2(1150, 65), Color.White);
            spriteBatch.DrawString(output, "Area: " + scene.currentArea, new Vector2(1150, 95), Color.White);
            spriteBatch.DrawString(output, "Player: " + Convert.ToString(player.coords[0] + "," + player.coords[1]), new Vector2(1150, 125), Color.White);

        }
        //gainsboro, gray, darkslategray, black
        //blue, midnightblue, darkslatgray, black

        public void drawTiles()
        {
            //string[] tiles = { "\u2591", "\u2588", "\u2593", "\u2592"};
            Color[] colors = { Color.DimGray, Color.DarkCyan, Color.BurlyWood, Color.Aqua };
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
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + x, startingTile[1] + y + 1 }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + y, startingTile[1] + x }));
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + y + 1, startingTile[1] + x }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - x, startingTile[1] + y }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - y, startingTile[1] + x }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - x, startingTile[1] - y }));
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - x - 1, startingTile[1] - y }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - y, startingTile[1] - x }));
                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] - y - 1, startingTile[1] - x }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + x, startingTile[1] - y }));

                tileQuerry.Add(getTilesInbetween(startingTile, new int[] { startingTile[0] + y, startingTile[1] - x }));
                y++;
                if (radiusError < 0) { radiusError += 2 * y + 1; }
                else { x--; radiusError += 2 * (y - x) + 1; }
            }
            //foreach (int[] a in tileQuerry) { Debug.Print(Convert.ToString(a[0]) + "," + Convert.ToString(a[1])); }
            spriteBatch.DrawString(symbols, tiles[scene.getTile(new int[] { player.coords[0] + currentCorner[0], player.coords[1] + currentCorner[1] }).getNum()], new Vector2(player.coords[0] * tileWidth, player.coords[1] * tileHeight), colors[scene.getTile(new int[] { player.coords[0] + currentCorner[0], player.coords[1] + currentCorner[1] }).getNum()]);
            drawArray[player.coords[0] + currentCorner[0], player.coords[1] + currentCorner[1]] = 1;

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
                        if (!scene.isSolid(new int[] { lineTile[0] - currentCorner[0], lineTile[1] - currentCorner[1] }) && collide) { break; }
                        if (drawArray[lineTile[0], lineTile[1]] != 1) { spriteBatch.DrawString(symbols, tiles[scene.getTile(lineTile).getNum()], new Vector2((lineTile[0] - currentCorner[0]) * tileWidth, (lineTile[1] - currentCorner[1]) * tileHeight), colors[scene.getTile(lineTile).getNum()]); }
                        drawArray[lineTile[0], lineTile[1]] = 1;
                        if (scene.isSolid(new int[] { lineTile[0] - currentCorner[0], lineTile[1] - currentCorner[1] })) { collide = true; }
                        if (seenArray[lineTile[0], lineTile[1]] != 1) { seenArray[lineTile[0], lineTile[1]] = 1; }
                    }
                }
            }
            for (int i = 0; i < seenArray.GetLength(0); i++)
            {
                for (int j = 0; j < seenArray.GetLength(1); j++)
                {
                    if (seenArray[i, j] == 1 && drawArray[i, j] != 1)
                    {
                        Tile tile = scene.getTile(new int[] { i, j });
                        spriteBatch.DrawString(symbols, tiles[tile.getNum()], new Vector2((i - currentCorner[0]) * tileWidth, (j - currentCorner[1]) * tileHeight), Color.LightGray);
                    }
                }
            }

        }
        public static List<int[]> getTilesInbetween(int[] A, int[] B)
        {
            List<int[]> tileArray = new List<int[]>();
            if (((A[0] + B[0]) / 2 == A[0] && (A[1] + B[1]) / 2 == A[1]) || ((A[0] + B[0]) / 2 == B[0] && (A[1] + B[1]) / 2 == B[1]))
            { return tileArray; }
            tileArray.Add(new int[] { (A[0] + B[0]) / 2, (A[1] + B[1]) / 2 });
            tileArray.AddRange(getTilesInbetween(A, new int[] { (A[0] + B[0]) / 2, (A[1] + B[1]) / 2 }));
            tileArray.AddRange(getTilesInbetween(new int[] { (A[0] + B[0]) / 2, (A[1] + B[1]) / 2 }, B));
            return tileArray;

        }

        public Stack<int[]> AstarSearch(int[] A, int[] B)
        {
            Queue<Tile> frontier = new Queue<Tile>();
            frontier.Enqueue(scene.getTile(A));
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            cameFrom.Add(scene.getTile(A), scene.getTile(new int[] { 0, 0 }));

            while (frontier.Count > 0) 
            {
                Tile current = frontier.Dequeue();
                if (current.coords[0] == scene.getTile(B).coords[0] && current.coords[1] == scene.getTile(B).coords[1]) { break; }
                if (!cameFrom.ContainsKey(scene.getTile(current.coords[0]+1,current.coords[1])) 
                    && !scene.collidesAbsolute(new int[] { current.coords[0] + 1, current.coords[1] })) 
                        { frontier.Enqueue(scene.getTile(current.coords[0] + 1, current.coords[1])); cameFrom.Add(scene.getTile(current.coords[0]+1,current.coords[1]), current);}
                
                if (!cameFrom.ContainsKey(scene.getTile(current.coords[0], current.coords[1] + 1))
                    && !scene.collidesAbsolute(new int[] { current.coords[0], current.coords[1] + 1 }))
                        { frontier.Enqueue(scene.getTile(current.coords[0], current.coords[1] + 1)); cameFrom.Add(scene.getTile(current.coords[0], current.coords[1] + 1), current); }
                
                if (!cameFrom.ContainsKey(scene.getTile(current.coords[0] - 1, current.coords[1]))
                    && !scene.collidesAbsolute(new int[] { current.coords[0] - 1, current.coords[1] }))
                        { frontier.Enqueue(scene.getTile(current.coords[0] - 1, current.coords[1])); cameFrom.Add(scene.getTile(current.coords[0] - 1, current.coords[1]), current); }
                
                if (!cameFrom.ContainsKey(scene.getTile(current.coords[0], current.coords[1] - 1))
                    && !scene.collidesAbsolute(new int[] { current.coords[0], current.coords[1] - 1 }))
                        { frontier.Enqueue(scene.getTile(current.coords[0], current.coords[1] - 1)); cameFrom.Add(scene.getTile(current.coords[0], current.coords[1] - 1), current); }

            }
            Stack<int[]> path = new Stack<int[]>();
            path.Push(B);
            Tile next = new Tile(0, "0", 0, 0);
            next = cameFrom[scene.getTile(B)];
            while (cameFrom[next].coords[0] > 0 && cameFrom[next].coords[1] > 0) { path.Push(next.coords); next = cameFrom[next]; }
            return path;
        }

        public int getScreenDimension(int dim) { return screenDim[dim]; }
        public void addParticle(Particle particle) { this.particles.Add(particle); }
        public void addProjectile(Projectile projectile) { this.projectiles.Add(projectile); }
        public void addEnemy(Enemy enemy) { this.enemies.Add(enemy); }
        public void addItem(Item item) { this.items.Add(item); }
        public void addBox(Box box) { this.boxes.Add(box); }
        public void addStatic(StaticObject staticObject) { this.staticObjects.Add(staticObject); }

        public Projectile getProjectile(int index) { return projectiles[index]; }
        public Enemy getEnemy(string tag) { return enemies.Find(a => a.getTag() == tag); }
        public Item getItem(int index) { return items[index]; }
        public bool isSeen(int[] coordinates) { return drawArray[coordinates[0], coordinates[1]] == 1; }

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
