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

        int rad = 20;
        bool tapped = false;
        int tappedID = 0;
        bool Hover = false;
        int HoverID;

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {            
            if(!Checker.FreePlaceCreate(rad, e.Location.X, e.Location.Y, list)&& Move.Checked == true)
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
                        Rewrite();
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
            Edge Edge = new Edge(tappedID, id);
            Edges.Add(Edge);
            int counter = 0;
            foreach(var e in Edges)
            {
                if ((tappedID == e.id1 && id==e.id2)|| (tappedID == e.id2 && id == e.id1))
                {
                    counter++;
                }                
            }
            DrawLine(Edge, counter);
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
        public void DrawLine(Edge Edge,int x)
        {
            if (Edge.id1 != Edge.id2)
            {
                Top F = list.FindLast(
                         delegate (Top Ver)
                         {
                             return Ver.id == Edge.id1;
                         }
                         );
                Top S = list.FindLast(
                         delegate (Top Ver)
                         {
                             return Ver.id == Edge.id2;
                         }
                         );
            
                double xF = F.x;
                double yF = F.y;
                double xS = S.x;
                double yS = S.y;
                int distance = Convert.ToInt32(Math.Sqrt(Math.Pow(xF - xS, 2) + Math.Pow(yS - yF, 2)));
                Graphics gr = pictureBox1.CreateGraphics();
                gr.DrawLine(new Pen(Brushes.Black,x*2), Convert.ToInt32(xF - (((xF - xS) / distance) * rad)), Convert.ToInt32(yF - (((yF - yS) / distance) * rad)), Convert.ToInt32(xS - (((xS - xF) / distance) * rad)), Convert.ToInt32(yS - (((yS - yF) / distance) * rad)));
            }
        }
        public void Rewrite()
        {
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Clear(Color.Violet);
            foreach (var V in list)
            {
                DrawTop(V.x, V.y, V.name);
            }
            int i = 0;
            foreach (var E in Edges)
            {
                gr.DrawString(Convert.ToString(E.id1 + " " + E.id2), new Font("Arial", 10), new SolidBrush(Color.Black), i * 25, 450);
                i++;
                int counter = 0;
                foreach (var E1 in Edges)
                {
                    if((E.id1 == E1.id1 && E.id2 == E1.id2)|| (E.id2 == E1.id1 && E.id1 == E1.id2))
                    {
                        counter++;
                    }
                }
                DrawLine(E, counter);
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
        }
        private void Algoritm()
        {            
            List<Edge> CEdges = new List<Edge>();
            foreach(var E in Edges)
            {
                if (E.id1 != E.id2)
                {
                    CEdges.Add(E);
                }                
            }
            if (CEdges.Count != 0)
            {
                Random rnd = new Random();
                Edge E1;

                for(int i=0;i< list.Count-2;i++)//цикл n-2
                {
                    while (true)
                    {
                        int Const = rnd.Next(0, list.Count);
                        if (CEdges.Count > Const)
                        {
                            E1 = CEdges[Const];
                            break;
                        }
                    } //берём случайную вершину
                    Merge(E1.id1, E1.id2, CEdges);//объединяем вершины
                }

                if (CEdges.Count <= 1)
                {
                    ListBox.Items.Insert(0, "Найден минимальный разрез это " + CEdges.Count + " ребро");
                }//вывод результата
                else if (CEdges.Count <= 4)
                {
                    ListBox.Items.Insert(0, "Найден минимальный разрез это " + CEdges.Count + " ребра");
                }
                else
                {
                    ListBox.Items.Insert(0, "Найден минимальный разрез это " + CEdges.Count + " рёбрер");
                }

                Edges.Clear();
                foreach (var T in list)
                {
                    foreach (var Con in T.Connect)
                    {
                        if (Con > T.id)
                        {
                            Edges.Add(new Edge(T.id, Con));
                        }
                    }
                }//восстонавливаем Edges list
            }
            else
            {
                ListBox.Items.Insert(0, "Найден минимальный разрез это 0 рёбрер");
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
            for (int i = 0; i < CEdges.Count; i++)
            {
                if ((CEdges[i].id1 == id1 && CEdges[i].id2 == id2) || (CEdges[i].id1 == id2 && CEdges[i].id2 == id1) || (CEdges[i].id1 == CEdges[i].id2))
                {
                    CEdges.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}





































//Top F;
//                if (E1.id1 == Const)
//                {
//                    F = list.FindLast(
//                        delegate (Top Ver)
//                        {
//                            return Ver.id == E1.id2;
//                        }
//                        );
//                }
//                else
//                {
//                    F = list.FindLast(
//                        delegate (Top Ver)
//                        {
//                            return Ver.id == E1.id1;
//                        }
//                        );
//                }