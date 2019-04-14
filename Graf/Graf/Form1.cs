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
        private void Form1_Load(object sender, EventArgs e)
        {
          
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
                        MessageBox.Show("Соедини меня(я вершина номер " + Checker.WhatTop(rad, coordinates.X, coordinates.Y, list) + ")");
                        tapped = true;
                    }
                    else
                    {
                        EnotherTap(Checker.WhatTop(rad, coordinates.X, coordinates.Y, list));
                        tapped = false;
                    }
                }                
            }
            else
            {
                foreach (Top V in list)
                {
                    MessageBox.Show(Convert.ToString(V.x)+ " " + Convert.ToString(V.y) + " " + Convert.ToString(V.id) + " " + Convert.ToString(V.name));
                }
                pictureBox1.BackColor = Color.Honeydew;
            }
        }

        public void Tap(int id)
        {

        }
        public void EnotherTap(int id)
        {

        }
        public void NewTop(int x, int y)
        {
            IDGen iDGen = new IDGen();           
            list.Add(new Top(iDGen.NextId(list), iDGen.NextIdl(list), x,y));
            DrawTop(x, y);
        }
        public void DrawTop(int x, int y)
        {
            int font = rad;
            Graphics gr = pictureBox1.CreateGraphics();
            gr.DrawEllipse(new Pen(Brushes.Black), x-rad, y-rad,rad*2,rad*2);
            Top a = list.Last();
            gr.DrawString(Convert.ToString(a.name),new Font("Arial", font), new SolidBrush(Color.Black), x-rad+font/2, y-rad+font/3);
        }
    }
    
}
