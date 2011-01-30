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
            public string textureFile { get; set; }
            public int cost { get; set; }
        }
        public Info info;
        protected int hunger;
        public int mated;
        public int age;
        public bool foodseeking = false;
        public AnimalCell(Info i)
            : base()
        {
            info = i;
            Reset();
        }
        public void Reset()
        {
            hunger = (info.sated + info.starved) / 2;
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
                /*if(r.Next(ExtGame.oxygen/(info.airCutoff/100)) == 0)
                {
                    ExtGame.RemoveCell(x, y);
                    return;
                }*/
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
                return Reproduce(i, j);
            }
            if (ExtGame.grid[i, j] == 0)
            {
                Move(x, y, i, j);
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
            if (!foodseeking || !FoodCount())
                return false;
            return Seek(x, y, new Predicate<Cell>(IsFood), Eat);
        }

        public bool SeekMate(int x, int y)
        {
            if (age < 0.1*info.lifeExpectancy || mated < info.reproRate || ExtGame.oxygen < info.airCutoff || foodseeking)
                return false;
            return Seek(x, y, new Predicate<Cell>(IsMate), Reproduce);
        }

        //static Queue<int[]> vertices = new Queue<int[]>();
        static Queue vertices = new Queue();
        static IDictionary<int, bool> lookup = new Dictionary<int, bool>();
        static int[][] lookupData = new int[ExtGame.width * ExtGame.height][];
        static bool ldInit = false;
        static int[,] spots = new int[8, 2];
        public delegate bool SeekHandler(int x, int y);
        void LookupAdd(int x, int y, int px, int py)
        {
            int i = x + y * ExtGame.width;
            lookup[i] = true;
            lookupData[i][0] = x;
            lookupData[i][1] = y;
            lookupData[i][2] = px;
            lookupData[i][3] = py;
        }
        public bool Seek(int x, int y, Predicate<Cell> pred, SeekHandler handler)
        {
            if (!ldInit)
            {
                for (int i = 0; i < lookupData.Count(); ++i)
                    lookupData[i] = new int[4];
                ldInit = true;
            }
            lookup.Clear();
            vertices.Clear();
            int depth = 0;

            // vertex plus parent vertex
            vertices.Enqueue(x, y, x, y);
            LookupAdd(x, y, x, y);

            //while (vertices.Count() > 0)
            while (vertices.Count() > 0 && depth++ < 256)
            {
                int[] v = vertices.Dequeue();
                int len = Spots(v[0], v[1], spots);
                for (int i = 0; i < len; ++i)
                {
                    int sx = spots[i, 0];
                    int sy = spots[i, 1];

                    Cell c = ExtGame.cells[ExtGame.grid[sx, sy]];

                    // if pred, we are done
                    if (pred(c) && (sx != x || sy != y))
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
                            p = lookupData[p[2] + p[3] * ExtGame.width];
                        }

                        Move(x, y, p[0], p[1]);
                        return true;
                    }

                    // don't add visited nodes
                    int s = sx + sy * ExtGame.width;
                    if (!lookup.ContainsKey(s) && c is EmptyCell) 
                    {
                        vertices.Enqueue(sx, sy, v[0], v[1]);
                        LookupAdd(sx, sy, v[0], v[1]);
                    }
                }
            }
            return false;
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
        public bool Reproduce(int i, int j)
        {
            if (hunger < info.starved || ExtGame.oxygen < info.airCutoff || age < 0.1*info.lifeExpectancy)
                return false;

            int[,] spots = new int[8, 2];
            int len = Spots(i, j, spots);
            for (int k = 0; k < len; ++k)
            {
                int x = spots[k, 0];
                int y = spots[k, 1];
                if (ExtGame.grid[x, y] == 0)
                {
                    Birth(x, y);
                    mated = 0;
                    return true;
                }
            }
            return false;
        }
        public abstract void Birth(int x, int y);
        public abstract bool IsFood(Cell c);
        public abstract bool IsMate(Cell c);
        public abstract bool FoodCount();
    }
}
