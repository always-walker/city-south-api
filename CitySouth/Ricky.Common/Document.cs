using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;


namespace Ricky
{
    public class Document
    {
        #region 图像处理
        /// <summary>
        /// 图像处理按照固定高宽等比例缩放
        /// </summary>
        /// <param name="sFile">源文件绝对路径</param>
        /// <param name="dFile">保存到绝对路径</param>
        /// <param name="dWidth">预处理图片缩放宽</param>
        /// <param name="isHeight">是否按照高度缩放</param>
        /// <returns>是否成功</returns>
        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth)
        {
            return GetPicThumbnail(sFile, null, dFile, dHeight, dWidth, false, false);
        }
        /// <summary>
        /// 图像处理按照高宽拉伸或者不拉伸缩放
        /// </summary>
        /// <param name="sFile">源文件绝对路径</param>
        /// <param name="dFile">保存到绝对路径</param>
        /// <param name="dHeight">预处理图片高</param>
        /// <param name="dWidth">预处理图片宽</param>
        ///<param name="isTensile">是否拉伸</param>
        /// <returns>是否成功</returns>
        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth, bool isTensile)
        {
            return GetPicThumbnail(sFile, null, dFile, dHeight, dWidth, true, isTensile);
        }
        /// <summary>
        /// 图像处理按照固定高宽等比例缩放
        /// </summary>
        /// <param name="sFile">源文件流</param>
        /// <param name="dFile">保存到绝对路径</param>
        /// <param name="dWidth">预处理图片缩放宽</param>
        /// <param name="isHeight">是否按照高度缩放</param>
        /// <returns>是否成功</returns>
        public static bool GetPicThumbnail(Stream sFile, string dFile, int dHeight, int dWidth)
        {
            return GetPicThumbnail(null, sFile, dFile, dHeight, dWidth, false, false);
        }
        /// <summary>
        /// 图像处理按照高宽拉伸或者不拉伸缩放
        /// </summary>
        /// <param name="sFile">源文件流</param>
        /// <param name="dFile">保存到绝对路径</param>
        /// <param name="dHeight">预处理图片高</param>
        /// <param name="dWidth">预处理图片宽</param>
        ///<param name="isTensile">是否拉伸</param>
        /// <returns>是否成功</returns>
        public static bool GetPicThumbnail(string sFile_d, Stream sFile_s, string dFile, int dHeight, int dWidth, bool isFix, bool isTensile)
        {
            Image iSource =null;
            if(string.IsNullOrEmpty(sFile_d))
                iSource = Image.FromStream(sFile_s);//创建Image实例
            else
                iSource = Image.FromFile(sFile_d);//创建Image实例
            ImageFormat tFormat = iSource.RawFormat;//设置保存格式
            Size tem_size = new Size(iSource.Width, iSource.Height);//实例化Size
            #region 高宽计算
            int ImgW = dWidth, ImgH = dHeight;
            int sW = 0, sH = 0;//记录宽度和高度
            if (isFix)
            {
                if (!isTensile)
                {
                    if (tem_size.Height > dHeight || tem_size.Width > dWidth)
                    {
                        if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                        {
                            sW = dWidth;
                            sH = (dWidth * tem_size.Height) / tem_size.Width;
                        }
                        else
                        {
                            sH = dHeight;
                            sW = (tem_size.Width * dHeight) / tem_size.Height;
                        }
                    }
                    else
                    {
                        sW = tem_size.Width;
                        sH = tem_size.Height;
                    }
                }
            }
            else
            {
                sW = tem_size.Width; sH = tem_size.Height;
                if (tem_size.Width > dWidth && dHeight == -1)
                {
                    sW = dWidth; sH = (int)(tem_size.Height * dWidth) / tem_size.Width;
                }
                else if (tem_size.Height > dHeight && dWidth == -1)
                {
                    sH = dHeight; sW = (int)(tem_size.Width * dHeight) / tem_size.Height;
                }
                ImgW = sW; ImgH = sH;
            }
            #endregion
            Bitmap oB = new Bitmap(ImgW, ImgH);//实例化Bitmap
            Graphics g = Graphics.FromImage(oB);//实例化Graphics
            g.Clear(Color.White);//设置画布背景颜色
            g.CompositingQuality = CompositingQuality.HighQuality;//Graphics类合成图像呈现质量
            g.SmoothingMode = SmoothingMode.HighQuality;//Graphics类得呈现质量
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;//Graphics关联的插补模式
            if (isFix)
                g.DrawImage(iSource, new System.Drawing.Rectangle((ImgW - sW) / 2, (ImgH - sH) / 2, sW, sH), 0, 0, tem_size.Width, tem_size.Height, GraphicsUnit.Pixel);//开始绘制图像
            else
                g.DrawImage(iSource, new System.Drawing.Rectangle(0, 0, sW, sH), 0, 0, tem_size.Width, tem_size.Height, GraphicsUnit.Pixel);//开始重新绘制图像
            g.Dispose();
            EncoderParameters eP = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = 100;
            EncoderParameter eParam = new EncoderParameter(Encoder.Quality, qy);
            eP.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIindo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))//设置JPEG编码
                    {
                        jpegICIindo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIindo != null)
                {
                    oB.Save(dFile, jpegICIindo, eP);
                }
                else
                {
                    oB.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                oB.Dispose();
            }
        }
        public static byte[] GetPicFixWidth(byte[] sFile_b, int dWidth)
        {
            Image iSource = Image.FromStream(new MemoryStream(sFile_b));
            ImageFormat tFormat = iSource.RawFormat;//设置保存格式
            Size tem_size = new Size(iSource.Width, iSource.Height);//实例化Size
            int ImgW = dWidth, ImgH = (tem_size.Height * dWidth) / tem_size.Width;
            Bitmap oB = new Bitmap(ImgW, ImgH);//实例化Bitmap
            Graphics g = Graphics.FromImage(oB);//实例化Graphics
            g.Clear(Color.White);//设置画布背景颜色
            g.CompositingQuality = CompositingQuality.HighQuality;//Graphics类合成图像呈现质量
            g.SmoothingMode = SmoothingMode.HighQuality;//Graphics类得呈现质量
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;//Graphics关联的插补模式
            g.DrawImage(iSource, new System.Drawing.Rectangle(0, 0, ImgW, ImgH), 0, 0, tem_size.Width, tem_size.Height, GraphicsUnit.Pixel);//开始重新绘制图像
            g.Dispose();
            MemoryStream dSource = null;
            try
            {
                dSource = new MemoryStream();
                oB.Save(dSource, tFormat);
                byte[] dByte = new byte[dSource.Length];
                dByte = dSource.ToArray();
                return dByte;
            }
            catch
            {
                return null;
            }
            finally
            {
                iSource.Dispose();
                oB.Dispose();
                dSource.Close();
            }
        }
        public static void SavePicFixWidth(Stream file, int dWidth, string localPath)
        {
            Image iSource = Image.FromStream(file);
            if (iSource.Width > dWidth)
            {
                int newHeight = (dWidth * iSource.Height) / iSource.Width;
                Bitmap oB = new Bitmap(dWidth, newHeight);//实例化Bitmap
                Graphics g = Graphics.FromImage(oB);//实例化Graphics
                g.Clear(Color.White);//设置画布背景颜色
                g.CompositingQuality = CompositingQuality.HighQuality;//Graphics类合成图像呈现质量
                g.SmoothingMode = SmoothingMode.HighQuality;//Graphics类得呈现质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;//Graphics关联的插补模式
                g.DrawImage(iSource, new System.Drawing.Rectangle(0, 0, dWidth, newHeight), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);//开始重新绘制图像
                oB.Save(localPath, iSource.RawFormat);
                g.Dispose();
            }
            else
            {
                iSource.Save(localPath, iSource.RawFormat);
            }
        }
        #endregion
    }
}
