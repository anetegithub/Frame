using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Sharp
{
    public static class Canvas
    {
        public static async void Draw(IElement Element, int Width, Padding Padding, Action<bool> OnEnd)
        {
            bool result = await Task<bool>.Run(() =>
            {
                float maxWidth = Element.GetResize().Width,
                maxHeight = Element.GetResize().Height;                

                GC.Collect();

                int Height = Width;// (int)(MinHeightInThisRow * Multiple);

                Bitmap canvas = new Bitmap((int)maxWidth, (int)maxWidth);
                Graphics context = Graphics.FromImage(canvas);

                context.FillRectangle(Brushes.Orange, new RectangleF(0, 0, Width, Height));                
                drawimages(Element, context, new SizeF(Width, Height), new RectangleF());

                var img = canvas.ScaleImage(Width, int.MaxValue);

                img.Save("result.jpg");

                return true;
            });
            OnEnd(result);
        }

        static int row = 0;
        static int col = 0;

        #region hide
        //private static void drawimages3(Element Element, Graphics Context, SizeF Max, RectangleF Start, Padding Padding)
        //{
        //    if (Element.GetTag() == ElementType.Row)
        //    {
        //        float SumResizedWidth = 0;
        //        int MinHeightInThisRow = 0;

        //        foreach (var El in Element)
        //            if (El.GetResize(Max, row != 0).Height < MinHeightInThisRow || MinHeightInThisRow == 0)
        //                MinHeightInThisRow = El.GetResize(Max, row != 0).Height;
        //        row++;

        //        foreach (var El in Element)
        //            SumResizedWidth += El.Resized(int.MaxValue, MinHeightInThisRow).Width;

        //        //var Multiple = Max.Width / SumResizedWidth;

        //        foreach (var El in Element)
        //        {
        //            Image Img = new Bitmap(1, 1);

        //            if (El.GetTag() == ElementType.Content)
        //            {
        //                Img = El.GetImage();

        //                Img = Img.ScaleImage(int.MaxValue, MinHeightInThisRow);
        //                Img = Img.ScaleImage(Img.Width, int.MaxValue);
        //            }
        //            else
        //            {
        //                Img = new Bitmap(El.GetResize(Max).Width, El.GetResize(Max).Height);
        //            }

        //            if (El.GetTag() == ElementType.Content)
        //            {
        //                Context.DrawImage(Img, Start.X, Start.Y, Img.Width, Img.Height);
        //            }

        //            if (El.GetTag() == ElementType.Column)
        //            {
        //                drawimages3((Element)El, Context, Img.Size, Start, Padding);
        //            }

        //            Start.X += Img.Width;
        //        }
        //    }
        //    else if (Element.GetTag() == ElementType.Column)
        //    {
        //        var MinWidthInThisColumn = 0;
        //        float SumResizedHeight = 0;

        //        foreach (var El in Element)
        //            if (El.GetResize(Max, col != 0).Height < MinWidthInThisColumn || MinWidthInThisColumn == 0)
        //                MinWidthInThisColumn = El.GetResize(Max, col != 0).Height;

        //        col++;

        //        foreach (var El in Element)
        //            SumResizedHeight += El.Resized(int.MaxValue, MinWidthInThisColumn).Height;

        //        //var Multiple = Max.Height / SumResizedHeight;                

        //        foreach (var El in Element)
        //        {
        //            Image Img = new Bitmap(El.GetResize(Max).Width, El.GetResize(Max).Height);

        //            if (El.GetTag() == ElementType.Content)
        //                Img = El.GetImage();

        //            Img = Img.ScaleImage(MinWidthInThisColumn, int.MaxValue);
        //            Img = Img.ScaleImage(int.MaxValue, Img.Height);

        //            if (El.GetTag() == ElementType.Content)
        //            {
        //                Context.DrawImage(Img, Start.X, Start.Y, Img.Width, Img.Height);
        //                Start.Y += Img.Height;
        //            }

        //            if (El.GetTag() == ElementType.Row)
        //            {
        //                drawimages3((Element)El, Context, Img.Size, Start, Padding);
        //            }
        //        }
        //    }
        //}
        #endregion

        private static RectangleF drawimages(IElement Element, Graphics Context, SizeF Limit, RectangleF Pos)
        {
            if (Element.GetTag() == ElementType.Row)
            {
                int ReturnY = 0;
                float MinHeightInThisRow = 0;

                foreach (var El in Element)
                    if (El.GetTag()!= ElementType.Content && (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0))
                        MinHeightInThisRow = El.GetResize().Height;
                
                if (MinHeightInThisRow==0)
                    foreach (var El in Element)
                        if (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0)
                            MinHeightInThisRow = El.GetResize().Height;

                foreach (var El in Element)
                {
                    if (El.GetTag() == ElementType.Content)
                    {
                        var img = El.GetImage();
                        img = img.ScaleImage(int.MaxValue, MinHeightInThisRow);

                        GC.Collect();

                        Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);
                        ReturnY = img.Height;
                        Pos.X += img.Width;
                    }
                    else
                    {
                        Pos.X += drawimages(El, Context, Limit, Pos).X;
                    }
                }

                return new RectangleF() { Y = ReturnY };
            }
            else if (Element.GetTag() == ElementType.Column)
            {
                int ReturnX = 0;
                float MinWidthInThisColumn = 0;
                
                foreach (var El in Element)
                    if (El.GetTag() != ElementType.Content && (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0))
                        MinWidthInThisColumn = El.GetResize().Width;

                if (MinWidthInThisColumn == 0)
                    foreach (var El in Element)
                        if (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0)
                            MinWidthInThisColumn = El.GetResize().Width;

                foreach (var El in Element)
                {
                    if (El.GetTag() == ElementType.Content)
                    {
                        var img = El.GetImage();
                        img = img.ScaleImage(MinWidthInThisColumn, int.MaxValue);

                        GC.Collect();

                        Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);
                        ReturnX = img.Width;
                        Pos.Y += img.Height;
                    }
                    else
                    {
                        Pos.Y += drawimages(El, Context, Limit, Pos).Y;
                    }
                }

                return new RectangleF() { X = ReturnX };
            }

            return Pos;
        }

        public struct Padding
        {
            public Padding(float Top, float Bottom, float Left, float Right)
            {
                this.Top = Top;
                this.Bottom = Bottom;
                this.Left = Left;
                this.Right = Right;
            }
            public float Top, Bottom, Left, Right;
        }
    }
}