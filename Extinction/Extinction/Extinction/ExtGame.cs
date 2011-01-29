using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Extinction
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ExtGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static int width, height;
        public static int cwidth, cheight;
        int offx, offy;
        int cursorx, cursory;
        static int currID = 1;
        public static int[,] grid;
        public static Dictionary<int, Cell> cells;
        public static int oxygen, maxOxygen;
        Texture2D background;
        TimeSpan elapsed;
        public static Texture2D empty_tile;
        public Texture2D cursor_tex;
        public static Texture2D green_tile;
        public static Texture2D red_tile;
        public static Texture2D orange_tile;
        Texture2D airbar, bar_seperator;
        int barsize = 568, bar_offset = 40;
        Texture2D plants_overlay, herb_overlay, carn_overlay, fastforward_overlay,
            reverse_overlay;
        Texture2D button_up;
        Texture2D button_down;
        Type new_cell_type;
        long turn_amount;
        List<PlantCell.Info> plantinfos = new List<PlantCell.Info>();
        List<AnimalCell.Info> animalinfos = new List<AnimalCell.Info>();
        
        public MouseState mouse_state;
        List<Button> buttons;

        public ExtGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            buttons = new List<Button>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            cells = new Dictionary<int, Cell>();
            width = 40;
            height = 40;
            cwidth = 16;
            cheight = 16;
            offx = 300;
            offy = 5;
            oxygen = 2000;
            maxOxygen = 4000;
            grid = new int[width, height];
            this.Reset();
            base.Initialize();
            IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 1000;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            empty_tile = Content.Load<Texture2D>("brown_tile");
            green_tile = Content.Load<Texture2D>("green_tile");
            red_tile = Content.Load<Texture2D>("red_tile");
            orange_tile = Content.Load<Texture2D>("orange_tile");
            cursor_tex = Content.Load<Texture2D>("cursor");
            plants_overlay = Content.Load<Texture2D>("plants");
            carn_overlay = Content.Load<Texture2D>("carnivore");
            herb_overlay = Content.Load<Texture2D>("herbivore");
            button_up = Content.Load<Texture2D>("button_up");
            button_down = Content.Load<Texture2D>("button_down");
            airbar = Content.Load<Texture2D>("airbar");
            bar_seperator = Content.Load<Texture2D>("bar_seperator");
            fastforward_overlay = Content.Load<Texture2D>("fastforward");
            reverse_overlay = Content.Load<Texture2D>("reverse");
           
            // TODO: use this.Content to load your game content here

            Button B = new Button("Plants", button_up, button_down, 
                plants_overlay, 130, 10);
            B.Pressed += new Button.ButtonPressedHandler(Plants_Pressed);
            buttons.Add(B);
            B = new Button("Herbivore", button_up, button_down, 
                herb_overlay, 130, 10+ Button.bheight);
            B.Pressed += new Button.ButtonPressedHandler(Herbivore_Pressed);
            buttons.Add(B);
            B = new Button("Carnivore", button_up, button_down,  
                carn_overlay, 130, 10+(Button.bheight)*2);
            B.Pressed += new Button.ButtonPressedHandler(Carnivore_Pressed);
            buttons.Add(B);

            B = new NormalButton("FastForward", button_up, button_down,
              fastforward_overlay, 130, 10 + (Button.bheight) * 4);
            B.Pressed += new Button.ButtonPressedHandler(FF_Pressed);
            buttons.Add(B);

            B = new NormalButton("Reverse", button_up, button_down,
              reverse_overlay, 130, 10 + (Button.bheight) * 5);
            B.Pressed += new Button.ButtonPressedHandler(Rev_Pressed);
            buttons.Add(B);
        }

        void Plants_Pressed(object sender, EventArgs e)
        {
            new_cell_type = typeof(PlantCell);
            foreach (Button B in buttons)
                if (B != sender as Button)
                    B.DeSelect();
        }
        void Herbivore_Pressed(object sender, EventArgs e)
        {
            new_cell_type = typeof(HerbivoreCell);
            foreach (Button B in buttons)
                if (B != sender as Button)
                    B.DeSelect();
        }
        void Carnivore_Pressed(object sender, EventArgs e)
        {
            new_cell_type = typeof(CarnivoreCell);
            foreach (Button B in buttons)
                if (B != sender as Button)
                    B.DeSelect();
        }

        void FF_Pressed(object sender, EventArgs e)
        {
            turn_amount /= 2;
            if (turn_amount < 1)
                turn_amount = 1;
        }
        void Rev_Pressed(object sender, EventArgs e)
        {
            turn_amount *= 2;
        }

        public void Reset()
        {
            oxygen = 2000;
            cells.Clear();
            for (int h = 0; h < height; ++h)
            {
                for (int w = 0; w < width; ++w)
                {
                    grid[w, h] = 0;
                }
            }
            cells.Add(0, new EmptyCell());
            LoadWorldInfo();   

            turn_amount = 1000;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void LoadWorldInfo()
        {
            Random R = new Random();
            //int num_plants = 0;
            //int num_herb = 0;
            string line = "";
            using (StreamReader reader =
                new StreamReader(Content.RootDirectory + "\\world_info.txt"))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string[] paramstrings = line.Split(new char[] { '(', ',', ')' });

                    if (line.StartsWith("Plants"))
                    {
                        PlantCell.Info pinfo = new PlantCell.Info();
                        pinfo.width = int.Parse(paramstrings[1]);
                        pinfo.height = int.Parse(paramstrings[2]);
                        pinfo.reproRate = int.Parse(paramstrings[3]);
                        pinfo.airRate = int.Parse(paramstrings[4]);
                        pinfo.food = int.Parse(paramstrings[5]);
                        pinfo.airCutoff = int.Parse(paramstrings[6]);
                        plantinfos.Add(pinfo);

                    }
                    if (line.StartsWith("Herbivores") || line.StartsWith("Carnivores"))
                    {
                        AnimalCell.Info pinfo = new AnimalCell.Info();
                        pinfo.width = int.Parse(paramstrings[1]);
                        pinfo.height = int.Parse(paramstrings[2]);
                        pinfo.reproRate = int.Parse(paramstrings[3]);
                        pinfo.airRate = int.Parse(paramstrings[4]);
                        pinfo.food = int.Parse(paramstrings[5]);
                        pinfo.sated = int.Parse(paramstrings[6]);
                        pinfo.starved = int.Parse(paramstrings[7]);
                        pinfo.lifeExectancy = int.Parse(paramstrings[8]);
                        pinfo.airCutoff = int.Parse(paramstrings[9]);
                        animalinfos.Add(pinfo);
                    }
                }
            }
            /*for (int ii = 0; ii < num_plants; ii++)
                AddCell(R.Next(0, width), R.Next(0, height), new PlantCell());
            for (int ii = 0; ii < num_herb; ii++)
                AddCell(R.Next(0, width), R.Next(0, height), new HerbivoreCell());*/
            //Content.RootDirectory
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            elapsed = elapsed + gameTime.ElapsedGameTime;
            TimeSpan span = new TimeSpan(turn_amount*10000);
            if(elapsed >= span)
            {
                Console.WriteLine("Oxygen: " + oxygen);
                elapsed = new TimeSpan(0);
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        cells[grid[x, y]].Update(gameTime, x, y);

                    }
                }
                foreach(Cell c in cells.Values)
                {
                    c.updated = false;
                }
            }

            //update the position of the cursor based on the mouse position
            mouse_state = Mouse.GetState();
            int mousex = mouse_state.X;
            int mousey = mouse_state.Y;

            mousex -= offx;
            mousey -= offy;

            cursorx = mousex - (mousex % cwidth);
            cursory = mousey - (mousey % cheight);

            if (cursorx > (width - 1) * cwidth) cursorx = (width - 1) * cwidth;
            if (cursory > (height - 1) * cheight) cursory = (height - 1) * cheight;
            if (cursorx < 0) cursorx = 0;
            if (cursory < 0) cursory = 0;

            if(mousex >= 0 && mousex <= (width - 1) * cwidth)
                if (mousey >= 0 && mousey <= (height - 1) * cheight)
                {
                    if (mouse_state.LeftButton == ButtonState.Pressed &&
                        new_cell_type != null)
                    {
                        int cellx = cursorx / cwidth, celly = cursory / cheight;
                        //Cell C = System.Activator.CreateInstance(new_cell_type) as Cell;
                        if (cells[grid[cellx, celly]] is EmptyCell)
                        {
                            if (new_cell_type == typeof(HerbivoreCell))
                            {
                                HerbivoreCell A = new HerbivoreCell(animalinfos[0]);              
                                AddCell(cellx, celly, A);
                            }
                            else if (new_cell_type == typeof(CarnivoreCell))
                            {
                                CarnivoreCell A = new CarnivoreCell(animalinfos[1]);
                                AddCell(cellx, celly, A);
                            }
                            else
                            {
                                PlantCell A = new PlantCell(plantinfos[0]);
                                AddCell(cellx, celly, A);
                            }
                        }
                    }
                }

            foreach (Button B in buttons)
            {
                B.Update(mouse_state);
            }

            //check for reset
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                this.Reset();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(offx, offy), Color.White);
            for (int h = 0; h < height; ++h)
            {
                for (int w = 0; w < width; ++w)
                {
                    int x = offx + w * cwidth;
                    int y = offy + h * cheight;
                    cells[grid[w, h]].Draw(spriteBatch, x, y);
                }
            }
            foreach (Cell c in cells.Values)
            {
                c.drawn = false;
            }
            //draw the cursor
            spriteBatch.Draw(cursor_tex, 
                new Vector2(cursorx + offx, cursory + offy), Color.White);

            foreach (Button B in buttons)
            {
                B.Draw(spriteBatch);
            }
            
            float oxplus = ((float)oxygen / (float)maxOxygen) * barsize;
            float colorval = ((float)oxygen / (float)maxOxygen) * 255;
            Color C = new Color((int)colorval, 0, (int)(255 - colorval));
            spriteBatch.Draw(airbar, 
                new Vector2(offx, offy + height * cheight), C);
            spriteBatch.Draw(bar_seperator, 
                new Vector2(offx + oxplus + bar_offset, offy + height * cheight),
                Color.Gold);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static int GetID()
        {
            return currID++;
        }

        public static bool AddCell(int x, int y, Cell c)
        {
            if (grid[x, y] != 0)
                return false;

            int id = GetID();
            cells.Add(id, c);
            grid[x, y] = id;

            return true;
        }
        public static void RemoveCell(int x, int y)
        {
            int id = grid[x, y];
            cells.Remove(id);
            for (int h = 0; h < height; ++h)
            {
                for (int w = 0; w < width; ++w)
                {
                    if(grid[x,y] == id)
                        grid[x, y] = 0;
                }
            }
        }
    }
}
