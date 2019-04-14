using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicGraf
{
    public class Top
    {
        public Top(int id,char name, int x, int y, List<int> Connect)
        {
            this.id = id;
            this.name = name;
            this.x = x;
            this.y = y;
            this.Connect = Connect;
        }

        public int id { get; set; }
        public char name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public List<int> Connect { get; set; }
    }
    public class IDGen
    {
        char[] letters = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
        public char NextIdl(List<Top> list)
        {
            if (list.Count>0)
            {
                Top a = list.Last();
                lastId =1+ a.id;
            }
            return letters[lastId];
        }
        public int NextId(List<Top> list)
        {
            if (list.Count > 0)
            {
                Top a = list.Last();
                lastId = 1+a.id;
            }
            return lastId;
        }

        private int lastId;
    }
    public class Checker
    {
        public static bool FreePlaceCreate(int rad, int x, int y, List<Top> list)
        {
            foreach(Top V in list)
            {
                if ((Math.Pow(x - V.x, 2) + Math.Pow(y - V.y,2)) < Math.Pow(2*rad,2))
                return false;
            }
            return true;
        }
        public static bool FreePlace(int rad, int x, int y, List<Top> list)
        {
            foreach (Top V in list)
            {
                if ((Math.Pow(x - V.x, 2) + Math.Pow(y - V.y, 2)) < Math.Pow(rad, 2))
                    return false;
            }
            return true;
        }
        public static int WhatTop(int rad, int x, int y, List<Top> list)
        {
            foreach (Top V in list)
            {
                if ((Math.Pow(x - V.x, 2) + Math.Pow(y - V.y, 2)) < Math.Pow(rad, 2))
                    return V.id;
            }
            return -1;
        }
    }
    public class Connection
    {
        public delegate void Connect();
        public event Connect tapped;
        public event Connect tapping;
    }
}
