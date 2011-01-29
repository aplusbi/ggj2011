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
        int hunger, sated, starved;
        public int mated;
        public int age;
        public HerbivoreCell()
            : base(0)
        {
            hunger = 100;
            sated = 70;
            starved = 30;
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
                ExtGame.RemoveCell(x, y);
                return;
            }
            if (++age > 100)
            {
                ExtGame.RemoveCell(x, y);
                return;
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
            if (neighbor is HerbivoreCell && mated > 10)
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
            if (neighbor is PlantCell && hunger < sated)
            {
                ExtGame.RemoveCell(i, j);
                hunger += 5;
                return true;
            }
            return false;
        }
    }
}
