using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace PasteLCM
{
    /// <summary>
    /// 该类寻找边缘，并拟合直线，返回两条直线，四个点坐标
    /// </summary>
    class FitLine
    {
        public HRegion Region1 { get; set; }//1#ROI
        public HRegion Region2 { get; set; }//2#ROI
        public int LowThreshold { get; set; }//低阈值
        public int HighThreshold { get; set; }//高阈值
        public HImage Ho_image { get; set; }//传入图片
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ho_image">传入图片</param>
        /// <param name="region1">1#ROI</param>
        /// <param name="region2">2#ROI</param>
        /// <param name="lowThreshold">低阈值</param>
        /// <param name="highThreshold">高阈值</param>
        public FitLine(HImage ho_image,HRegion region1,HRegion region2,int lowThreshold,int highThreshold) 
        {
            this.Ho_image = ho_image;
            this.Region1 = region1;
            this.Region2 = region2;
            this.LowThreshold = lowThreshold;
            this.HighThreshold = highThreshold;
        }
        /// <summary>
        /// 图像处理
        /// </summary>
        /// <returns>返回8个点位数组</returns>
        public double[] Process() 
        {
            double[]coorderate=new double[8];
            double hv_RowBegin1; double hv_ColBegin1; double hv_RowEnd1; double hv_ColEnd1; double hv_Nr1; double hv_Nc1; double hv_Dist1;
            double hv_RowBegin2; double hv_ColBegin2; double hv_RowEnd2; double hv_ColEnd2; double hv_Nr2; double hv_Nc2; double hv_Dist2;
            //图像阈值化，获得region
            HRegion hRegion = Ho_image.Threshold((double)LowThreshold,(double)HighThreshold);
            //region转化为himage
            HImage hImage = hRegion.RegionToBin(255, 0, 512, 512);
            //减去ROI以外多余部分
            HImage image1 = hImage.ReduceDomain(Region1);
            HImage image2 = hImage.ReduceDomain(Region2);
            //生成边缘轮廓
            HXLDCont edges1 = image1.EdgesSubPix("canny", 1, 5, 5);
            HXLDCont edges2 = image2.EdgesSubPix("canny", 1, 5, 5);
            //分割轮廓
            HXLDCont xldContsplit1 = edges1.SegmentContoursXld("lines_circles", 5, 4, 2);
            HXLDCont xldContsplit2 = edges2.SegmentContoursXld("lines_circles", 5, 4, 2);
            //根据特征筛选轮廓
            HXLDCont xldCont1 = xldContsplit1.SelectContoursXld("contour_length", 100, 99999999, -0.5, 0.5);
            HXLDCont xldCont2 = xldContsplit2.SelectContoursXld("contour_length", 100, 99999999, -0.5, 0.5);
            //拟合直线
            xldCont1.FitLineContourXld("tukey", -1, 0, 5, 2, out hv_RowBegin1, out hv_ColBegin1, out hv_RowEnd1, out hv_ColEnd1, out hv_Nr1, out hv_Nc1, out hv_Dist1);
            xldCont2.FitLineContourXld("tukey", -1, 0, 5, 2, out hv_RowBegin2, out hv_ColBegin2, out hv_RowEnd2, out hv_ColEnd2, out hv_Nr2, out hv_Nc2, out hv_Dist2);
            coorderate[0] = hv_RowBegin1;
            coorderate[1] = hv_ColBegin1;
            coorderate[2] = hv_RowEnd1;
            coorderate[3] = hv_ColEnd1;
            coorderate[4] = hv_RowBegin2;
            coorderate[5] = hv_ColBegin2;
            coorderate[6] = hv_RowEnd2;
            coorderate[7] = hv_ColEnd2;
            return coorderate;
        }
    }
}
