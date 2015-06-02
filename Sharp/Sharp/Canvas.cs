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
        public static async void Draw(IElement Element, int Width, Padding Padding,Action<bool> OnEnd)
        {
            bool result = await Task<bool>.Run(() =>
            {                
                int Height = Element.GetMaxSize().Height;
                maxmax = Height;

                Bitmap canvas = new Bitmap(Width, Height);
                Graphics context = Graphics.FromImage(canvas);

                context.FillRectangle(Brushes.Orange, new RectangleF(0, 0, Width, Height));

                try
                {
                    drawimages(Element, context, new SizeF(Width, Height), new RectangleF(), Padding);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }

                canvas.Save("result.jpg");

                return true;
            });
            OnEnd(result);
        }

        private static void drawimages(IElement Element, Graphics Context, SizeF Max, RectangleF StartPos, Padding Padding)
        {
            RectangleF rect = StartPos;

            float Hmultiplicator = 0;

            if (Element.GetTag()== ElementType.Row)
            {
                var MinHeight = Element.GetMinSize().Height;
                MinHeight *= Element.CountInner();
                Hmultiplicator = (Max.Width / MinHeight);
            }

            foreach (IElement El in Element)
            {
                if (El is Picture)
                {
                    var img = El.GetImage().ScaleImage(int.MaxValue, Element.GetMinSize().Height*(Element.GetSize().Height-(Element.GetMinSize().Height* Element.CountInner())));
                    
                    //img = img.ScaleImage(img.Width * Hmultiplicator, int.MaxValue);
                    Context.DrawImage(img, rect.X, rect.Y, img.Width, img.Height);
                    rect.X += img.Width;
                }

                

                //    var widththisEl = (float)El.GetSize().Width;
                //var percent = (widththisEl / Element.GetSize().Width) * 100;

                //var segment = (Max.Width / 100) * percent;
                //rect.Width = segment;
                //rect.Height = (float)El.GetSize().Height;

                //if (El is Picture)
                //{
                //    //RectangleF paddingrect = new RectangleF(rect.X + Padding.Left, rect.Y+Padding.Top, rect.Width - Padding.Left, rect.Height-Padding.Top);
                //    //paddingrect.Width -= Padding.Right;
                //    //paddingrect.Height -= Padding.Bottom;
                //    //var img = El.GetImage().ScaleImage(paddingrect.Width, maxmax);
                //    var img = El.GetImage();
                //    Context.DrawImage(img,rect);
                //}
                ////else
                ////{
                ////    if (El.GetTag() == ElementType.Row)
                ////        Max.Height = segment;
                ////    else
                ////        Max.Width = segment;
                ////    drawimages(El, Context, Max, rect, Padding);
                ////}

                //if (Element.GetTag() == ElementType.Row)
                //    rect.X += segment;
                //else
                //    rect.Y += segment;
            }
        }

        public struct Padding
        {
            public Padding(float Top,float Bottom,float Left,float Right)
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