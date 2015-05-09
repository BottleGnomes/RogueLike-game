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

        public int[] currentCorner = { -15, 14 };
        int tileWidth = 28;
        int tileHeight = 46;
        int[] screenDim = { 50, 20 };

        int[] textScroll = new int[] { -4, -2, -1, 0, 0, 1, 2, 4, 2, 1, 0, 0, -1, -2 };
        int scrollTimer = 0;
        int textScrollIndex = 0;

        Player player;
        List<Enemy> enemies;
        List<Item> items;
        List<Particle> particles;

        public Playing(SpriteBatch spriteBatch, RogueLike ingame) 
        {
            paused = new Paused(this);
            this.ingame = ingame;
            scene = new Scene(this);
            symbols = ingame.Content.Load<SpriteFont>("symbols");
            output = ingame.Content.Load<SpriteFont>("Output18pt");
            this.spriteBatch = spriteBatch;
            menu = new ingameMenu(ingame, output);

            enemies = new List<Enemy> { new Enemy(new int[] { 3, 21 }, scene, 20, "\u2646","Your magic has no power in these lands, wizard!","sword") };
            enemies.Add(new Enemy(new int[] {3,5},scene,20,"\u2645","I am DEATH, nigh on apocalypse! Die, cur!","life"));
            items =  new List<Item> { new Item(new int[] { 3,18 },scene,"life") };
            particles = new List<Particle>();
            player = new Player(new int[] { 24, 5 }, scene);
            ui = new UI(player);


            //pass in enemies and objects later
            unpaused = new Unpaused(this, scene, player, enemies, items, particles);

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
            particles.RemoveAll(a => a.toBeDeleted);
            enemies.RemoveAll(a => a.toBeDeleted);
            state.update(gameTime);
        }

        public void draw()
        {
            //Debug.Print(Convert.ToString(currentCorner[0]) + "," + Convert.ToString(currentCorner[1]));

            this.drawTiles();
            //particles
            foreach (Particle particle in particles) 
            {
                Debug.Print(Convert.ToString(particle.getLocation()[0] - currentCorner[0]) + "," + Convert.ToString(particle.getLocation()[1] - currentCorner[1]));
                spriteBatch.DrawString(symbols, particle.getTag(), new Vector2 ((particle.getLocation()[0] - currentCorner[0])*tileWidth + particle.pixMod[0], (particle.getLocation()[1] - currentCorner[1])*tileHeight+particle.pixMod[1]), particle.color); 
            }

            //attack
            if (player.attacking) { spriteBatch.DrawString(symbols, player.attackSymbol, new Vector2((player.coords[0] + player.facing[0]) * tileWidth, (player.coords[1] + player.facing[1]) * tileHeight), Color.SandyBrown); }
            
            Vector2 textVector = new Vector2();
            if (textScrollIndex >= textScroll.Length - 1) { textScrollIndex = 0; } else if (scrollTimer > 168) { textScrollIndex++; scrollTimer = 0; }
            //enemies
            foreach (Enemy enemy in enemies) 
            { 
                spriteBatch.DrawString(symbols, enemy.uniVal, new Vector2((enemy.coords[0] - currentCorner[0])*tileWidth -5, (enemy.coords[1] - currentCorner[1])*tileHeight), enemy.color);
                if (enemy.speaking)
                {
                    textVector = new Vector2(((enemy.coords[0] - currentCorner[0]) * tileWidth) - (tileWidth * 10), ((enemy.coords[1] - currentCorner[1]) * tileHeight) - (tileHeight) + textScroll[textScrollIndex]);
                    spriteBatch.DrawString(output, enemy.getDialog(), new Vector2(textVector.X - 2, textVector.Y - 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog(), new Vector2(textVector.X + 2, textVector.Y + 1), Color.Black);
                    spriteBatch.DrawString(output, enemy.getDialog(), textVector, Color.White);

                }
            }
            foreach (Item item in items)
            {
                spriteBatch.DrawString(symbols, item.getUniVal(), new Vector2(((item.coords[0] - currentCorner[0]) * tileWidth), ((item.coords[1] - currentCorner[1]) * tileHeight) + textScroll[textScrollIndex]), item.color);
            }

            //UI
            spriteBatch.DrawString(symbols, ui.getHealth(), new Vector2(10, 10), Color.Red);
            spriteBatch.DrawString(symbols, ui.getMissingHealth(), new Vector2((ui.getHealth().Length * 27) + 4, 10), Color.Gray);

            //player on top during unpaused
            spriteBatch.DrawString(symbols, "\u265E", new Vector2(player.coords[0] * tileWidth - 5, player.coords[1] * tileHeight), player.color);

            if (state == paused) 
            {
                Texture2D dummyTexture = new Texture2D(ingame.GraphicsDevice, 1, 1);
                Texture2D dummyTexture2 = new Texture2D(ingame.GraphicsDevice, 1, 1);
                dummyTexture.SetData(new Color[] { Color.Gray });
                spriteBatch.Draw(dummyTexture, new Rectangle(200, 150, 900, 600), Color.Gray);
                dummyTexture2.SetData(new Color[] { Color.Black });
                spriteBatch.Draw(dummyTexture2, new Rectangle(215, 165, 870, 570), Color.Gray);

                menu.update();
                spriteBatch.DrawString(output, menu.draw(), new Vector2(225, 175), Color.White);
            }
        }
        //gainsboro, gray, darkslategray, black
        //blue, midnightblue, darkslatgray, black

        public void drawTiles()
        {
            string[] tiles = { "\u2591", "\u2589", "\u2593" };
            Color[,] colors = { { Color.Gainsboro, Color.Gray, Color.DarkSlateGray, Color.Black }, { Color.Blue, Color.MidnightBlue, Color.DarkSlateGray, Color.Black }, { Color.Gainsboro, Color.Gray, Color.DarkSlateGray, Color.Black } };
            int[] startingTile = new int[] {player.coords[0] + currentCorner[0], player.coords[1] + currentCorner[1]};
            List<List<int[]>> tileQuerry = new List<List<int[]>>();

            int x = player.sight;
            int y = 0;
            int radiusError = 1 - x;
            int r = player.sight;
            while(x >= y)
            {
                tileQuerry.Add(getTilesInbetween(new int[] { x + startingTile[0], y + startingTile[1] },new int[] { (-1) * x + startingTile[0], y + startingTile[1] }));
                tileQuerry.Add(getTilesInbetween(new int[] { y + startingTile[0], x + startingTile[1] }, new int[] { (-1) * y + startingTile[0], x + startingTile[1] }));
                tileQuerry.Add(getTilesInbetween(new int[] { (-1) * x + startingTile[0], (-1) * y + startingTile[1] }, new int[] { x + startingTile[0], (-1) * y + startingTile[1] }));
                tileQuerry.Add(getTilesInbetween(new int[] { (-1) * y + startingTile[0], (-1) * x + startingTile[1] }, new int[] { y + startingTile[0], (-1) * x + startingTile[1] }));
                y++;
                if (radiusError < 0) { radiusError += 2 * y + 1; }
                else {x--; radiusError += 2 * (y - x) + 1; }
            }
            //foreach (int[] a in tileQuerry) { Debug.Print(Convert.ToString(a[0]) + "," + Convert.ToString(a[1])); }
                
                foreach (List<int[]> tileList in tileQuerry)
                {
                    List<int[]> list = tileList.OrderByDescending(f => Math.Abs(f[0] - startingTile[0]) + Math.Abs(f[1] - startingTile[1])).ToList();
                    list.Reverse();
                    List<int[]> listForward = new List<int[]>();
                    List<int[]> listBackwards = new List<int[]>();
                    for(int i = 0; i < list.Count; i++)
                    {
                        if (i % 2 == 0) { listForward.Add(list[i]); }
                        else { listBackwards.Add(list[i]); }
                    }
                    //foreach (int[] whatever in listForward) { Debug.Print(Convert.ToString(whatever[0]) + "," + Convert.ToString(whatever[1])); }
                   
                    foreach (int[] lineTile in listForward)
                    {
                        //if (Math.Abs(distance[0]) + Math.Abs(distance[1]) <= player.sight) 
                        //{
                        if(scene.includesTile(lineTile)){
                            spriteBatch.DrawString(symbols, tiles[scene.getTile(lineTile).getNum()], new Vector2((lineTile[0] - currentCorner[0]) * tileWidth, (lineTile[1] - currentCorner[1]) * tileHeight), colors[scene.getTile(lineTile).getNum(), 0]);
                            //if (scene.collides(new int[]{lineTile[0] -currentCorner[0],lineTile[1] - currentCorner[1]})) { break; }
                        }//}
                        
                    }
                    foreach (int[] lineTile in listBackwards)
                    {
                        //if (Math.Abs(distance[0]) + Math.Abs(distance[1]) <= player.sight) 
                        //{
                        if (scene.includesTile(lineTile))
                        {
                            spriteBatch.DrawString(symbols, tiles[scene.getTile(lineTile).getNum()], new Vector2((lineTile[0] - currentCorner[0]) * tileWidth, (lineTile[1] - currentCorner[1]) * tileHeight), colors[scene.getTile(lineTile).getNum(), 0]);
                            //if (scene.collides(new int[]{lineTile[0] -currentCorner[0],lineTile[1] - currentCorner[1]})) { break; }
                        }//}

                    }
                    //process tile afterwards
                    //it's excluded in getTilesInbetween
                }

        }
        public List<int[]> getTilesInbetween(int[] A, int[] B)
        {
            List<int[]> tileArray = new List<int[]>();
            if (((A[0] + B[0]) / 2 == A[0] && (A[1] + B[1]) / 2 == A[1]) || ((A[0] + B[0]) / 2 == B[0] && (A[1] + B[1]) / 2 == B[1]))
            { return tileArray; }
            tileArray.Add(new int[] {(A[0] + B[0]) / 2, (A[1] + B[1]) / 2});
            tileArray.AddRange(getTilesInbetween(A, new int[]{(A[0] + B[0])/2, (A[1] + B[1])/2}));
            tileArray.AddRange(getTilesInbetween(new int[] {(A[0] + B[0]) / 2, (A[1] + B[1]) / 2}, B));
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
