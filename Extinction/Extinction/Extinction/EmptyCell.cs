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
    public class EmptyCell : Cell
    {
        public EmptyCell()
            : base()
        {
        }
        public override void Update(GameTime gameTime, int x, int y) { }
        public override void Draw(SpriteBatch S, int x, int y) 
        {
            S.Draw(ExtGame.empty_tile, new Vector2(x, y), Color.White);
        }
        public override bool DoStuff(int x, int y, int i, int j)
        {
            return true;
        }
    }
}
