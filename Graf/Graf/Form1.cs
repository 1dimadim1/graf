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
        List<Edge> Edges = new List<Edge>();
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
            Edges.Add(new Edge(id,tappedID));
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
            int i = 0;
            foreach(var E in Edges)
            {
                gr.DrawString(Convert.ToString(E.id1+" "+E.id2), new Font("Arial", 10), new SolidBrush(Color.Black), i*25, 400);
                i++;
            }
        }

        private void ALLClean_Click(object sender, EventArgs e)
        {
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Clear(Color.Violet);
            list.Clear();
            Edges.Clear();
            tapped = false;
            tappedID = 0;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Algoritm();
            //foreach(var E in Edges)
            //{
            //    MessageBox.Show("Связь с вершиной номер " + E.id1 + " установлена c "+E.id2);
            //}
        }
        private void Algoritm()
        {
            List<Edge> CEdges = new List<Edge>();
            foreach(var E in Edges)
            {
                CEdges.Add(E);
            }
            if (CEdges.Count != 0)
            {
                Edge E1 = CEdges.First();//случайный

                while (CEdges.Find((Edge E) => { return (E.id1 != E1.id1 || E.id2 != E1.id2)&&(E.id2 != E1.id1 || E.id1 != E1.id2); }) != null)
                {                    
                    Merge(E1.id1, E1.id2, CEdges);
                    E1 = CEdges.First();//случайный из соседних с E1
                }
                Top F = list.FindLast(
                        delegate (Top Ver)
                        {
                            return Ver.id == E1.id2;
                        }
                        );
               ListBox.Items.Insert(0, "Найден минимальный разрез с ребром " + F.name + " это " + CEdges.Count + " рёбер");
            }
            else
            {
                MessageBox.Show("Answer is 0");
            }

        }
        private void Merge(int id1,int id2, List<Edge> CEdges)
        {
            foreach(var E in CEdges)
            {
                if (E.id1 != id1 && E.id2 == id2)
                {
                    E.id2 = id1;
                }
                else if (E.id1==id2)
                {
                    E.id1 = id1;
                }                
            }
            foreach (var E in CEdges)
            {
                if(E.id1==id1 && E.id2 == id2)
                {
                    CEdges.Remove(E);
                    break;
                }                
            }            
        }
    }

}
//foreach(var v in Connections)
//                {
//                    MessageBox.Show("Answer is "+v);
//                }
//                foreach (var v in V2.Connect)
//                {
//                    MessageBox.Show("Edge of v2: " + v);
//                }
//Merge
//var Connections = V1.Connect.Concat(V2.Connect);
//V1.Connect.Clear();
//            foreach (var T in Connections)
//            {
//              V1.Connect.Add(T);
//            }
//            foreach (var T in V1.Connect)
//            {
//                if (T == V1.id||T==V2.id)
//                {
//                    V1.Connect.Remove(T);
//                }
//            }