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

                Bitmap canvas = new Bitmap((int)maxWidth, (int)maxHeight, 0, System.Drawing.Imaging.PixelFormat.Format16bppRgb565, IntPtr.Zero);
                Graphics context = Graphics.FromImage(canvas);

                context.FillRectangle(Brushes.Orange, new RectangleF(0, 0, Width, Height));                
                drawimages(Element, context, new RectangleF(),new SizeF(),new SizeF());

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

        private static RectangleF drawimages(IElement Element, Graphics Context, RectangleF Pos, SizeF Previos, SizeF Limit)
        {
            if (Element.GetTag() == ElementType.Row)
            {
                if (Limit.Width > 0)
                {
                    int ReturnX = 0,
                        ReturnY = 0;
                    float MinHeightInThisColumn = 0;

                    //берём по самому минимальному
                    foreach (var El in Element)
                        if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage() && (El.GetResize().Height < MinHeightInThisColumn || MinHeightInThisColumn == 0))
                            MinHeightInThisColumn = El.GetResize().Height;

                    if (MinHeightInThisColumn == 0)
                        foreach (var El in Element)
                            if (El.GetResize().Height < MinHeightInThisColumn || MinHeightInThisColumn == 0)
                                MinHeightInThisColumn = El.GetResize().Height;

                    float SumOfWidth = 0;

                    foreach (var El in Element)
                    {
                        GC.Collect();
                        if (El.GetTag() == ElementType.Content)
                        {
                            SumOfWidth += El.GetImage().Size.ScaleSize(int.MaxValue, MinHeightInThisColumn).Width;
                        }
                    }

                    foreach (var El in Element)
                    {
                        GC.Collect();
                        if (El.GetTag() == ElementType.Content)
                        {
                            var a = (float)El.GetImage().Size.ScaleSize(int.MaxValue,MinHeightInThisColumn).Width / SumOfWidth;
                            var b = a * Limit.Width;
                            var img = El.GetImage().ScaleImage(b,int.MaxValue);

                            Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);

                            ReturnX = img.Width;
                            ReturnY = img.Height;
                            Pos.X += img.Width;
                        }
                    }

                    return new RectangleF() { X = ReturnX, Y = ReturnY };
                }
                else
                {
                    int ReturnY = 0;
                    int ReturnX = 0;
                    float MinHeightInThisRow = 0,
                        PreviosBranches = 0;

                    //берём по самому минимальному
                    //foreach (var El in Element)
                    //    if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage() && (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0))
                    //        MinHeightInThisRow = El.GetResize().Height;

                    //попробуем брать по самому внутреннему
                    foreach (var El in Element)
                        if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage())
                            if (El.GetTree().Count() > PreviosBranches)
                            {
                                PreviosBranches = El.GetTree().Count();
                                MinHeightInThisRow = El.GetResize().Height;
                            }

                    if (MinHeightInThisRow == 0)
                        foreach (var El in Element)
                            if (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0)
                                MinHeightInThisRow = El.GetResize().Height;

                    foreach (var El in Element)
                    {
                        GC.Collect();
                        if (El.GetTag() == ElementType.Content)
                        {
                            var img = El.GetImage();
                            img = img.ScaleImage(int.MaxValue, MinHeightInThisRow);

                            if (Element.CountInner() == 1)
                                img = img.ScaleImage(Previos.Width, int.MaxValue);

                            Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);
                            ReturnY = img.Height;
                            Pos.X += img.Width;
                            ReturnX += img.Width;
                        }
                        else
                        {
                            Previos.Height = MinHeightInThisRow;

                            //определяем нужен ли лимит этой колонке
                            if (El.GetResize().Height < MinHeightInThisRow)
                                Limit.Height = MinHeightInThisRow;
                            else
                                Limit.Height = 0;

                            var returned = drawimages(El, Context, Pos, Previos, Limit);


                            if (El.GetTag() == ElementType.Row)
                                ReturnX += (int)returned.Y;

                            //ReturnX = (int)returned.X;
                            Pos.X += (int)returned.X;
                            ReturnY = (int)returned.Y;
                        }
                    }

                    return new RectangleF() { Y = ReturnY, X = ReturnX };
                }
            }
            else if (Element.GetTag() == ElementType.Column)
            {
                if (Limit.Height > 0)
                {
                    int ReturnX = 0,
                        ReturnY = 0;
                    float MinWidthInThisColumn = 0;

                    //берём по самому минимальному
                    foreach (var El in Element)
                        if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage() && (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0))
                            MinWidthInThisColumn = El.GetResize().Width;

                    if (MinWidthInThisColumn == 0)
                        foreach (var El in Element)
                            if (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0)
                                MinWidthInThisColumn = El.GetResize().Width;

                    float SumOfHeight = 0;

                    foreach(var El in Element)
                    {
                        GC.Collect();
                        if(El.GetTag()== ElementType.Content)
                        {
                            SumOfHeight += El.GetImage().Size.ScaleSize(MinWidthInThisColumn, int.MaxValue).Height;
                        }
                    }

                    foreach (var El in Element)
                    {
                        GC.Collect();
                        if (El.GetTag() == ElementType.Content)
                        {
                            var a = (float)El.GetImage().Size.ScaleSize(MinWidthInThisColumn, int.MaxValue).Height / SumOfHeight;
                            var b = a * Limit.Height;
                            var img = El.GetImage().ScaleImage(int.MaxValue, b);

                            Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);

                            ReturnX = img.Width;
                            ReturnY = img.Height;
                            Pos.Y += img.Height;
                        }
                    }

                    return new RectangleF() { X = ReturnX, Y = ReturnY };
                }
                else
                {

                    int ReturnX = 0;
                    int ReturnY = 0;
                    float MinWidthInThisColumn = 0,
                        PreviosBranches = 0;

                    //берём по самому минимальному
                    //foreach (var El in Element)
                    //    if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage() && (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0))
                    //        MinWidthInThisColumn = El.GetResize().Width;

                    //попробуем брать по самому внутреннему
                    foreach (var El in Element)
                        if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage())
                            if (El.GetTree().Count() > PreviosBranches)
                            {
                                PreviosBranches = El.GetTree().Count();
                                MinWidthInThisColumn = El.GetResize().Width;
                            }

                    if (MinWidthInThisColumn == 0)
                        foreach (var El in Element)
                            if (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0)
                                MinWidthInThisColumn = El.GetResize().Width;

                    foreach (var El in Element)
                    {
                        GC.Collect();
                        if (El.GetTag() == ElementType.Content)
                        {
                            var img = El.GetImage();
                            img = img.ScaleImage(MinWidthInThisColumn, int.MaxValue);

                            if (Element.CountInner() == 1)
                                img = img.ScaleImage(int.MaxValue, Previos.Height);

                            Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);
                            ReturnX = img.Width;

                            ReturnY += img.Height;

                            Pos.Y += img.Height;
                        }
                        else
                        {
                            Previos.Width = MinWidthInThisColumn;

                            //определяем нужен ли лимит этой строке
                            if (El.GetResize().Width > MinWidthInThisColumn)
                                Limit.Width = MinWidthInThisColumn;
                            else
                                Limit.Width = 0;

                            var returned = drawimages(El, Context, Pos, Previos, Limit);

                            if (El.GetTag() == ElementType.Row)
                                ReturnY += (int)returned.Y;

                            Pos.Y += (int)returned.Y;
                            ReturnX += (int)returned.X;
                        }
                    }

                    return new RectangleF() { X = ReturnX, Y = ReturnY };
                }
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