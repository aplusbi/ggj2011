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
    public class HerbivoreCell: Cell
    {
        public struct Info
        {
            public int sated, starved;
            public int reproRate;
            public int airRate;
            public int lifeExectancy;
        }
        Info info;
        int hunger;
        public int mated;
        public int age;
        public HerbivoreCell()
            : base()
        {
            info.sated = 200;
            info.starved = 75;
            info.airRate = -5;
            info.lifeExectancy = 100;

            hunger = info.sated;
            mated = 0;
            age = 0;
        }

        public override void Update(GameTime gameTime, int x, int y)
        {
            if (updated)
                return;

            ++mated;

            if (--hunger < 0)
            {
                if (r.Next(9) == 0)
                {
                    ExtGame.RemoveCell(x, y);
                    return;
                }
            }
            if (++age > info.lifeExectancy)
            {
                int max = Math.Max(0, info.lifeExectancy / 4 - (age - info.lifeExectancy));
                if (r.Next(max) == 0)
                {
                    ExtGame.RemoveCell(x, y);
                    return;
                }
            }
            
            ExtGame.oxygen += info.airRate;
            if (ExtGame.oxygen < 0)
            {
                ExtGame.oxygen = 0;
                if (r.Next(9) == 0)
                {
                    ExtGame.RemoveCell(x, y);
                    return;
                }
            }

            base.Update(gameTime, x, y);
        }

        public override void Draw(SpriteBatch S, int x, int y)
        {
            if (drawn)
                return;

            S.Draw(ExtGame.red_tile, new Vector2(x, y), Color.White);
        }
        public override bool DoStuff(int x, int y, int i, int j)
        {
            Cell neighbor = ExtGame.cells[ExtGame.grid[i, j]];
            if (neighbor is HerbivoreCell && mated > info.reproRate && hunger > info.sated && ExtGame.oxygen > 0)
            {
                mated = -1;
                return false;
            }
            if (ExtGame.grid[i, j] == 0)
            {
                if (mated == -1)
                {
                    ExtGame.AddCell(i, j, new HerbivoreCell());
                }
                else
                {
                    ExtGame.grid[i, j] = ExtGame.grid[x, y];
                    ExtGame.grid[x, y] = 0;
                }
                return true;
            }
            if (neighbor is PlantCell && hunger < info.sated)
            {
                PlantCell p = neighbor as PlantCell;
                ExtGame.RemoveCell(i, j);
                hunger += p.Food();
                return true;
            }
            return false;
        }
    }
}
