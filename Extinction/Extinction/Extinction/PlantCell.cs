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
            public int width { get; set; }
            public int height { get; set; } // in number of cells, not yet used
            public int reproRate { get; set; } // how long to wait before reproducing
            public int airRate { get; set; } // how much air is breathed
            public int food { get; set; } // how much energy an animal gets from eating this
            public int airCutoff { get; set; } //how much oxygen before plants start dying
        }
        int reproduction;
        public Info info;
        public static int count = 0;
        public PlantCell()
        {
            reproduction = 0;
            ++count;
        }
        public PlantCell(Info i)
        {
            info = i;
            reproduction = 0;
            ++count;
        }
        ~PlantCell()
        {
            --count;
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
                /*float deathChance =
                    (int)(ExtGame.oxygen - info.airCutoff) /
                    (float)(ExtGame.maxOxygen - info.airCutoff);
                deathChance *= 100;
                if (r.Next(100) <= deathChance)*/
                int cutOff = (ExtGame.maxOxygen - info.airCutoff) / 100;
                if(r.Next((ExtGame.maxOxygen - ExtGame.oxygen)/cutOff) == 0)
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
            if (ExtGame.grid[i, j] == 0 && ExtGame.oxygen < info.airCutoff && reproduction > info.reproRate)
            {
                reproduction = 0;
                ExtGame.AddCell(i, j, new PlantCell(info));
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
