using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Threading;

namespace PasteLCM
{
    class ImageGrab
    {
        public double Row11 { get; set; }
        public double Column11 { get; set; }
        public double Row12 { get; set; }
        public double Column12 { get; set; }
        public double Row21 { get; set; }
        public double Column21 { get; set; }
        public double Row22 { get; set; }
        public double Column22 { get; set; }
        public string CCD { get; set; }
        public HWindow Window { get; set; }
        public double LineRowBegin1 { get; set; }
        public double LineColBegin1 { get; set; }
        public double LineRowEnd1 { get; set; }
        public double LineColEnd1 { get; set; }
        public double LineRowBegin2 { get; set; }
        public double LineColBegin2 { get; set; }
        public double LineRowEnd2 { get; set; }
        public double LineColEnd2 { get; set; }
        public double Distance1 { get; set; }
        public double Distance2 { get; set; }
        public int SendImg { get; set; }
        private HTuple acqHandle;
        public ImageGrab(string ccd, HWindow window, HTuple row11, HTuple column11, HTuple row12, HTuple column12, HTuple row21, HTuple column21, HTuple row22, HTuple column22, HTuple lineRowBegin1, HTuple lineColBegin1, HTuple lineRowEnd1, HTuple lineColEnd1, HTuple lineRowBegin2, HTuple lineColBegin2, HTuple lineRowEnd2, HTuple lineColEnd2, HTuple distance1, HTuple distance2,int sendImg)
        {
            this.CCD = ccd;
            this.Window = window;
            this.Row11 = row11;
            this.Column11 = column11;
            this.Row12 = row12;
            this.Column12 = column12;
            this.Row21 = row21;
            this.Column21 = column21;
            this.Row22 = row22;
            this.Column22 = column22;
            this.LineRowBegin1 = lineRowBegin1;
            this.LineColBegin1 = lineColBegin1;
            this.LineRowEnd1 = lineRowEnd1;
            this.LineColEnd1 = lineColEnd1;
            this.LineRowBegin2 = lineRowBegin2;
            this.LineColBegin2 = lineColBegin2;
            this.LineRowEnd2 = lineRowEnd2;
            this.LineColEnd2 = lineColEnd2;
            this.Distance1 = distance1;
            this.Distance2 = distance2;
            this.SendImg = sendImg;
        }
        public void OpenFramegrabber()
        {
            HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb",
        -1, "false", "default", "0", -1, -1, out acqHandle);
            HOperatorSet.GrabImageStart(acqHandle, -1);
        }
        public HObject ho_img;
        //public bool sendImg = false;
        public HObject ho_img1;
        public void ShowImg()
        {
            Thread th = new Thread(Show);
            th.IsBackground = true;
            th.Start();
        }
       /* public void SengImg() 
        {
            while(SendImg) 
            {
                ho_img1 = ho_img;
                this.SendImg = false;
            }
        }*/
        public void Show()
        {
            while (true)
            {
                if (this.SendImg>=5) 
                {
                    ho_img1 = ho_img;
                    //break;
                }
                if (ho_img != null)
                {
                    ho_img.Dispose();
                }
                HOperatorSet.GrabImageAsync(out ho_img, acqHandle, -1);
                HOperatorSet.DispImage(ho_img, Window);
                
                Window.SetLineWidth(2);
                Window.SetDraw("margin");
                Window.SetColor("green");
                ShowRectancleandLine();
                ExtendAndDoubleLine();
                displayActive();
            }
        }
        public void ShowRectancleandLine()
        {
            Window.DispRectangle1(Row11, Column11, Row12, Column12);
            Window.DispRectangle2(Row11 / 2 + Row12 / 2, Column12 / 2 + Column11 / 2, 0, 5, 5);
            Window.DispRectangle1(Row21, Column21, Row22, Column22);
            Window.DispRectangle2(Row21 / 2 + Row22 / 2, Column22 / 2 + Column21 / 2, 0, 5, 5);
        }
        /// <summary>
        /// 延伸和复制线
        /// </summary>
        public void ExtendAndDoubleLine()
        {
            HTuple phi1, phi2;
            HOperatorSet.LineOrientation(LineRowBegin1, LineColBegin1, LineRowEnd1, LineColEnd1, out phi1);
            HOperatorSet.LineOrientation(LineRowBegin2, LineColBegin2, LineRowEnd2, LineColEnd2, out phi2);
            Window.DispLine(LineRowBegin1 - 10000 * Math.Sin(phi1), LineColBegin1 + 10000 * Math.Cos(phi1), LineRowEnd1 + 10000 * Math.Sin(phi1), LineColEnd1 - 10000 * Math.Cos(phi1));
            Window.DispLine(LineRowBegin2 - 10000 * Math.Sin(phi2), LineColBegin2 + 10000 * Math.Cos(phi2), LineRowEnd2 + 10000 * Math.Sin(phi2), LineColEnd2 - 10000 * Math.Cos(phi2));
            Window.SetColor("red");
            if ((LineRowBegin1 / 2 + LineRowEnd1 / 2) < (LineRowBegin2 / 2 + LineRowEnd2 / 2))
            {
                //Window.ReadString("wwww", 32);
                //Window.ClearWindow();
                
                Window.DispLine(LineRowBegin1 - 10000 * Math.Sin(phi1) + Distance2 / Math.Cos(phi1), LineColBegin1 + 10000 * Math.Cos(phi1), LineRowEnd1 + 10000 * Math.Sin(phi1) + Distance2 / Math.Cos(phi1), LineColEnd1 - 10000 * Math.Cos(phi1));
                Window.DispLine(LineRowBegin2 - 10000 * Math.Sin(phi2), LineColBegin2 + 10000 * Math.Cos(phi2) + Math.Abs(Distance1 / Math.Sin(phi2)), LineRowEnd2 + 10000 * Math.Sin(phi2), LineColEnd2 - 10000 * Math.Cos(phi2) + Math.Abs(Distance1 / Math.Sin(phi2)));
            }
            else
            {
                Window.DispLine(LineRowBegin1 - 10000 * Math.Sin(phi1), LineColBegin1 + 10000 * Math.Cos(phi1) + Math.Abs(Distance1 / Math.Sin(phi1)), LineRowEnd1 + 10000 * Math.Sin(phi1), LineColEnd1 - 10000 * Math.Cos(phi1) + Math.Abs(Distance1 / Math.Sin(phi1)));
                Window.DispLine(LineRowBegin2 - 10000 * Math.Sin(phi2) + Distance2 / Math.Cos(phi2), LineColBegin2 + 10000 * Math.Cos(phi2), LineRowEnd2 + 10000 * Math.Sin(phi2) + Distance2 / Math.Cos(phi2), LineColEnd2 - 10000 * Math.Cos(phi2));
            }
        }
        public void CloseFramegrabber()
        {
            HOperatorSet.CloseFramegrabber(acqHandle);
            ho_img.Dispose();
        }
        int NumHandles = 10;
        int activeHandleIdx = 10;
        /// <summary>
        /// 计算鼠标到点的距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double distToClosestHandle(double x, double y)
        {
            double max = 10000;
            double[] val = new double[NumHandles];
            val[0] = HMisc.DistancePp(y, x, Row11, Column11); // 1upper left 
            val[1] = HMisc.DistancePp(y, x, Row11, Column12); // 1upper right 
            val[2] = HMisc.DistancePp(y, x, Row12, Column12); // 1lower right 
            val[3] = HMisc.DistancePp(y, x, Row12, Column11); // 1lower left 
            val[4] = HMisc.DistancePp(y, x, Row11 / 2 + Row12 / 2, Column11 / 2 + Column12 / 2); // 1midpoint
            val[5] = HMisc.DistancePp(y, x, Row21, Column21); // 2upper left 
            val[6] = HMisc.DistancePp(y, x, Row21, Column22); // 2upper right 
            val[7] = HMisc.DistancePp(y, x, Row22, Column22); // 2lower right 
            val[8] = HMisc.DistancePp(y, x, Row22, Column21); // 2lower left 
            val[9] = HMisc.DistancePp(y, x, Row21 / 2 + Row22 / 2, Column21 / 2 + Column22 / 2); // 2midpoint 
            for (int i = 0; i < NumHandles; i++)
            {
                if (val[i] < max)
                {
                    max = val[i];
                    activeHandleIdx = i;
                }
            }// end of for 

            return val[activeHandleIdx];
        }
        /// <summary>
        /// 显示激活点
        /// </summary>
        public void displayActive()
        {
            switch (activeHandleIdx)
            {
                case 0:
                    Window.DispRectangle2(Row11, Column11, 0, 5, 5);
                    break;
                case 1:
                    Window.DispRectangle2(Row11, Column12, 0, 5, 5);
                    break;
                case 2:
                    Window.DispRectangle2(Row12, Column12, 0, 5, 5);
                    break;
                case 3:
                    Window.DispRectangle2(Row12, Column11, 0, 5, 5);
                    break;
                case 4:
                    Window.DispRectangle2(Row11 / 2 + Row12 / 2, Column11 / 2 + Column12 / 2, 0, 5, 5);
                    break;
                case 5:
                    Window.DispRectangle2(Row21, Column21, 0, 5, 5);
                    break;
                case 6:
                    Window.DispRectangle2(Row21, Column22, 0, 5, 5);
                    break;
                case 7:
                    Window.DispRectangle2(Row22, Column22, 0, 5, 5);
                    break;
                case 8:
                    Window.DispRectangle2(Row22, Column21, 0, 5, 5);
                    break;
                case 9:
                    Window.DispRectangle2(Row21 / 2 + Row22 / 2, Column21 / 2 + Column22 / 2, 0, 5, 5);
                    break;
            }
        }
        /// <summary>
        /// 用鼠标拖动
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void moveByHandle(double newX, double newY)
        {
            double len1, len2;
            double tmp;
            switch (activeHandleIdx)
            {
                case 0:
                    Row11 = newY;
                    Column11 = newX;
                    break;
                case 1:
                    Row11 = newY;
                    Column12 = newX;
                    break;
                case 2:
                    Row12 = newY;
                    Column12 = newX;
                    break;
                case 3:
                    Row12 = newY;
                    Column11 = newX;
                    break;
                case 4:
                    len1 = Row12 / 2 - Row11 / 2;
                    len2 = Column12 / 2 - Column11 / 2;
                    Row11 = newY - len1;
                    Column11 = newX - len2;
                    Row12 = newY + len1;
                    Column12 = newX + len2;
                    break;
                case 5:
                    Row21 = newY;
                    Column21 = newX;
                    break;
                case 6:
                    Row21 = newY;
                    Column22 = newX;
                    break;
                case 7:
                    Row22 = newY;
                    Column22 = newX;
                    break;
                case 8:
                    Row22 = newY;
                    Column21 = newX;
                    break;
                case 9:
                    len1 = Row22 / 2 - Row21 / 2;
                    len2 = Column22 / 2 - Column21 / 2;
                    Row21 = newY - len1;
                    Column21 = newX - len2;
                    Row22 = newY + len1;
                    Column22 = newX + len2;
                    break;
            }
            if (Row12 <= Row11)
            {
                tmp = Row11;
                Row11 = Row12;
                Row12 = tmp;
            }
            if (Column12 <= Column11)
            {
                tmp = Column11;
                Column11 = Column12;
                Column12 = tmp;
            }
            if (Row22 <= Row21)
            {
                tmp = Row21;
                Row21 = Row22;
                Row22 = tmp;
            }
            if (Column22 <= Column21)
            {
                tmp = Column21;
                Column21 = Column22;
                Column22 = tmp;
            }
        }
    }
}
