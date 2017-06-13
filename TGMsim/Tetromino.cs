﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGMsim
{
    public class Tetromino
    {
        enum piece { I = 1, T, L, J, S, Z, O };

        public int id = 0;
        public int rotation = 0;
        public int x = 3;
        public int y = 3;
        public int kicked = 0;
        public bool bone = false;
        public bool big = false;
        public bool swapped = false;
        public bool floored = false;
        public int groundTimer = 0;

        public List<BlockBit> bits = new List<BlockBit>(); //first four will always be the "small" version

        public Tetromino(int i, bool nbig) : this(i, 0, 0, 0, nbig)
        {

        }

        public Tetromino(int i, int r, int nx, int ny, bool nbig)
        {
            id = i;
            rotation = r;
            big = nbig;
            switch (i)
            {
                case 1://I
                    switch (r)
                    {
                        case 0:
                        case 2:
                            bits.Add(new BlockBit(0, 1));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(3, 1));
                            break;
                        case 1:
                        case 3:
                            bits.Add(new BlockBit(2, 2));
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(2, 0));
                            bits.Add(new BlockBit(2, -1));
                            break;
                    }
                    break;
                case 2://T
                    switch (r)
                    {
                        case 0:
                            bits.Add(new BlockBit(0, 1));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(1, 0));
                            break;
                        case 1:
                            bits.Add(new BlockBit(1, 2));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(1, 0));
                            break;
                        case 2:
                            bits.Add(new BlockBit(0, 0));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(2, 0));
                            bits.Add(new BlockBit(1, 1));
                            break;
                        case 3:
                            bits.Add(new BlockBit(0, 1));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(1, 2));
                            bits.Add(new BlockBit(1, 0));
                            break;
                    }
                    break;
                case 3://L
                    switch (r)
                    {
                        case 0:
                            bits.Add(new BlockBit(0, 1));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(0, 0));
                            break;
                        case 1:
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(1, 2));
                            bits.Add(new BlockBit(2, 0));
                            break;
                        case 2:
                            bits.Add(new BlockBit(0, 0));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(2, 0));
                            bits.Add(new BlockBit(2, 1));
                            break;
                        case 3:
                            bits.Add(new BlockBit(1, 2));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(0, 2));
                            break;
                    }
                    break;
                case 4://J
                    switch (r)
                    {
                        case 0:
                            bits.Add(new BlockBit(0, 1));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(2, 0));
                            break;
                        case 1:
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(1, 2));
                            bits.Add(new BlockBit(2, 2));
                            break;
                        case 2:
                            bits.Add(new BlockBit(0, 0));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(2, 0));
                            bits.Add(new BlockBit(0, 1));
                            break;
                        case 3:
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(1, 2));
                            bits.Add(new BlockBit(0, 0));
                            break;
                    }
                    break;
                case 5://S
                    switch (r)
                    {
                        case 0:
                        case 2:
                            bits.Add(new BlockBit(0, 0));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(2, 1));
                            break;
                        case 1:
                        case 3:
                            bits.Add(new BlockBit(0, 1));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(0, 2));
                            break;
                    }
                    break;
                case 6://Z
                    switch (r)
                    {
                        case 0:
                        case 2:
                            bits.Add(new BlockBit(0, 1));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(2, 0));
                            break;
                        case 1:
                        case 3:
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(1, 0));
                            bits.Add(new BlockBit(2, 2));
                            break;
                    }
                    break;
                case 7://O
                    switch (r)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            bits.Add(new BlockBit(1, 1));
                            bits.Add(new BlockBit(2, 1));
                            bits.Add(new BlockBit(2, 0));
                            bits.Add(new BlockBit(1, 0));
                            break;
                    }
                    break;
            }

            int b = 2;
            if (this.big) b = 1;

            for (int p = 0; p < bits.Count; p++)
            {
                bits[p].x = this.x + ((bits[p].x * (2 / b)) - (1 % (3 - b)));
                bits[p].y = this.y - ((bits[p].y * (2 / b)) - (1 % (3 - b)));
            }

            move(nx, ny);
        }

        public void move(int mx, int my)
        {
            for(int i = 0; i < bits.Count; i++)
            {
                bits[i].move(mx, my);
            }
            this.x += mx;
            this.y += my;

        }

        public Tetromino clone()
        {
            Tetromino newTet = new Tetromino(id, this.rotation, this.x - 3, this.y - 3, this.big);
            newTet.bone = this.bone;
            newTet.kicked = kicked;
            newTet.groundTimer = groundTimer;
            return newTet;
        }
    }
}
