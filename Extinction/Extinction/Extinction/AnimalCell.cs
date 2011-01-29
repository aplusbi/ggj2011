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
    public abstract class AnimalCell: Cell
    {
        public struct Info
        {
            public int width, height;
            public int reproRate;
            public int airRate;
            public int food;
            public int sated; // hunger level at which they are full
            public int starved; // hunger level at which they seek food
            public int lifeExectancy; // max age
        }
        protected Info info;
        protected int hunger;
        public int mated;
        public int age;
        public AnimalCell()
            : base()
        {
            info.sated = 100;
            info.starved = 75;
            info.airRate = 5;
            info.lifeExectancy = 1000000;
            info.food = 20;

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
            
            ExtGame.oxygen -= info.airRate;
            if (ExtGame.oxygen < 0)
            {
                ExtGame.oxygen = 0;
                if (r.Next(9) == 0)
                {
                    ExtGame.RemoveCell(x, y);
                    return;
                }
            }

            if (SeekFood(x, y))
                updated = true;

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
            if (neighbor is AnimalCell && mated > info.reproRate && ExtGame.oxygen > 0)
            {
                mated = -1;
                return false;
            }
            if (ExtGame.grid[i, j] == 0)
            {
                if (mated == -1)
                {
                    Reproduce(i, j);
                }
                else
                {
                    Move(x, y, i, j);
                }
                return true;
            }
            return Eat(neighbor, i, j);
        }
        public override int Food()
        {
            return info.food;
        }
        public void Move(int x, int y, int i, int j)
        {
            ExtGame.grid[i, j] = ExtGame.grid[x, y];
            ExtGame.grid[x, y] = 0;
        }

        struct Pair
        {
        }
        Queue<int[]> vertices = new Queue<int[]>();
        IDictionary<int, int[]> lookup = new Dictionary<int, int[]>();
        public bool SeekFood(int x, int y)
        {
            if (hunger > info.starved)
                return false;

            vertices.Clear();
            lookup.Clear();

            // vertex plus parent vertex
            vertices.Enqueue(new int[4]{x, y, x, y});
            lookup.Add( x + y * ExtGame.width, new int[4] { x, y, x, y });
            int[,] spots = new int[8, 2];

            while (vertices.Count > 0)
            {
                int[] v = vertices.Dequeue();
                int len = Spots(v[0], v[1], spots);
                for (int i = 0; i < len; ++i)
                {
                    int sx = spots[i, 0];
                    int sy = spots[i, 1];

                    Cell c = ExtGame.cells[ExtGame.grid[sx, sy]];

                    // if food, we are done
                    if (IsFood(c))
                    {
                        int[] p = v;
                        // traverse backwards
                        while (p[2] != x && p[3] != y)
                        {
                            // get the parent
                            p = lookup[p[2] + p[3] * ExtGame.width];
                        }

                        if (!Eat(ExtGame.cells[ExtGame.grid[p[0], p[1]]], p[0], p[1]))
                            Move(x, y, p[0], p[1]);
                        return true;
                    }

                    // don't add visited nodes
                    int s = sx + sy * ExtGame.width;
                    if (!lookup.ContainsKey(s) && c is EmptyCell) 
                    {
                        vertices.Enqueue(new int[4] { sx, sy, v[0], v[1] });
                        lookup.Add(s, new int[4] { sx, sy, v[0], v[1] });
                    }
                }
            }
            return true;
        }
        public bool Eat(Cell neighbor, int i, int j)
        {
            if (IsFood(neighbor) && hunger < info.sated)
            {
                ExtGame.RemoveCell(i, j);
                hunger += neighbor.Food();
                return true;
            }
            return false;
        }
        public abstract void Reproduce(int i, int j);
        public abstract bool IsFood(Cell c);
        //public abstract bool IsMate(Cell c);
    }
}
