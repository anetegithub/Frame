using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Sharp
{
    public static class Extensions
    {
        public static Image ScaleImage(this Image image, float maxWidth, float maxHeight)
        {
            var ratioX = maxWidth / image.Width;
            var ratioY = maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            GC.Collect();

            var newImage = new Bitmap(newWidth, newHeight,0, System.Drawing.Imaging.PixelFormat.Format16bppRgb565,IntPtr.Zero);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        public static SizeF Resized(this IElement Element, float MaxWidth, float MaxHeigt)
        {
            var ElSize = Element.GetSize();
            var ratioX = MaxWidth / ElSize.Width;
            var ratioY = MaxHeigt / ElSize.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = ElSize.Width * ratio;
            var newHeight = ElSize.Height * ratio;

            return new SizeF(newWidth, newHeight);
        }

        public static SizeF ScaleSize(this SizeF Size, float MaxWidth, float MaxHeigt)
        {
            var ratioX = MaxWidth / Size.Width;
            var ratioY = MaxHeigt / Size.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = Size.Width * ratio;
            var newHeight = Size.Height * ratio;

            return new SizeF(newWidth, newHeight);
        }

        public static SizeF ScaleSize(this Size Size, float MaxWidth, float MaxHeigt)
        {
            return new SizeF(Size.Width, Size.Height).ScaleSize(MaxWidth, MaxHeigt);
        }

        public static Image ScaleImageByWidth(this Image imgToResize, float width)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = ((float)width / (float)sourceWidth);

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        public static int GetTreeCount(this Element Element)
        {
            var ElementCount = (from x in Element.GetTree() where x.GetImage() != null select x).Count();

            return ElementCount;
        }

        public static bool InnerOnlyImage(this Element Element)
        {
            if (Element.CountInner() == 1)
                foreach (var El in Element)
                    if (El.GetTag() == Tag.Content)
                        return true;
            return false;
        }

        public static bool HaveSomeTag(this Element Element)
        {
            var SelfTag = Element.GetTag();
            foreach (var El in Element)
                if (El.GetTag() == SelfTag)
                    return true;
            return false;
        }        

        public static bool IsLimited(this Element Element,SizeF Limited)
        {
            if (Element.Tag == Tag.Row)
                if (Limited.Height != 0)
                    return true;

            if (Element.Tag == Tag.Column)
                if (Limited.Width != 0)
                    return true;

            return false;
        }
    }
}