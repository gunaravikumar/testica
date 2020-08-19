using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Selenium.Scripts.Pages;

namespace Selenium.Scripts.Tests
{
    
        public  class Imager
        {
      
        public static void SaveJpeg(string path, Image img)
            {
                var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                var jpegCodec = GetEncoderInfo("image/jpeg");

                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;
                img.Save(path, jpegCodec, encoderParams);
            }

            /// <summary>  
            /// Save image  
            /// </summary>  
            /// <param name="path">path where to save</param>  
            /// <param name="img">image to save</param>  
            /// <param name="imageCodecInfo">codec info</param>  
            public static void Save(string path, Image img, ImageCodecInfo imageCodecInfo)
            {
                var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;
                img.Save(path, imageCodecInfo, encoderParams);
            }

            /// <summary>  
            /// get codec info by mime type  
            /// </summary>  
            /// <param name="mimeType"></param>  
            /// <returns></returns>  
            public static ImageCodecInfo GetEncoderInfo(string mimeType)
            {
                return ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == mimeType);
            }

            /// <summary>  
            /// the image remains the same size, and it is placed in the middle of the new canvas  
            /// </summary>  
            /// <param name="image">image to put on canvas</param>  
            /// <param name="width">canvas width</param>  
            /// <param name="height">canvas height</param>  
            /// <param name="canvasColor">canvas color</param>  
            /// <returns></returns>  
            public static Image PutOnCanvas(Image image, int width, int height, Color canvasColor)
            {
                var res = new Bitmap(width, height);
                using (var g = Graphics.FromImage(res))
                {
                    g.Clear(canvasColor);
                    var x = (width - image.Width) / 2;
                    var y = (height - image.Height) / 2;
                    g.DrawImageUnscaled(image, x, y, image.Width, image.Height);
                }

                return res;
            }

            /// <summary>  
            /// the image remains the same size, and it is placed in the middle of the new canvas  
            /// </summary>  
            /// <param name="image">image to put on canvas</param>  
            /// <param name="width">canvas width</param>  
            /// <param name="height">canvas height</param>  
            /// <returns></returns>  
            public static Image PutOnWhiteCanvas(Image image, int width, int height)
            {
                return PutOnCanvas(image, width, height, Color.White);
            }

            /// <summary>  
            /// resize an image and maintain aspect ratio  
            /// </summary>  
            /// <param name="image">image to resize</param>  
            /// <param name="newWidth">desired width</param>  
            /// <param name="maxHeight">max height</param>  
            /// <param name="onlyResizeIfWider">if image width is smaller than newWidth use image width</param>  
            /// <returns>resized image</returns>  
            public static Image Resize(Image image, int newWidth, int maxHeight, bool onlyResizeIfWider)
            {
                if (onlyResizeIfWider && image.Width <= newWidth) newWidth = image.Width;

                var newHeight = image.Height * newWidth / image.Width;
                if (newHeight > maxHeight)
                {
                    // Resize with height instead  
                    newWidth = image.Width * maxHeight / image.Height;
                    newHeight = maxHeight;
                }

                var res = new Bitmap(newWidth, newHeight);

                using (var graphic = Graphics.FromImage(res))
                {
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;
                    graphic.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                return res;
            }

            /// <summary>  
            /// Crop an image   
            /// </summary>  
            /// <param name="img">image to crop</param>  
            /// <param name="cropArea">rectangle to crop</param>  
            /// <returns>resulting image</returns>  
            public  Image Crop(Image img, Rectangle cropArea)
            {
                Bitmap bmpImage = new Bitmap(img);
                Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
                return bmpCrop;
            }

            public static byte[] imageToByteArray(System.Drawing.Image imageIn)
            {
                MemoryStream ms = new MemoryStream();
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }

            public static Image byteArrayToImage(byte[] byteArrayIn)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }

            //The actual converting function  
            public static string GetImage(object img)
            {
                return "data:image/jpg;base64," + Convert.ToBase64String((byte[])img);
            }


		public  void PerformImageResize(string pFilePath, int pWidth, int pHeight, string pOutputFileName)
		{
			try
			{
                if (File.Exists(pOutputFileName))
                    File.Delete(pOutputFileName);
                Bitmap imgBef = new Bitmap(pFilePath);
				Bitmap _imgR = ResizeImage(imgBef, pWidth, pHeight);
                System.Drawing.Image _img2 = Imager.PutOnCanvas(_imgR, pWidth, pHeight, System.Drawing.Color.Black);
                Imager.SaveJpeg(pOutputFileName, _img2);
                _imgR.Save(pOutputFileName, ImageFormat.Png);
                imgBef.Dispose();
                _imgR.Dispose();
                _img2.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("Failed due to exception : " + e.StackTrace);
            }

        }
        public Bitmap ResizeImage(Bitmap image ,int newWidth, int newHeight)
        {
            try
            {
                Bitmap temp = image;
                Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);

                double nWidthFactor = (double)temp.Width / (double)newWidth;
                double nHeightFactor = (double)temp.Height / (double)newHeight;

                double fx, fy, nx, ny;
                int cx, cy, fr_x, fr_y;
                Color color1 = new Color();
                Color color2 = new Color();
                Color color3 = new Color();
                Color color4 = new Color();
                byte nRed, nGreen, nBlue;

                byte bp1, bp2;

                for (int x = 0; x < bmap.Width; ++x)
                {
                    for (int y = 0; y < bmap.Height; ++y)
                    {

                        fr_x = (int)Math.Floor(x * nWidthFactor);
                        fr_y = (int)Math.Floor(y * nHeightFactor);
                        cx = fr_x + 1;
                        if (cx >= temp.Width) cx = fr_x;
                        cy = fr_y + 1;
                        if (cy >= temp.Height) cy = fr_y;
                        fx = x * nWidthFactor - fr_x;
                        fy = y * nHeightFactor - fr_y;
                        nx = 1.0 - fx;
                        ny = 1.0 - fy;

                        color1 = temp.GetPixel(fr_x, fr_y);
                        color2 = temp.GetPixel(cx, fr_y);
                        color3 = temp.GetPixel(fr_x, cy);
                        color4 = temp.GetPixel(cx, cy);

                        // Blue
                        bp1 = (byte)(nx * color1.B + fx * color2.B);

                        bp2 = (byte)(nx * color3.B + fx * color4.B);

                        nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Green
                        bp1 = (byte)(nx * color1.G + fx * color2.G);

                        bp2 = (byte)(nx * color3.G + fx * color4.G);

                        nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Red
                        bp1 = (byte)(nx * color1.R + fx * color2.R);

                        bp2 = (byte)(nx * color3.R + fx * color4.R);

                        nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        bmap.SetPixel(x, y, System.Drawing.Color.FromArgb
                (255, nRed, nGreen, nBlue));
                    }
                }
                Bitmap cloneimage = (Bitmap)bmap.Clone();
                return cloneimage;
            }
            catch (Exception e)
            {
                throw new Exception("Resize failed due to exception : " + e.StackTrace);
            }
        }

        public void CropAndSaveImage(string pFilePath, int x_axis, int y_axis, int pWidth, int pHeight, string pOutputFileName)
		{
            if (File.Exists(pOutputFileName))
                File.Delete(pOutputFileName);
            Bitmap imgBef = new Bitmap(pFilePath);
			Bitmap cropimage = DrawOutCropArea(imgBef, x_axis, y_axis, pWidth, pHeight);
			cropimage.Save(pOutputFileName, ImageFormat.Png);
			imgBef.Dispose();
		}
        public Bitmap DrawOutCropArea(Bitmap image, int xPosition, int yPosition, int width, int height)
        {
            Bitmap bmap = image;
            if (xPosition + width > bmap.Width)
                width = bmap.Width - xPosition;
            if (yPosition + height > bmap.Height)
                height = bmap.Height - yPosition;
            Rectangle rect = new Rectangle(xPosition, yPosition, width, height);
           Bitmap resultimg = (Bitmap)bmap.Clone(rect, bmap.PixelFormat);
			return resultimg;
        }

    }
}


