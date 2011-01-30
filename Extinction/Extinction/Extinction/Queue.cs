using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extinction
{
    class Queue
    {
        public int beg, end;
        int[][] data;
        public Queue()
        {
            data = new int[ExtGame.width * ExtGame.height][];
            for (int i = 0; i < data.Count(); ++i)
            {
                data[i] = new int[4];
                beg = 0;
                end = 0;
            }
        }

        public void Enqueue(int a, int b, int c, int d)
        {
            data[end][0] = a;
            data[end][1] = b;
            data[end][2] = c;
            data[end][3] = d;
            ++end;
        }
        public int[] Dequeue()
        {
            return data[beg++];
        }
        public int Count()
        {
            return end - beg;
        }
        public void Clear()
        {
            beg = end = 0;
        }
    }

    public class CellStack
    {
        public int beg;
        Cell[] data;
        public CellStack()
        {
            data = new Cell[ExtGame.width * ExtGame.height];
            Clear();
        }

        public void Push(Cell c)
        {
            data[beg++] = c;
        }
        public void Push()
        {
            ++beg;
        }
        public Cell Pop()
        {
            return data[--beg];
        }
        public int Count()
        {
            return beg;
        }
        public void Clear()
        {
            beg = 0;
        }
    }
}
