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
    public abstract class Cell
    {
        protected int id = -1;
        public bool updated;
        public bool drawn;
        static protected Random r = new Random();

        public Cell()
        {
            updated = false;
            drawn = false;
        }
        public virtual void Reset()
        {
            updated = false;
            drawn = false;
        }

        public void Shuffle(int[,] array, int length)
        {
            for (int i = length-1; i >= 0; --i)
            {
                int index = r.Next(0, i);
                int tempx = array[i, 0];
                int tempy = array[i, 1];
                array[i, 0] = array[index, 0];
                array[i, 1] = array[index, 1];
                array[index, 0] = tempx;
                array[index, 1] = tempy;
            }
        }

        public int Spots(int x, int y, int[,] arr)
        {
            int index = 0;
            for (int j = Math.Max(y - 1, 0); j <= Math.Min(y + 1, ExtGame.height - 1); ++j)
            {
                for (int i = Math.Max(x - 1, 0); i <= Math.Min(x + 1, ExtGame.width - 1); ++i)
                {
                    if ((i != x || j != y))
                    {
                        arr[index, 0] = i;
                        arr[index, 1] = j;
                        ++index;
                    }
                }
            }
            return index;
        }

        public virtual void Update(GameTime gameTime, int x, int y)
        {
            if (updated)
                return;
            updated = true;

            // check everything around it
            int[,] spots = new int[8,2];
            int index = Spots(x, y, spots);
            Shuffle(spots, index);

            for(int i=0; i<index; ++i)
            {
                if(DoStuff(x, y, spots[i,0], spots[i,1]))
                    break;
            }
            
        }
        public abstract void Draw(SpriteBatch S, int x, int y);
        public abstract bool DoStuff(int x, int y, int i, int j);
        public virtual int Food() { return 0; }
    }
}
