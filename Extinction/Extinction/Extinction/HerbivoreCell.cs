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
    public class HerbivoreCell: AnimalCell
    {
        public HerbivoreCell()
            : base()
        {
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
