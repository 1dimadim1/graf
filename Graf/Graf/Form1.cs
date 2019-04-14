using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicGraf;
using System.Diagnostics;

namespace Graf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();            
        }

        List<Top> list = new List<Top>();
        int rad = 30;
        bool tapped = false;
        int tappedID = 0;
        bool Hover = false;
        int HoverID;

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {            
            if(!Checker.FreePlaceCreate(rad, e.Location.X, e.Location.Y, list))
            {
                HoverID = Checker.WhatTop(rad, e.Location.X, e.Location.Y, list);
                Hover = true;
            }
        }
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Hover)
            {
                Top V = list.FindLast(
                         delegate (Top Ver)
                         {
                             return Ver.id == HoverID;
                         }
                         );
                if (V != null)
                {
                    V.x = e.Location.X;
                    V.y = e.Location.Y;
                    Rewrite();
                }
            }
        }
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Hover = false;
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {            
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            if (CreateTop.Checked == true)
            {
                if (Checker.FreePlaceCreate(rad, coordinates.X, coordinates.Y, list))
                {                    
                     NewTop(coordinates.X, coordinates.Y);
                }
                else
                    MessageBox.Show("НИЗЯ так ставить вершины");
            }
            else if(CreateEdge.Checked == true)
            {
                if (!Checker.FreePlace(rad, coordinates.X, coordinates.Y, list))
                {
                    if (!tapped)
                    {
                        Tap(Checker.WhatTop(rad, coordinates.X, coordinates.Y, list));                        
                        tapped = true;
                    }
                    else
                    {
                        EnotherTap(Checker.WhatTop(rad, coordinates.X, coordinates.Y, list));
                        tapped = false;
                        //MessageBox.Show("Связь с вершиной номер " + tappedID + " установлена");
                    }
                }                
            }
            else
            {
            }
        }

        public void Tap(int id)
        {
            tappedID = id;
        }
        public void EnotherTap(int id)
        {
            Top V=list.FindLast(
                         delegate (Top Ver)
                         {
                           return Ver.id == id;
                         }
                         );
            V.Connect.Add(tappedID);
            V = list.FindLast(
                         delegate (Top Ver)
                         {
                             return Ver.id == tappedID;
                         }
                         );
            V.Connect.Add(id);
            DrawLine(tappedID,id);
        }
        public void NewTop(int x, int y)
        {
            IDGen iDGen = new IDGen();
            int ID = iDGen.NextId(list);
            char name = iDGen.NextIdl(list);
            list.Add(new Top(ID, name, x,y,new List<int>()));
            DrawTop(x, y,name);
        }
        public void DrawTop(int x, int y,char name)
        {
            int font = rad;
            Graphics gr = pictureBox1.CreateGraphics();
            gr.DrawEllipse(new Pen(Brushes.Black), x-rad, y-rad,rad*2,rad*2);
            Top a = list.Last();
            gr.DrawString(Convert.ToString(name),new Font("Arial", font), new SolidBrush(Color.Black), x-rad+font/2, y-rad+font/3);
        }
        public void DrawLine(int FID, int SID)
        {
            if (SID != FID)
            {
                Top F = list.FindLast(
                         delegate (Top Ver)
                         {
                             return Ver.id == FID;
                         }
                         );
            Top S = list.FindLast(
                         delegate (Top Ver)
                         {
                             return Ver.id == SID;
                         }
                         );
            
                double xF = F.x;
                double yF = F.y;
                double xS = S.x;
                double yS = S.y;
                int distance = Convert.ToInt32(Math.Sqrt(Math.Pow(xF - xS, 2) + Math.Pow(yS - yF, 2)));
                Graphics gr = pictureBox1.CreateGraphics();
                gr.DrawLine(new Pen(Brushes.Black), Convert.ToInt32(xF - (((xF - xS) / distance) * rad)), Convert.ToInt32(yF - (((yF - yS) / distance) * rad)), Convert.ToInt32(xS - (((xS - xF) / distance) * rad)), Convert.ToInt32(yS - (((yS - yF) / distance) * rad)));
            }
        }
        public void Rewrite()
        {
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Clear(Color.Violet);
            foreach (var V in list)
            {
                DrawTop(V.x, V.y,V.name);
                if (V.Connect != null)
                {
                    foreach(int con in V.Connect)
                    {
                        DrawLine(V.id, con);
                    }
                }
            }
        }

        private void ALLClean_Click(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Wheat;
        }
    }
    
}
