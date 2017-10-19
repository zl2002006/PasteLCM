using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HalconDotNet;

namespace PasteLCM
{

    /// <summary>
    /// 灰度值调整小助手
    /// </summary>
    public partial class Adjust : Form
    {
        public HObject Ho_img { get; set; }
        //窗体间传值
        private FrmMain Fm;
        [DllImport("kernel32", EntryPoint = "RtlMoveMemory", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern void CopyMemory(int Destination, int Source, int length);
        public Adjust(HObject ho_img,FrmMain fm)
        {
            InitializeComponent();
            this.Ho_img = ho_img;
            Fm = fm;
        }

        private void @try_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            trackBar1.Enabled = false;
            trackBar2.Enabled = false;
            pictureBox1.Image = H2Bitmap(HobjectToHimage(this.Ho_img));
        }
        HObject hregion;
        void Threshold(int a,int b)
        {
            //阈值化，输出region.
            HOperatorSet.Threshold(this.Ho_img, out hregion, a, b);
            //region转换为image.
            HOperatorSet.RegionToBin(hregion, out hregion, 255, 0, 512, 512);
        }
        #region Himage转Bitmap
        public Bitmap H2Bitmap(HImage img1)
        {
            IntPtr pt;
            int mwidth, mheight;
            string mtype = "";
            Bitmap img;
            ColorPalette pal;
            int i;
            const int Alpha = 255;
            BitmapData bitmapData;
            Rectangle rect;
            int[] ptr = new int[2];
            int PixelSize;
            //this.Ho_image.ReadImage("clip");
            pt = img1.GetImagePointer1(out mtype, out mwidth, out mheight);
            img = new Bitmap(mwidth, mheight, PixelFormat.Format8bppIndexed);
            pal = img.Palette;
            for (i = 0; i <= 255; i++)
            {
                pal.Entries[i] = Color.FromArgb(Alpha, i, i, i);
            }
            img.Palette = pal;
            rect = new Rectangle(0, 0, mwidth, mheight);
            bitmapData = img.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            ptr[0] = bitmapData.Scan0.ToInt32();
            ptr[1] = pt.ToInt32();
            if (mwidth % 4 == 0)
                CopyMemory(ptr[0], ptr[1], mwidth * mheight * PixelSize);
            else
            {
                for (i = 0; i < mheight; i++)
                {
                    ptr[1] += mwidth;
                    CopyMemory(ptr[0], ptr[1], mwidth * PixelSize);
                    ptr[0] += bitmapData.Stride;
                }
            }
            img.UnlockBits(bitmapData);
            img1.Dispose();
            return img;
        }
        #endregion
        /// <summary>
        /// hobject转himage
        /// </summary>
        /// <param name="hobject">hobject</param>
        /// <returns>himage</returns>
        private HImage HobjectToHimage(HObject hobject)
        {
            HImage image = new HImage();
            HTuple pointer, type, width, height;
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
            return image;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                trackBar1.Enabled = true;
                trackBar2.Enabled = true; 
                Threshold(trackBar1.Value,trackBar2.Value);
                Bitmap image = H2Bitmap(HobjectToHimage(hregion));
                pictureBox1.Image = image;
            }
            else 
            {
                trackBar1.Enabled = false;
                trackBar2.Enabled = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackBar1.Maximum = trackBar2.Value;
            label3.Text = trackBar1.Value.ToString();
            Threshold(trackBar1.Value, trackBar2.Value);
            Bitmap image = H2Bitmap(HobjectToHimage(hregion));
            pictureBox1.Image = image;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            trackBar2.Minimum = trackBar1.Value;
            label4.Text = trackBar2.Value.ToString();
            Threshold(trackBar1.Value, trackBar2.Value);
            Bitmap image = H2Bitmap(HobjectToHimage(hregion));
            pictureBox1.Image = image;
        }

        private void Adjust_FormClosed(object sender, FormClosedEventArgs e)
        {
            Fm.textBox4.Text = this.trackBar1.Value.ToString();
            Fm.textBox3.Text = this.trackBar2.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
