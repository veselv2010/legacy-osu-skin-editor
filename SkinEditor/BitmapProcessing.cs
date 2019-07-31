using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SkinEditor
{
    class BitmapProcessing
    {
        public static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            int outputImageWidth = firstImage.Width;

            int outputImageHeight = firstImage.Height - 7;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(7,7), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }
    }
}
