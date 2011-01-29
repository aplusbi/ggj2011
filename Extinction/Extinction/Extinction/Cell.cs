using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Extinction
{
    public abstract class Cell
    {
        int[,] grid;
        int id = -1;
        public bool updated;
        public Cell(int i, int[,] g)
        {
            id = i;
            grid = g;
            updated = false;
        }

        public abstract void Update(GameTime gameTime, int x, int y);
        public abstract void Draw(int x, int y);
    }
}
