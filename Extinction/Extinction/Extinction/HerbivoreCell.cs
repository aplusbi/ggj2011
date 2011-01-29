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

        public override void Reproduce(int i, int j)
        {
            ExtGame.AddCell(i, j, new PlantCell());
        }
        public override bool IsFood(Cell c)
        {
            return c is PlantCell;
        }
    }
}
