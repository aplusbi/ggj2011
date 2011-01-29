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
    public class PlantCell: Cell
    {
        int reproduction;
        public PlantCell(int i)
            : base(i)
        {
            reproduction = 0;
        }
        public override void Update(GameTime gameTime, int x, int y)
        {
            if (updated)
                return;

            ++reproduction;
            base.Update(gameTime, x, y);
        }
        public override void Draw(SpriteBatch S, int x, int y) 
        {
            if (drawn)
                return;

            S.Draw(ExtGame.green_tile, new Vector2(x, y), Color.White);
        }
        public override bool DoStuff(int x, int y, int i, int j)
        {
            if (ExtGame.grid[i, j] == 0 && reproduction > 3)
            {
                reproduction = 0;
                ExtGame.AddCell(i, j, new PlantCell(0));
                return true;
            }
            return false;
        }
    }
}
