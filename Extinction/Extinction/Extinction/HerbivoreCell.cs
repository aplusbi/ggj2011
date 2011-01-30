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
        public static int seekCount = 0;
        public HerbivoreCell(Info i)
            : base(i)
        {
        }
        public override void Reset()
        {
            ++count;
            base.Reset();
        }

        public override void Birth(int x, int y)
        {
            ExtGame.AddCell(x, y, typeof(HerbivoreCell));
        }
        public override bool IsFood(Cell c)
        {
            return c is PlantCell;
        }
        public override bool FoodCount()
        {
            return seekCount++ < PlantCell.count;
        }
        public override bool IsMate(Cell c)
        {
            return c is HerbivoreCell;
        }
        public override void Draw(SpriteBatch S, int x, int y)
        {
            if (drawn)
                return;

            S.Draw(ExtGame.animaltextures[this.info.textureFile],
                new Vector2(x, y), Color.White);
        }
    }
}
