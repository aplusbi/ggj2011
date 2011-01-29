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
        Texture2D plants_overlay, herb_overlay, carn_overlay;
        Texture2D button_up;
        Texture2D button_down;
        Type new_cell_type;

        
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
            offy = 30;
            oxygen = 2000;
            maxOxygen = 4000;
            grid = new int[width, height];
            for (int h = 0; h < height; ++h)
            {
                for (int w = 0; w < width; ++w)
                {
                    grid[w, h] = 0;
                }
            }

            cells.Add(0, new EmptyCell());
            AddCell(30, 30, new PlantCell());
            AddCell(6, 6, new PlantCell());
            AddCell(5, 5, new HerbivoreCell());
            AddCell(20, 20, new HerbivoreCell());
            AddCell(25, 20, new HerbivoreCell());
            AddCell(30, 20, new HerbivoreCell());
            AddCell(35, 20, new HerbivoreCell());
            AddCell(33, 20, new HerbivoreCell());

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
            cursor_tex = Content.Load<Texture2D>("cursor");
            plants_overlay = Content.Load<Texture2D>("plants");
            carn_overlay = Content.Load<Texture2D>("carnivore");
            herb_overlay = Content.Load<Texture2D>("herbivore");
            button_up = Content.Load<Texture2D>("button_up");
            button_down = Content.Load<Texture2D>("button_down");
            // TODO: use this.Content to load your game content here

            Button B = new Button("Plants", button_up, button_down, 
                plants_overlay, 0, 0);
            B.Pressed += new Button.ButtonPressedHandler(Plants_Pressed);
            buttons.Add(B);
            B = new Button("Herbivore", button_up, button_down, 
                herb_overlay, 0, Button.bheight);
            B.Pressed += new Button.ButtonPressedHandler(Herbivore_Pressed);
            buttons.Add(B);
            B = new Button("Carnivore", button_up, button_down,  
                carn_overlay, 0, Button.bheight*2);
            B.Pressed += new Button.ButtonPressedHandler(Carnivore_Pressed);
            buttons.Add(B);

            LoadWorldInfo();
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
            string line = "";
            using (StreamReader reader =
                new StreamReader(Content.RootDirectory + "\\world_info.txt"))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
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
            if(elapsed >= new TimeSpan(0,0,0,1,000))
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
                    if (mouse_state.LeftButton == ButtonState.Pressed)
                    {
                        int cellx = cursorx / cwidth, celly = cursory / cheight;
                        Cell C = System.Activator.CreateInstance(new_cell_type) as Cell;
                        if (cells[grid[cellx, celly]] is EmptyCell)
                        {
                            AddCell(cellx, celly, C);
                        }
                    }
                }

            foreach (Button B in buttons)
            {
                B.Update(mouse_state);
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
