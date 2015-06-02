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
        private static int maxmax;
        public static async void Draw(IElement Element, int Width, Padding Padding, Action<bool> OnEnd)
        {
            bool result = await Task<bool>.Run(() =>
            {
                float SumResizedWidth = 0;
                int MinHeightInThisRow = 0;

                foreach (var El in Element)
                    if (El.GetSize().Height < MinHeightInThisRow || MinHeightInThisRow == 0)
                        MinHeightInThisRow = El.GetSize().Height;

                foreach (var El in Element)
                    SumResizedWidth += El.Resized(int.MaxValue, MinHeightInThisRow).Width;

                var Multiple = Width / SumResizedWidth;

                int Height = (int)(MinHeightInThisRow * Multiple);

                Bitmap canvas = new Bitmap(Width, Height);
                Graphics context = Graphics.FromImage(canvas);

                context.FillRectangle(Brushes.Orange, new RectangleF(0, 0, Width, Height));
                float PlusX = 0;
                drawimages3(Element, context, new SizeF(Width, Height), new RectangleF(), Padding, ref PlusX);

                canvas.Save("result.jpg");

                return true;
            });
            OnEnd(result);
        }
        
        private static void drawimages3(IElement Element, Graphics Context, SizeF Max, RectangleF Start, Padding Padding,ref float PlusX)
        {
            if (Element.GetTag() == ElementType.Row)
            {
                float SumResizedWidth = 0;
                int MinHeightInThisRow = 0;
                

                foreach (var El in Element)
                    if (El.GetSize().Height < MinHeightInThisRow || MinHeightInThisRow == 0)
                        MinHeightInThisRow = El.GetSize().Height;

                foreach (var El in Element)
                    SumResizedWidth += El.Resized(int.MaxValue, MinHeightInThisRow).Width;

                var Multiple = Max.Width / SumResizedWidth;

                foreach (var El in Element)
                {
                    Image Img = new Bitmap(El.GetSize().Width, El.GetSize().Height);

                    if (El.GetTag() == ElementType.Content)
                        Img = El.GetImage();

                    Img = Img.ScaleImage(int.MaxValue, MinHeightInThisRow);
                    Img = Img.ScaleImage(Img.Width * Multiple, int.MaxValue);

                    if (El.GetTag() == ElementType.Content)
                    {
                        Context.DrawImage(Img, Start.X, Start.Y, Img.Width, Img.Height);
                        PlusX += Img.Width;
                    }

                    if (El.GetTag() == ElementType.Column)
                    {
                        drawimages3(El, Context, Img.Size, Start, Padding, ref PlusX);                        
                    }
                    
                    Start.X += PlusX;
                }
            }
            else if (Element.GetTag() == ElementType.Column)
            {
                var MinWidthInThisColumn = 0;
                float SumResizedHeight = 0;

                foreach (var El in Element)
                    if (El.GetSize().Height < MinWidthInThisColumn || MinWidthInThisColumn == 0)
                        MinWidthInThisColumn = El.GetSize().Height;

                foreach (var El in Element)
                    SumResizedHeight += El.Resized(int.MaxValue, MinWidthInThisColumn).Height;

                var Multiple = Max.Height / SumResizedHeight;                

                foreach (var El in Element)
                {
                    Image Img = new Bitmap(El.GetSize().Width, El.GetSize().Height);

                    if (El.GetTag() == ElementType.Content)
                        Img = El.GetImage();

                    Img = Img.ScaleImage(MinWidthInThisColumn, int.MaxValue);
                    Img = Img.ScaleImage(int.MaxValue,Img.Height* Multiple);

                    if (El.GetTag() == ElementType.Content)
                    {
                        Context.DrawImage(Img, Start.X, Start.Y, Img.Width, Img.Height);
                        Start.Y += Img.Height;
                        PlusX += 0;
                    }

                    if (El.GetTag() == ElementType.Row)
                    {
                        drawimages3(El, Context, Img.Size, Start, Padding,ref PlusX);
                    }                    
                }
            }
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