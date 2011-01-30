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
        public struct world_info
        {
            public int currency { get; set; }
            public int moneygainrate { get; set; }
        }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Song morning;
        public static int width, height;
        public static int cwidth, cheight;
        int score;
        int currency;
        int moneygainrate;
        bool is_in_title = true;
        bool has_begun = false;
        bool game_over = false;
        int credits_timer = 180;
        int offx, offy;
        int cursorx, cursory;
        static int currID = 1;
        public static int[,] grid;
        public static Dictionary<int, Cell> cells;
        public static int oxygen, maxOxygen;
        Texture2D title_screen, credits;
        Texture2D background;
        TimeSpan elapsed;
        public SpriteFont theFont;
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
        List<PlantCell.Info> plantinfos;
        List<AnimalCell.Info> herbivoreinfos;
        List<AnimalCell.Info> carnivoreinfos;
        public static Dictionary<string, Texture2D> animaltextures = new Dictionary<string,Texture2D>();
        
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
            
            maxOxygen = 40000;
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

            theFont = Content.Load<SpriteFont>("ComicSans");
            background = Content.Load<Texture2D>("background");
            empty_tile = Content.Load<Texture2D>("brown_tile");
            green_tile = Content.Load<Texture2D>("green_tile");
            red_tile = Content.Load<Texture2D>("red_tile");
            animaltextures.Add("red_tile", red_tile);
            orange_tile = Content.Load<Texture2D>("orange_tile");
            animaltextures.Add("orange_tile", orange_tile);
            animaltextures.Add("man_tile", Content.Load<Texture2D>("man_tile"));
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
            title_screen = Content.Load<Texture2D>("title");
            credits = Content.Load<Texture2D>("credits");

           
            // TODO: use this.Content to load your game content here

            Button B = new Button("Plants", button_up, button_down, 
                plants_overlay, 130, 10);
            B.Pressed += new Button.ButtonPressedHandler(Plants_Pressed);
            B.Pressed += new Button.ButtonPressedHandler(Cells_Pressed);
            buttons.Add(B);
            B = new Button("Herbivore", button_up, button_down, 
                herb_overlay, 130, 10+ Button.bheight);
            B.Pressed += new Button.ButtonPressedHandler(Herbivore_Pressed);
            B.Pressed += new Button.ButtonPressedHandler(Cells_Pressed);
            buttons.Add(B);
            B = new Button("Carnivore", button_up, button_down,  
                carn_overlay, 130, 10+(Button.bheight)*2);
            B.Pressed += new Button.ButtonPressedHandler(Carnivore_Pressed);
            B.Pressed += new Button.ButtonPressedHandler(Cells_Pressed);
            buttons.Add(B);

            B = new NormalButton("FastForward", button_up, button_down,
              fastforward_overlay, 130, 10 + (Button.bheight) * 4);
            B.Pressed += new Button.ButtonPressedHandler(FF_Pressed);
            buttons.Add(B);

            B = new NormalButton("Reverse", button_up, button_down,
              reverse_overlay, 130, 10 + (Button.bheight) * 5);
            B.Pressed += new Button.ButtonPressedHandler(Rev_Pressed);
            buttons.Add(B);

            morning = Content.Load<Song>("morning");
            MediaPlayer.Play(morning);
            MediaPlayer.IsRepeating = true;
        }

        void Cells_Pressed(object sender, EventArgs e)
        {
            /*if (!has_begun)
            {
                has_begun = true;
            }*/
        }

        void Plants_Pressed(object sender, EventArgs e)
        {
            if (game_over) return;
            new_cell_type = typeof(PlantCell);
            foreach (Button B in buttons)
                if (B != sender as Button)
                    B.DeSelect();
        }
        void Herbivore_Pressed(object sender, EventArgs e)
        {
            if (game_over) return;
            new_cell_type = typeof(HerbivoreCell);
            foreach (Button B in buttons)
                if (B != sender as Button)
                    B.DeSelect();
        }
        void Carnivore_Pressed(object sender, EventArgs e)
        {
            if (game_over) return;
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
            currency = 2000;
            credits_timer = 180;
            is_in_title = true;
            game_over = false;
            has_begun = false;
            score = 0;
            oxygen = maxOxygen/2;
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
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            using (StreamReader reader =
                new StreamReader(Content.RootDirectory + "\\plants.txt"))
            {
                line = reader.ReadToEnd();
                {
                    plantinfos = serializer.Deserialize<List<PlantCell.Info>>(line);
                }
            }
            using (StreamReader reader =
                new StreamReader(Content.RootDirectory + "\\herbivores.txt"))
            {
                line = reader.ReadToEnd();
                {
                    herbivoreinfos = serializer.Deserialize<List<HerbivoreCell.Info>>(line);
                }
            }
            using (StreamReader reader =
                new StreamReader(Content.RootDirectory + "\\carnivores.txt"))
            {
                line = reader.ReadToEnd();
                {
                    carnivoreinfos = serializer.Deserialize<List<CarnivoreCell.Info>>(line);
                }
            }
            using (StreamReader reader =
               new StreamReader(Content.RootDirectory + "\\world_info.txt"))
            {
                line = reader.ReadToEnd();
                {
                    List<ExtGame.world_info> world_infos = serializer.Deserialize<List<ExtGame.world_info>>(line);
                    this.currency = world_infos[0].currency;
                    this.moneygainrate = world_infos[0].moneygainrate;
                }
            }
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
            bool isPlants = false, isHerbs = false, isCarn = false;
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
                    if (c is PlantCell) isPlants = true;
                    if (c is HerbivoreCell) isHerbs = true;
                    if (c is CarnivoreCell) isCarn = true;
                    c.updated = false;
                }
                if (isPlants && isHerbs && isCarn) this.currency += this.moneygainrate;
                HerbivoreCell.seekCount = 0;
                CarnivoreCell.seekCount = 0;
                if(has_begun && !game_over) score++;
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

            if (mousex >= 0 && mousex <= (width) * cwidth)
            {
                if (mousey >= 0 && mousey <= (height) * cheight)
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
                                HerbivoreCell A = new HerbivoreCell(herbivoreinfos[0]);
                                if (currency >= A.info.cost)
                                {
                                    AddCell(cellx, celly, A);
                                    currency -= A.info.cost;
                                }
                            }
                            else if (new_cell_type == typeof(CarnivoreCell))
                            {
                                CarnivoreCell A = new CarnivoreCell(carnivoreinfos[0]);
                                if (currency >= A.info.cost)
                                {
                                    AddCell(cellx, celly, A);
                                    currency -= A.info.cost;
                                }
                            }
                            else
                            {
                                PlantCell A = new PlantCell(plantinfos[0]);
                                if (currency >= A.info.cost)
                                {
                                    AddCell(cellx, celly, A);
                                    currency -= A.info.cost;
                                }
                            }
                            if (!has_begun)
                            {
                                has_begun = true;
                            }
                        }
                    }
                }
            }
            if (cells.Count() == 1 && has_begun)
            {
                MediaPlayer.Stop();
                game_over = true;
            }
            if (game_over == true) credits_timer--;

            foreach (Button B in buttons)
            {
                B.Update(mouse_state);
            }

            Keys [] pressedkeys = Keyboard.GetState().GetPressedKeys();
            if (pressedkeys.Count() > 0)
            {
                is_in_title = false;
            }
            if (mouse_state.LeftButton == ButtonState.Pressed ||
                mouse_state.RightButton == ButtonState.Pressed)
                is_in_title = false;


            //check for reset
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                this.Reset();
                MediaPlayer.Play(morning);
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

            spriteBatch.DrawString(theFont, "Score: " + score.ToString(),
                new Vector2(130, 20 + (Button.bheight) * 6), Color.White);
            spriteBatch.DrawString(theFont, "Currency: " + currency.ToString(),
                new Vector2(130, 40 + (Button.bheight) * 6), Color.White);

            spriteBatch.DrawString(theFont, "HOW TO PLAY",
                new Vector2(30, 100 + (Button.bheight) * 6), Color.White);
            spriteBatch.DrawString(theFont, "* Place 3 types of Creatures",
                new Vector2(30, 120 + (Button.bheight) * 6), Color.White);
            spriteBatch.DrawString(theFont, "* Carnivores eat Herbivores",
                new Vector2(30, 140 + (Button.bheight) * 6), Color.White);
            spriteBatch.DrawString(theFont, "* Herbivores eat Plants",
                new Vector2(30, 160 + (Button.bheight) * 6), Color.White);
            spriteBatch.DrawString(theFont, "* Gain currency with all 3",
                new Vector2(30, 180 + (Button.bheight) * 6), Color.White);
            spriteBatch.DrawString(theFont, "* Survive!",
                new Vector2(30, 200 + (Button.bheight) * 6), Color.White);


            if (game_over)
            {
                string endstring = "EXTINCTION ACHIEVED! PRESS R TO RESTART";
                Vector2 stringsize = theFont.MeasureString(endstring);
                //stringsize.X += 32;
                spriteBatch.DrawString(theFont, endstring,
                    new Vector2(offx + (width*cwidth)/2- stringsize.X/2, offy + (height * cheight) / 2), 
                    Color.White);
            }

            if (is_in_title)
            {
                spriteBatch.Draw(title_screen, new Vector2(0, 0), Color.White);
            }
            if (game_over && credits_timer <= 0)
            {
                spriteBatch.Draw(credits, new Vector2(0, 0), Color.White);
            }

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
            Cell c = cells[id];
            if (c is HerbivoreCell)
                HerbivoreCell.count--;
            else if (c is PlantCell)
                PlantCell.count--;
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
