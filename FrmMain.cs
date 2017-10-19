using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.Threading;

namespace PasteLCM
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            { Application.Exit(); }
        }
        ImageGrab ig = null;
        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw|ControlStyles.AllPaintingInWmPaint,true);
            ig = new ImageGrab("DirectShow", hWindowControl1.HalconWindow, 100, 300, 200, 500, 300, 100, 400, 250, 148.4, 399.9, 149.8, 300.3, 250.0, 229.5, 399.9, 218.5, double.Parse(textBox1.Text), double.Parse(textBox2.Text),0);
            ig.OpenFramegrabber();
            ig.ShowImg();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ig.Row11 += 1;
        }
        private HImage HobjectToHimage(HObject hobject)
        {
            HImage image = new HImage();
            HTuple pointer, type, width, height;
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
            return image;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HImage hImage = HobjectToHimage(ig.ho_img1);
            HRegion region1 = new HRegion(ig.Row11, ig.Column11, ig.Row12, ig.Column12);
            HRegion region2 = new HRegion(ig.Row21, ig.Column21, ig.Row22, ig.Column22);
            try
            {
                FitLine ft = new FitLine(hImage, region1, region2, int.Parse(textBox4.Text), int.Parse(textBox3.Text));
                double[] lines = ft.Process();
                ig.LineRowBegin1 = lines[0];
                ig.LineColBegin1 = lines[1];
                ig.LineRowEnd1 = lines[2];
                ig.LineColEnd1 = lines[3];
                ig.LineRowBegin2 = lines[4];
                ig.LineColBegin2 = lines[5];
                ig.LineRowEnd2 = lines[6];
                ig.LineColEnd2 = lines[7];
            }
            catch { }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ig.SendImg = 10;
            Adjust try1 = new Adjust(ig.ho_img1, this);
            ig.SendImg = 0;
            try1.ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ig.Distance2 = double.Parse(textBox2.Text);
        }

        private void textBox1_TextAlignChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            ig.Distance1 = double.Parse(textBox1.Text);

        }

        private void hWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (distToClose <= 5) 
            {
                ig.moveByHandle(e.X, e.Y);
            }
        }
        double distToClose;
        private void hWindowControl1_HMouseDown(object sender, HMouseEventArgs e)
        {
            distToClose = ig.distToClosestHandle(e.X, e.Y);
        }

        private void hWindowControl1_HMouseUp(object sender, HMouseEventArgs e)
        {
            distToClose = 10;
        }

    }
}
