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
            public int width { get; set; }
            public int height { get; set; }
            public int reproRate { get; set; }
            public int airRate { get; set; }
            public int food { get; set; }
            public int sated { get; set; } // hunger level at which they are full
            public int starved { get; set; } // hunger level at which they seek food
            public int lifeExpectancy { get; set; } // max age
            public int airCutoff { get; set; }
        }
        public Info info;
        protected int hunger;
        public int mated;
        public int age;
        public bool foodseeking = false;
        public AnimalCell()
            : base()
        {
            hunger = info.sated;
            mated = 0;
            age = 0;
        }
        public AnimalCell(Info i)
            : base()
        {
            info = i;
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
            if (hunger < info.starved)
                foodseeking = true;
            else if (hunger > info.sated)
                foodseeking = false;
            if (++age > info.lifeExpectancy)
            {
                int max = Math.Max(0, info.lifeExpectancy / 4 - (age - info.lifeExpectancy));
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
            }
            if (ExtGame.oxygen < info.airCutoff)
            {
                /*float deathChance =
                    (float)(info.airCutoff) /
                    (float)(ExtGame.oxygen);
                deathChance *= 100;
                if (r.Next(100) <= deathChance)*/
                if(r.Next(ExtGame.oxygen/(info.airCutoff/100)) == 0)
                {
                    ExtGame.RemoveCell(x, y);
                    return;
                }
            }
            if (SeekFood(x, y))
                updated = true;
            else if (SeekMate(x, y))
                updated = true;

            base.Update(gameTime, x, y);
        }
        public override bool DoStuff(int x, int y, int i, int j)
        {
            Cell neighbor = ExtGame.cells[ExtGame.grid[i, j]];
            if (neighbor is AnimalCell && mated > info.reproRate && ExtGame.oxygen > info.airCutoff)
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
            return Eat(i, j);
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

        public bool SeekFood(int x, int y)
        {
            if (!foodseeking || FoodCount() <= 0)
                return false;
            return Seek(x, y, new Predicate<Cell>(IsFood), Eat);
        }

        public bool SeekMate(int x, int y)
        {
            if ((age < 15 || age > 45) || mated < info.reproRate || ExtGame.oxygen < 50)
                return false;
            return Seek(x, y, new Predicate<Cell>(IsMate), Reproduce);
        }

        Queue<int[]> vertices = new Queue<int[]>();
        IDictionary<int, int[]> lookup = new Dictionary<int, int[]>();
        public delegate bool SeekHandler(int x, int y);
        public bool Seek(int x, int y, Predicate<Cell> pred, SeekHandler handler)
        {
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

                    // if pred, we are done
                    if (pred(c))
                    {
                        // if we are at the root
                        if (v[0] == x && v[1] == y)
                        {
                            handler(sx, sy);
                            return true;
                        }
                        int[] p = v;
                        // traverse backwards
                        while (p[2] != x || p[3] != y)
                        {
                            // get the parent
                            p = lookup[p[2] + p[3] * ExtGame.width];
                        }

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
        public bool Eat(int i, int j)
        {
            Cell neighbor = ExtGame.cells[ExtGame.grid[i, j]];
            if (IsFood(neighbor) && hunger < info.sated)
            {
                ExtGame.RemoveCell(i, j);
                hunger += neighbor.Food();
                return true;
            }
            return false;
        }
        public abstract bool Reproduce(int i, int j);
        public abstract bool IsFood(Cell c);
        public abstract bool IsMate(Cell c);
        public abstract int FoodCount();
    }
}
