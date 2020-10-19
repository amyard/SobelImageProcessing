using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SobelAlgImage.Helper
{
    public class SobelAlgorithm
    {
        public Bitmap ChooseCorrectAlgorithn(Bitmap imageSource, int algorithmChooser, int greyScale)
        {
            return algorithmChooser == 1 ? SobelFilter(imageSource, greyScale) : ConvolutionFilter(imageSource, xSobel, ySobel, 1.0, 0, true);
        }

        public void SobelProcessTaskChooser(Bitmap imageSource, int algorithmChooser, int positionInList, List<Bitmap> resultedListOfBitmaps, int greyScale)
        {
            Bitmap bmp = ChooseCorrectAlgorithn(imageSource, algorithmChooser, greyScale);
            resultedListOfBitmaps[positionInList] = bmp;
        }

        public Bitmap SobelProcessStart(Bitmap imageSource, int algorithmChooser, int greyScale)
        {
            Bitmap bmp = ChooseCorrectAlgorithn(imageSource, algorithmChooser, greyScale);
            return bmp;
        }

        public Bitmap SobelFilter(Bitmap sourceImage, int greyScale)
        {
            int b, g, r, r_x, g_x, b_x, r_y, g_y, b_y, grayscale, location, location2;

            sbyte[,] weights_x = new sbyte[,] { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } };
            sbyte[,] weights_y = new sbyte[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            BitmapData srcData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            byte[] pixelBuffer = new byte[srcData.Stride * sourceImage.Height];
            byte[] resultBuffer = new byte[srcData.Stride * sourceImage.Height];

            IntPtr pointer = srcData.Scan0;
            IntPtr pointer2 = resultData.Scan0;

            Marshal.Copy(pointer, pixelBuffer, 0, pixelBuffer.Length);

            for (int y = 0; y < sourceImage.Height; y++)
            {
                for (int x = 0; x < sourceImage.Width * 3; x += 3)
                {
                    //reset the gradients in x-direcion values
                    r_x = g_x = b_x = 0;

                    //reset the gradients in y-direction values
                    r_y = g_y = b_y = 0;

                    //to get the location of any pixel >> location = x + y * Stride
                    location = x + y * srcData.Stride;
                    for (int yy = -(int)Math.Floor(weights_y.GetLength(0) / 2.0d), yyy = 0; yy <= (int)Math.Floor(weights_y.GetLength(0) / 2.0d); yy++, yyy++)
                    {
                        //to prevent crossing the bounds of the array
                        if (y + yy >= 0 && y + yy < sourceImage.Height) 
                        {
                            for (int xx = -(int)Math.Floor(weights_x.GetLength(1) / 2.0d) * 3, xxx = 0; xx <= (int)Math.Floor(weights_x.GetLength(1) / 2.0d) * 3; xx += 3, xxx++)
                            {
                                //to prevent crossing the bounds of the array
                                if (x + xx >= 0 && x + xx <= sourceImage.Width * 3 - 3) 
                                {
                                    //to get the location of any pixel >> location = x + y * Stride
                                    location2 = x + xx + (yy + y) * srcData.Stride; 
                                    sbyte weight_x = weights_x[yyy, xxx];
                                    sbyte weight_y = weights_y[yyy, xxx];

                                    //applying the same weight to all channels
                                    b_x += pixelBuffer[location2] * weight_x;
                                    g_x += pixelBuffer[location2 + 1] * weight_x;
                                    r_x += pixelBuffer[location2 + 2] * weight_x;
                                    b_y += pixelBuffer[location2] * weight_y;
                                    g_y += pixelBuffer[location2 + 1] * weight_y;
                                    r_y += pixelBuffer[location2 + 2] * weight_y;
                                }
                            }
                        }
                    }
                    //getting the magnitude for each channel
                    b = (int)Math.Sqrt(Math.Pow(b_x, 2) + Math.Pow(b_y, 2));
                    g = (int)Math.Sqrt(Math.Pow(g_x, 2) + Math.Pow(g_y, 2));
                    r = (int)Math.Sqrt(Math.Pow(r_x, 2) + Math.Pow(r_y, 2));

                    if (b > 255) b = 255;
                    if (g > 255) g = 255;
                    if (r > 255) r = 255;

                    //getting grayscale value
                    grayscale = (b + g + r) / 3;

                    //thresholding to clean up the background
                    if (grayscale < greyScale) grayscale = 0;

                    resultBuffer[location] = (byte)grayscale;
                    resultBuffer[location + 1] = (byte)grayscale;
                    resultBuffer[location + 2] = (byte)grayscale;
                    //thresholding to clean up the background - add colors to page if we need
                    //if (b < 100) b = 0;
                    //if (g < 100) g = 0;
                    //if (r < 100) r = 0;
                }
            }
            Marshal.Copy(resultBuffer, 0, pointer2, pixelBuffer.Length);
            sourceImage.UnlockBits(srcData);
            resultImage.UnlockBits(resultData);

            return RemoveWhiteBorderFromImage(resultImage);
        }


        public Bitmap ConvolutionFilter(Bitmap sourceImage, double[,] xkernel, double[,] ykernel, double factor = 1, int bias = 0, bool grayscale = false)
        {
            //Image dimensions stored in variables for convenience
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            //Lock source image bits into system memory
            BitmapData srcData = sourceImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //Get the total number of bytes in your image - 32 bytes per pixel x image width x image height -> for 32bpp images
            int bytes = srcData.Stride * srcData.Height;

            //Create byte arrays to hold pixel information of your image
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            //Get the address of the first pixel data
            IntPtr srcScan0 = srcData.Scan0;

            //Copy image data to one of the byte arrays
            Marshal.Copy(srcScan0, pixelBuffer, 0, bytes);

            //Unlock bits from system memory -> we have all our needed info in the array
            sourceImage.UnlockBits(srcData);

            //Convert your image to grayscale if necessary
            if (grayscale == true)
            {
                float rgb = 0;
                for (int i = 0; i < pixelBuffer.Length; i += 4)
                {
                    rgb = pixelBuffer[i] * .21f;
                    rgb += pixelBuffer[i + 1] * .71f;
                    rgb += pixelBuffer[i + 2] * .071f;
                    pixelBuffer[i] = (byte)rgb;
                    pixelBuffer[i + 1] = pixelBuffer[i];
                    pixelBuffer[i + 2] = pixelBuffer[i];
                    pixelBuffer[i + 3] = 255;
                }
            }

            //Create variable for pixel data for each kernel
            double xr = 0.0;
            double xg = 0.0;
            double xb = 0.0;
            double yr = 0.0;
            double yg = 0.0;
            double yb = 0.0;
            double rt = 0.0;
            double gt = 0.0;
            double bt = 0.0;

            //This is how much your center pixel is offset from the border of your kernel
            //Sobel is 3x3, so center is 1 pixel from the kernel border
            int filterOffset = 1;
            int calcOffset = 0;
            int byteOffset = 0;

            //Start with the pixel that is offset 1 from top and 1 from the left side
            //this is so entire kernel is on your image
            for (int OffsetY = filterOffset; OffsetY < height - filterOffset; OffsetY++)
            {
                for (int OffsetX = filterOffset; OffsetX < width - filterOffset; OffsetX++)
                {
                    //reset rgb values to 0
                    xr = xg = xb = yr = yg = yb = 0;
                    rt = gt = bt = 0.0;

                    //position of the kernel center pixel
                    byteOffset = OffsetY * srcData.Stride + OffsetX * 4;

                    //kernel calculations
                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + filterX * 4 + filterY * srcData.Stride;
                            xb += (double)(pixelBuffer[calcOffset]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xg += (double)(pixelBuffer[calcOffset + 1]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xr += (double)(pixelBuffer[calcOffset + 2]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            yb += (double)(pixelBuffer[calcOffset]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yg += (double)(pixelBuffer[calcOffset + 1]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yr += (double)(pixelBuffer[calcOffset + 2]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    //total rgb values for this pixel
                    bt = Math.Sqrt((xb * xb) + (yb * yb));
                    gt = Math.Sqrt((xg * xg) + (yg * yg));
                    rt = Math.Sqrt((xr * xr) + (yr * yr));

                    //set limits, bytes can hold values from 0 up to 255;
                    if (bt > 255) bt = 255;
                    else if (bt < 0) bt = 0;
                    if (gt > 255) gt = 255;
                    else if (gt < 0) gt = 0;
                    if (rt > 255) rt = 255;
                    else if (rt < 0) rt = 0;

                    //set new data in the other byte array for your image data
                    resultBuffer[byteOffset] = (byte)(bt);
                    resultBuffer[byteOffset + 1] = (byte)(gt);
                    resultBuffer[byteOffset + 2] = (byte)(rt);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            //Create new bitmap which will hold the processed data
            Bitmap resultImage = new Bitmap(width, height);

            //Lock bits into system memory
            BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            //Copy from byte array that holds processed data to bitmap
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

            //Unlock bits from system memory
            resultImage.UnlockBits(resultData);

            //Return processed image
            return resultImage;
        }


        #region private methods
        //Sobel operator kernel for horizontal pixel changes
        private double[,] xSobel
        {
            get
            {
                return new double[,]
                {
                    { -1, 0, 1 },
                    { -2, 0, 2 },
                    { -1, 0, 1 }
                };
            }
        }

        //Sobel operator kernel for vertical pixel changes
        private double[,] ySobel
        {
            get
            {
                return new double[,]
                {
                    {  1,  2,  1 },
                    {  0,  0,  0 },
                    { -1, -2, -1 }
                };
            }
        }

        private Bitmap RemoveWhiteBorderFromImage(Bitmap bitmap)
        {
            int leftSHift = 0;
            int topShift = 0;
            int rightShift = 0;
            int bottomShift = 0;

            // left Shift
            for (int widthX = 0; widthX < bitmap.Width; widthX++)
            {
                for (int heightY = 0; heightY < bitmap.Height; heightY++)
                {
                    Color c = bitmap.GetPixel(widthX, heightY);
                    if (!c.Name.Equals("0"))
                    {
                        leftSHift = widthX;
                        break;
                    }

                }
                if (!leftSHift.Equals(0))
                    break;
            }

            // Top Shift
            for (int widthX = 0; widthX < bitmap.Height; widthX++)
            {
                for (int heightY = 0; heightY < bitmap.Width - 1; heightY++)
                {
                    Color c = bitmap.GetPixel(heightY, widthX);
                    if (!c.Name.Equals("0"))
                    {
                        topShift = widthX;
                        break;
                    }
                }
                if (!topShift.Equals(0))
                    break;
            }

            // Right Shift
            for (int heightX = bitmap.Width - 1; heightX >= 0; heightX--)
            {
                for (int widthY = 0; widthY < bitmap.Height; widthY++)
                {
                    Color c = bitmap.GetPixel(heightX, widthY);
                    if (!c.Name.Equals("0"))
                    {
                        rightShift = heightX;
                        break;
                    }
                }
                if (!rightShift.Equals(0))
                    break;
            }

            //Bottom Shift.
            for (int heightX = bitmap.Height - 1; heightX >= 0; heightX--)
            {
                for (int widthY = 0; widthY < bitmap.Width - 1; widthY++)
                {
                    Color c = bitmap.GetPixel(widthY, heightX);
                    if (!c.Name.Equals("0"))
                    {
                        bottomShift = heightX;
                        break;
                    }
                }
                if (!bottomShift.Equals(0))
                    break;
            }

            Rectangle cropRect = new Rectangle
                (
                leftSHift + 1,
                topShift,
                bitmap.Width - (leftSHift + (bitmap.Width - rightShift)),
                bitmap.Height - (topShift + (bitmap.Height - bottomShift))
                );

            Bitmap src = bitmap;
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);

            }

            return target;
        }

        #endregion
    }
}
