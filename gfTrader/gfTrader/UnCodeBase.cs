using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
namespace GFTrader
{
    /// <summary>
    /// 用于验证码图片识别的类
    /// GrayByPixels(); //灰度处理
    /// GetPicValidByValue(128, 4); //得到有效空间
    /// Bitmap[] pics = GetSplitPics(4, 1);     //分割
    /// string code = GetSingleBmpCode(pics[i], 128);   //得到代码串
    /// </summary>
    public class UnCodebase
    {
        public Bitmap bmpobj;

        public UnCodebase(Bitmap pic)
        {
            bmpobj = new Bitmap(pic);    //转换为Format32bppRgb
        }

        public UnCodebase(string filename)
        {
            if (File.Exists(filename))
            {
                this.bmpobj = (Bitmap)Image.FromFile(filename);
            }
        }
        /////获取主颜色
        //private System.Drawing.Color GetMainColor()
        //{
        //    for (int i = 0; i < bmpobj.Height; i++)
        //    {
        //        for (int j = 0; j < bmpobj.Width; j++)
        //        {
        //            System.Drawing.Color currentColor = bmpobj.GetPixel(i, j);

        //        }
        //    }
        //}
        /// <summary>
        /// 根据RGB，计算灰度值
        /// </summary>
        /// <param name="posClr">Color值</param>
        /// <returns>灰度值，整型</returns>
        private int GetGrayNumColor(System.Drawing.Color posClr)
        {
            return (posClr.R * 19595 + posClr.G * 38469 + posClr.B * 7472) >> 16;
        }

        /// <summary>
        /// 灰度转换,逐点方式
        /// </summary>
        public void GrayByPixels()
        {
            for (int i = 0; i < bmpobj.Height; i++)
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    int tmpValue = GetGrayNumColor(bmpobj.GetPixel(j, i));
                    bmpobj.SetPixel(j, i, Color.FromArgb(tmpValue, tmpValue, tmpValue));
                }
            }
        }

        /// <summary>
        /// 去图形边框
        /// </summary>
        /// <param name="borderWidth"></param>
        public void ClearPicBorder(int borderWidth)
        {
            for (int i = 0; i < bmpobj.Height; i++)
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    if (i < borderWidth || j < borderWidth || j > bmpobj.Width - 1 - borderWidth || i > bmpobj.Height - 1 - borderWidth)
                        bmpobj.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
        }

        /// <summary>
        /// 灰度转换,逐行方式
        /// </summary>
        public void GrayByLine()
        {
            Rectangle rec = new Rectangle(0, 0, bmpobj.Width, bmpobj.Height);
            BitmapData bmpData = bmpobj.LockBits(rec, ImageLockMode.ReadWrite, bmpobj.PixelFormat);// PixelFormat.Format32bppPArgb);
            //    bmpData.PixelFormat = PixelFormat.Format24bppRgb;
            IntPtr scan0 = bmpData.Scan0;
            int len = bmpobj.Width * bmpobj.Height;
            int[] pixels = new int[len];
            Marshal.Copy(scan0, pixels, 0, len);

            //对图片进行处理
            int GrayValue = 0;
            for (int i = 0; i < len; i++)
            {
                GrayValue = GetGrayNumColor(Color.FromArgb(pixels[i]));
                pixels[i] = (byte)(Color.FromArgb(GrayValue, GrayValue, GrayValue)).ToArgb();      //Color转byte
            }

            bmpobj.UnlockBits(bmpData);
        }
        /// <summary>
        /// 得到灰度图像前景背景的临界值 最大类间方差法，yuanbao,2007.08
        /// </summary>
        /// <returns>前景背景的临界值</returns>
        public int GetDgGrayValue()
        {
            int[] pixelNum = new int[256];           //图象直方图，共256个点
            int n, n1, n2;
            int total;                              //total为总和，累计值
            double m1, m2, sum, csum, fmax, sb;     //sb为类间方差，fmax存储最大方差值
            int k, t, q;
            int threshValue = 1;                      // 阈值
            int step = 1;
            //生成直方图
            for (int i = 0; i < bmpobj.Width; i++)
            {
                for (int j = 0; j < bmpobj.Height; j++)
                {
                    //返回各个点的颜色，以RGB表示
                    pixelNum[bmpobj.GetPixel(i, j).R]++;            //相应的直方图加1
                }
            }
            //直方图平滑化
            for (k = 0; k <= 255; k++)
            {
                total = 0;
                for (t = -2; t <= 2; t++)              //与附近2个灰度做平滑化，t值应取较小的值
                {
                    q = k + t;
                    if (q < 0)                     //越界处理
                        q = 0;
                    if (q > 255)
                        q = 255;
                    total = total + pixelNum[q];    //total为总和，累计值
                }
                pixelNum[k] = (int)((float)total / 5.0 + 0.5);    //平滑化，左边2个+中间1个+右边2个灰度，共5个，所以总和除以5，后面加0.5是用修正值
            }
            //求阈值
            sum = csum = 0.0;
            n = 0;
            //计算总的图象的点数和质量矩，为后面的计算做准备
            for (k = 0; k <= 255; k++)
            {
                sum += (double)k * (double)pixelNum[k];     //x*f(x)质量矩，也就是每个灰度的值乘以其点数（归一化后为概率），sum为其总和
                n += pixelNum[k];                       //n为图象总的点数，归一化后就是累积概率
            }

            fmax = -1.0;                          //类间方差sb不可能为负，所以fmax初始值为-1不影响计算的进行
            n1 = 0;
            for (k = 0; k < 256; k++)                  //对每个灰度（从0到255）计算一次分割后的类间方差sb
            {
                n1 += pixelNum[k];                //n1为在当前阈值遍前景图象的点数
                if (n1 == 0) { continue; }            //没有分出前景后景
                n2 = n - n1;                        //n2为背景图象的点数
                if (n2 == 0) { break; }               //n2为0表示全部都是后景图象，与n1=0情况类似，之后的遍历不可能使前景点数增加，所以此时可以退出循环
                csum += (double)k * pixelNum[k];    //前景的“灰度的值*其点数”的总和
                m1 = csum / n1;                     //m1为前景的平均灰度
                m2 = (sum - csum) / n2;               //m2为背景的平均灰度
                sb = (double)n1 * (double)n2 * (m1 - m2) * (m1 - m2);   //sb为类间方差
                if (sb > fmax)                  //如果算出的类间方差大于前一次算出的类间方差
                {
                    fmax = sb;                    //fmax始终为最大类间方差（otsu）
                    threshValue = k;              //取最大类间方差时对应的灰度的k就是最佳阈值
                }
            }
            return threshValue;
        }

        /// <summary>
        /// 得到有效图形并调整为可平均分割的大小
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="CharsCount">有效字符数</param>
        /// <returns></returns>
        public void GetPicValidByValue(int dgGrayValue, int CharsCount)
        {
            int posx1 = bmpobj.Width; int posy1 = bmpobj.Height;
            int posx2 = 0; int posy2 = 0;
            for (int i = 0; i < bmpobj.Height; i++)      //找有效区
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    int pixelValue = bmpobj.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            // 确保能整除
            int Span = CharsCount - (posx2 - posx1 + 1) % CharsCount;   //可整除的差额数
            if (Span < CharsCount)
            {
                int leftSpan = Span / 2;    //分配到左边的空列 ，如span为单数,则右边比左边大1
                if (posx1 > leftSpan)
                    posx1 = posx1 - leftSpan;
                if (posx2 + Span - leftSpan < bmpobj.Width)
                    posx2 = posx2 + Span - leftSpan;
            }
            //复制新图
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            bmpobj = bmpobj.Clone(cloneRect, bmpobj.PixelFormat);
        }

        /// <summary>
        /// 得到有效图形,图形为类变量
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <returns></returns>
        public void GetPicValidByValue(int dgGrayValue)
        {
            int posx1 = bmpobj.Width; int posy1 = bmpobj.Height;
            int posx2 = 0; int posy2 = 0;
            for (int i = 0; i < bmpobj.Height; i++)      //找有效区
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    int pixelValue = bmpobj.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            //复制新图
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            bmpobj = bmpobj.Clone(cloneRect, bmpobj.PixelFormat);
        }

        /// <summary>
        /// 得到有效图形,图形由外面传入
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="CharsCount">有效字符数</param>
        /// <returns></returns>
        public Bitmap GetPicValidByValue(Bitmap singlepic, int dgGrayValue)
        {
            int posx1 = singlepic.Width; int posy1 = singlepic.Height;
            int posx2 = 0; int posy2 = 0;
            for (int i = 0; i < singlepic.Height; i++)      //找有效区
            {
                for (int j = 0; j < singlepic.Width; j++)
                {
                    int pixelValue = singlepic.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            //复制新图
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            return singlepic.Clone(cloneRect, singlepic.PixelFormat);
        }

        /// <summary>
        /// 平均分割图片
        /// </summary>
        /// <param name="RowNum">水平上分割数</param>
        /// <param name="ColNum">垂直上分割数</param>
        /// <returns>分割好的图片数组</returns>
        public Bitmap[] GetSplitPics(int RowNum, int ColNum)
        {
            if (RowNum == 0 || ColNum == 0)
                return null;
            int singW = bmpobj.Width / RowNum;
            int singH = bmpobj.Height / ColNum;
            Bitmap[] PicArray = new Bitmap[RowNum * ColNum];

            Rectangle cloneRect;
            for (int i = 0; i < ColNum; i++)      //找有效区
            {
                for (int j = 0; j < RowNum; j++)
                {
                    cloneRect = new Rectangle(j * singW, i * singH, singW, singH);
                    PicArray[i * RowNum + j] = bmpobj.Clone(cloneRect, bmpobj.PixelFormat);//复制小块图
                }
            }
            return PicArray;
        }

        /// <summary>
        /// 返回灰度图片的点阵描述字串，1表示灰点，0表示背景
        /// </summary>
        /// <param name="singlepic">灰度图</param>
        /// <param name="dgGrayValue">背前景灰色界限</param>
        /// <returns></returns>
        public string GetSingleBmpCode(Bitmap singlepic, int dgGrayValue)
        {
            Color piexl;
            string code = "";
            for (int posy = 0; posy < singlepic.Height; posy++)
                for (int posx = 0; posx < singlepic.Width; posx++)
                {
                    piexl = singlepic.GetPixel(posx, posy);
                    if (piexl.R < dgGrayValue)    // Color.Black )
                        code = code + "1";
                    else
                        code = code + "0";
                }
            return code;
        }


        //根据周边有效点数去噪函数
        /// <summary>
        ///  去掉杂点（适合杂点/杂线粗为1）
        /// </summary>
        /// <param name="dgGrayValue">背前景灰色界限</param>
        /// <returns></returns>
        public void ClearNoise(int dgGrayValue, int MaxNearPoints)
        {
            Color piexl;
            int nearDots = 0;
            //int XSpan, YSpan, tmpX, tmpY;
            //逐点判断
            for (int i = 0; i < bmpobj.Width; i++)
                for (int j = 0; j < bmpobj.Height; j++)
                {
                    piexl = bmpobj.GetPixel(i, j);
                    if (piexl.R < dgGrayValue)
                    {
                        nearDots = 0;
                        //判断周围8个点是否全为空
                        if (i == 0 || i == bmpobj.Width - 1 || j == 0 || j == bmpobj.Height - 1)  //边框全去掉
                        {
                            bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        }
                        else
                        {
                            if (bmpobj.GetPixel(i - 1, j - 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i, j - 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i + 1, j - 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i - 1, j).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i + 1, j).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i - 1, j + 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i, j + 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i + 1, j + 1).R < dgGrayValue) nearDots++;
                        }

                        if (nearDots < MaxNearPoints)
                            bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));   //去掉单点 && 粗细小3邻边点
                    }
                    else  //背景
                        bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
        }
        /// <summary>
        /// 3×3中值滤波除杂，yuanbao,2007.10
        /// </summary>
        /// <param name="dgGrayValue"></param>
        public void ClearNoise(int dgGrayValue)
        {
            int x, y;
            byte[] p = new byte[9]; //最小处理窗口3*3
            byte s;
            //byte[] lpTemp=new BYTE[nByteWidth*nHeight];
            int i, j;

            //--!!!!!!!!!!!!!!下面开始窗口为3×3中值滤波!!!!!!!!!!!!!!!!
            for (y = 1; y < bmpobj.Height - 1; y++) //--第一行和最后一行无法取窗口
            {
                for (x = 1; x < bmpobj.Width - 1; x++)
                {
                    //取9个点的值
                    p[0] = bmpobj.GetPixel(x - 1, y - 1).R;
                    p[1] = bmpobj.GetPixel(x, y - 1).R;
                    p[2] = bmpobj.GetPixel(x + 1, y - 1).R;
                    p[3] = bmpobj.GetPixel(x - 1, y).R;
                    p[4] = bmpobj.GetPixel(x, y).R;
                    p[5] = bmpobj.GetPixel(x + 1, y).R;
                    p[6] = bmpobj.GetPixel(x - 1, y + 1).R;
                    p[7] = bmpobj.GetPixel(x, y + 1).R;
                    p[8] = bmpobj.GetPixel(x + 1, y + 1).R;
                    //计算中值
                    for (j = 0; j < 5; j++)
                    {
                        for (i = j + 1; i < 9; i++)
                        {
                            if (p[j] > p[i])
                            {
                                s = p[j];
                                p[j] = p[i];
                                p[i] = s;
                            }
                        }
                    }
                    //      if (bmpobj.GetPixel(x, y).R < dgGrayValue)
                    bmpobj.SetPixel(x, y, Color.FromArgb(p[4], p[4], p[4]));    //给有效值付中值
                }
            }
        }
    }
}