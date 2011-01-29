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
        public struct Info
        {
            public int width, height; // in number of cells, not yet used
            public int reproRate; // how long to wait before reproducing
            public int airRate; // how much air is breathed
            public int food; // how much energy an animal gets from eating this
            public int airCutoff; //how much oxygen before plants start dying
        }
        int reproduction;
        public Info info;
        public PlantCell()
            : base()
        {
            reproduction = 0;
            info = new Info();
            info.airRate = 1;
            info.reproRate = 3;
            info.food = 5;
            info.airCutoff = 3400;
        }
        public override void Update(GameTime gameTime, int x, int y)
        {
            if (updated)
                return;

            ExtGame.oxygen += info.airRate;
            if (ExtGame.oxygen > ExtGame.maxOxygen)
            {
                ExtGame.oxygen = ExtGame.maxOxygen;
            }
            if (ExtGame.oxygen > info.airCutoff)
            {
                float deathChance =
                    (int)(ExtGame.oxygen - info.airCutoff) /
                    (float)(ExtGame.maxOxygen - info.airCutoff);
                deathChance *= 100;
                if (r.Next(100) <= deathChance)
                {
                    ExtGame.RemoveCell(x, y);
                    return;
                }
            }

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
            if (ExtGame.grid[i, j] == 0 && ExtGame.oxygen < ExtGame.maxOxygen && reproduction > info.reproRate)
            {
                reproduction = 0;
                ExtGame.AddCell(i, j, new PlantCell());
                return true;
            }
            return false;
        }

        public override int Food()
        {
            return info.food;
        }
    }
}
