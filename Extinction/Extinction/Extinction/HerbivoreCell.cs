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
        public static int count = 0;
        public HerbivoreCell()
            : base()
        {
            ++count;
        }
        public HerbivoreCell(Info i)
            : base(i)
        {
            ++count;
        }
        ~HerbivoreCell()
        {
            --count;
        }

        public override bool Reproduce(int i, int j)
        {
            ExtGame.AddCell(i, j, new HerbivoreCell(info));
            return true;
        }
        public override bool IsFood(Cell c)
        {
            return c is PlantCell;
        }
        public override int FoodCount()
        {
            return PlantCell.count;
        }
        public override bool IsMate(Cell c)
        {
            return c is HerbivoreCell;
        }
        public override void Draw(SpriteBatch S, int x, int y)
        {
            if (drawn)
                return;

            S.Draw(ExtGame.red_tile, new Vector2(x, y), Color.White);
        }
    }
}
