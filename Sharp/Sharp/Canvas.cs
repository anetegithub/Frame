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
        public static async void Draw(Element Element, int Width, Padding Padding, Action<bool> OnEnd)
        {
            bool result = await Task<bool>.Run(() =>
            {
                float maxWidth = Element.DrivenSize.Width,
                    maxHeight = Element.DrivenSize.Height;
                
                Bitmap Canvas = new Bitmap((int)maxWidth, (int)maxHeight*2, 0, System.Drawing.Imaging.PixelFormat.Format16bppRgb565, IntPtr.Zero);
                Graphics Context = Graphics.FromImage(Canvas);                

                DrawElements(Element, Context,new PointF(),new SizeF());

                var img = Canvas.ScaleImage(maxWidth, int.MaxValue);
                Canvas.Save("result.jpg");

                return true;
            });
            OnEnd(result);
        }

        static int row = 0;
        static int col = 0;

        //private static RectangleF drawimages(IElement Element, Graphics Context, RectangleF Pos, SizeF Previos, SizeF Limit)
        //{
        //    if (Element.GetTag() == Tag.Row)
        //    {
        //        if (Limit.Width > 0)
        //        {
        //            int ReturnX = 0,
        //                ReturnY = 0;
        //            float MinHeightInThisColumn = 0;

        //            //берём по самому минимальному
        //            foreach (var El in Element)
        //                if (El.GetTag() != Tag.Content && !El.InnerOnlyImage() && (El.GetResize().Height < MinHeightInThisColumn || MinHeightInThisColumn == 0))
        //                    MinHeightInThisColumn = El.GetResize().Height;

        //            if (MinHeightInThisColumn == 0)
        //                foreach (var El in Element)
        //                    if (El.GetResize().Height != 0)
        //                        if (El.GetResize().Height < MinHeightInThisColumn || MinHeightInThisColumn == 0)
        //                            MinHeightInThisColumn = El.GetResize().Height;

        //            float SumOfWidth = 0;

        //            foreach (var El in Element)
        //            {
        //                GC.Collect();
        //                if (El.GetTag() == Tag.Content)
        //                {
        //                    SumOfWidth += El.GetImage().Size.ScaleSize(int.MaxValue, MinHeightInThisColumn).Width;
        //                }
        //            }

        //            foreach (var El in Element)
        //            {
        //                GC.Collect();
        //                if (El.GetTag() == Tag.Content)
        //                {
        //                    var a = (float)El.GetImage().Size.ScaleSize(int.MaxValue, MinHeightInThisColumn).Width / SumOfWidth;
        //                    var b = a * Limit.Width;
        //                    var img = El.GetImage().ScaleImage(b, int.MaxValue);

        //                    Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);

        //                    ReturnX = img.Width;
        //                    ReturnY = img.Height;
        //                    Pos.X += img.Width;
        //                }
        //            }

        //            return new RectangleF() { X = ReturnX, Y = ReturnY };
        //        }
        //        else
        //        {
        //            int ReturnY = 0;
        //            int ReturnX = 0;
        //            float MinHeightInThisRow = 0,
        //                PreviosBranches = 0;

        //            //берём по самому минимальному
        //            //foreach (var El in Element)
        //            //    if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage() && (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0))
        //            //        MinHeightInThisRow = El.GetResize().Height;

        //            //попробуем брать по самому внутреннему
        //            foreach (var El in Element)
        //                if (El.GetTag() != Tag.Content && !El.InnerOnlyImage())
        //                    if (El.GetTreeCount() > PreviosBranches)
        //                    {
        //                        PreviosBranches = El.GetTreeCount();
        //                        MinHeightInThisRow = El.GetResize().Height;
        //                    }

        //            //из самых внутренних определить какой меньший если у них одинаковый уровень вложенности
        //            var ElementsWithSameBranches = Element.Where(x => x.GetTag() != Tag.Content).Where(x => !x.InnerOnlyImage()).Where(x => x.GetTreeCount() == PreviosBranches);
        //            if (ElementsWithSameBranches.Count() > 1)
        //                MinHeightInThisRow = ElementsWithSameBranches.Min(x => x.GetResize().Height);

        //            if (MinHeightInThisRow == 0)
        //                foreach (var El in Element)
        //                    if (El.GetResize().Height != 0)
        //                        if (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0)
        //                            MinHeightInThisRow = El.GetResize().Height;

        //            foreach (var El in Element)
        //            {
        //                GC.Collect();
        //                if (El.GetTag() == Tag.Content)
        //                {
        //                    var img = El.GetImage();
        //                    img = img.ScaleImage(int.MaxValue, MinHeightInThisRow);

        //                    if (Element.CountInner() == 1)
        //                        img = img.ScaleImage(Previos.Width, int.MaxValue);

        //                    Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);
        //                    ReturnY = img.Height;
        //                    Pos.X += img.Width;
        //                    ReturnX += img.Width;
        //                }
        //                else
        //                {
        //                    Previos.Height = MinHeightInThisRow;

        //                    //определяем нужен ли лимит этой колонке
        //                    if (El.GetResize().Height != MinHeightInThisRow)
        //                        Limit.Height = MinHeightInThisRow;
        //                    else
        //                        Limit.Height = 0;

        //                    var returned = drawimages(El, Context, Pos, Previos, Limit);

        //                    if (Limit.Height == 0)
        //                    {

        //                        if (El.GetTag() == Tag.Column)
        //                            ReturnX += (int)El.GetResize().Width;//(int)returned.X;

        //                        Pos.X += El.GetResize().Width;//(int)returned.X;

        //                        //Pos.X += (int)returned.X;

        //                        ReturnY = (int)El.GetResize().Height;//(int)returned.Y;
        //                    }
        //                    else
        //                    {
        //                        if (El.GetTag() == Tag.Column)
        //                            ReturnX += (int)returned.X;

        //                        Pos.X += (int)returned.X;

        //                        //Pos.X += (int)returned.X;

        //                        ReturnY = (int)returned.Y;
        //                    }
        //                }
        //            }

        //            return new RectangleF() { Y = ReturnY, X = ReturnX };
        //        }
        //    }
        //    else if (Element.GetTag() == Tag.Column)
        //    {
        //        if (Limit.Height > 0)
        //        {
        //            int ReturnX = 0,
        //                ReturnY = 0;
        //            float MinWidthInThisColumn = 0;

        //            //берём по самому минимальному
        //            foreach (var El in Element)
        //                if (El.GetTag() != Tag.Content && !El.InnerOnlyImage() && (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0))
        //                    MinWidthInThisColumn = El.GetResize().Width;

        //            if (MinWidthInThisColumn == 0)
        //                foreach (var El in Element)
        //                    if (El.GetResize().Width != 0)
        //                        if (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0)
        //                            MinWidthInThisColumn = El.GetResize().Width;

        //            float SumOfHeight = 0;

        //            foreach (var El in Element)
        //            {
        //                GC.Collect();
        //                if (El.GetTag() == Tag.Content)
        //                {
        //                    SumOfHeight += El.GetImage().Size.ScaleSize(MinWidthInThisColumn, int.MaxValue).Height;
        //                }
        //            }

        //            foreach (var El in Element)
        //            {
        //                GC.Collect();
        //                if (El.GetTag() == Tag.Content)
        //                {
        //                    var a = (float)El.GetImage().Size.ScaleSize(MinWidthInThisColumn, int.MaxValue).Height / SumOfHeight;
        //                    var b = a * Limit.Height;
        //                    var img = El.GetImage().ScaleImage(int.MaxValue, b);

        //                    Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);

        //                    ReturnX = img.Width;
        //                    ReturnY = img.Height;
        //                    Pos.Y += img.Height;
        //                }
        //            }

        //            return new RectangleF() { X = ReturnX, Y = ReturnY };
        //        }
        //        else
        //        {

        //            int ReturnX = 0;
        //            int ReturnY = 0;
        //            float MinWidthInThisColumn = 0,
        //                PreviosBranches = 0;

        //            //берём по самому минимальному
        //            //foreach (var El in Element)
        //            //    if (El.GetTag() != ElementType.Content && !El.InnerOnlyImage() && (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0))
        //            //        MinWidthInThisColumn = El.GetResize().Width;

        //            //попробуем брать по самому внутреннему
        //            foreach (var El in Element)
        //                if (El.GetTag() != Tag.Content && !El.InnerOnlyImage())
        //                    if (El.GetTreeCount() > PreviosBranches)
        //                    {
        //                        PreviosBranches = El.GetTreeCount();
        //                        MinWidthInThisColumn = El.GetResize().Width;
        //                    }

        //            //из самых внутренних определить какой меньший если у них одинаковый уровень вложенности
        //            var ElementsWithSameBranches = Element.Where(x => x.GetTag() != Tag.Content).Where(x => !x.InnerOnlyImage()).Where(x => x.GetTreeCount() == PreviosBranches);
        //            if (ElementsWithSameBranches.Count() > 1)
        //                MinWidthInThisColumn = ElementsWithSameBranches.Min(x => x.GetResize().Width);

        //            if (MinWidthInThisColumn == 0)
        //                foreach (var El in Element)
        //                    if (El.GetResize().Width != 0)
        //                        if (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0)
        //                            MinWidthInThisColumn = El.GetResize().Width;

        //            foreach (var El in Element)
        //            {
        //                GC.Collect();
        //                if (El.GetTag() == Tag.Content)
        //                {
        //                    var img = El.GetImage();
        //                    img = img.ScaleImage(MinWidthInThisColumn, int.MaxValue);

        //                    if (Element.CountInner() == 1)
        //                        img = img.ScaleImage(int.MaxValue, Previos.Height);

        //                    Context.DrawImage(img, Pos.X, Pos.Y, img.Width, img.Height);
        //                    ReturnX = img.Width;

        //                    ReturnY += img.Height;

        //                    Pos.Y += img.Height;
        //                }
        //                else
        //                {
        //                    Previos.Width = MinWidthInThisColumn;

        //                    //определяем нужен ли лимит этой строке
        //                    if (El.GetResize().Width != MinWidthInThisColumn)
        //                        Limit.Width = MinWidthInThisColumn;
        //                    else
        //                        Limit.Width = 0;

        //                    var returned = drawimages(El, Context, Pos, Previos, Limit);

        //                    if (Limit.Width == 0)
        //                    {
        //                        if (El.GetTag() == Tag.Row)
        //                            ReturnY += (int)El.GetResize().Height;//returned.Y;

        //                        Pos.Y += (int)El.GetResize().Height;//returned.Y;
        //                        ReturnX += (int)El.GetResize().Width;//returned.X;
        //                    }
        //                    else
        //                    {
        //                        if (El.GetTag() == Tag.Row)
        //                            ReturnY += (int)returned.Y;

        //                        Pos.Y += (int)returned.Y;
        //                        ReturnX += (int)returned.X;
        //                    }
        //                }
        //            }

        //            return new RectangleF() { X = ReturnX, Y = ReturnY };
        //        }
        //    }

        //    return Pos;
        //}

        private static RectangleF? DrawElements(Element Element, Graphics Context, PointF Pos, SizeF Limit)
        {
            float MinimalSideSize = 0;

            foreach (var El in Element)
                if (El.InnerSideSize != 0)
                    if (Element.Tag == Tag.Row)
                    {
                        if (El.InnerSize.Height < MinimalSideSize || MinimalSideSize == 0)
                            MinimalSideSize = El.InnerSize.Height;
                    }
                    else
                        if (El.InnerSize.Width < MinimalSideSize || MinimalSideSize == 0)
                        MinimalSideSize = El.InnerSize.Width;

            float DrivenByLimitSumOfElements = 0;
            if (Element.IsLimited(Limit))
            {
                foreach (var El in Element)
                    if (Element.Tag == Tag.Row)
                        DrivenByLimitSumOfElements += El.DrivenSize.ScaleSize(int.MaxValue, Limit.Height).Width;
                    else
                        DrivenByLimitSumOfElements += El.DrivenSize.ScaleSize(Limit.Width, int.MaxValue).Height;
            }


            foreach (var El in Element)
            {
                if (El.Tag == Tag.Content)
                {
                    var Img = El.GetImage();

                    if (!Element.IsLimited(Limit))
                    {
                        if (Element.Tag == Tag.Row)
                            if (Img.Width != MinimalSideSize)
                                Img = Img.ScaleImage(int.MaxValue, MinimalSideSize);

                        if (Element.Tag == Tag.Column)
                            if (Img.Height != MinimalSideSize)
                                Img = Img.ScaleImage(MinimalSideSize, int.MaxValue);
                    }
                    else
                    {
                        if (Element.Tag == Tag.Row)
                        {
                            var a = Img.Size.ScaleSize(int.MaxValue, MinimalSideSize).Width / DrivenByLimitSumOfElements;
                            var b = a * Limit.Width;
                            Img = Img.ScaleImage(b, int.MaxValue);
                        }
                        if (Element.Tag == Tag.Column)
                        {
                            var a = Img.Size.ScaleSize(MinimalSideSize, int.MaxValue).Height / DrivenByLimitSumOfElements;
                            var b = a * Limit.Width;
                            Img = Img.ScaleImage(int.MaxValue, b);
                        }
                    }

                    Context.DrawImage(Img, Pos);

                    if (Element.Tag == Tag.Row)
                        Pos.X += Img.Width;
                    else
                        Pos.Y += Img.Height;
                }
                else
                {
                    if (Element.Tag == Tag.Row)
                    {
                        if (El.DrivenSize.Width != MinimalSideSize)
                            Limit.Width = MinimalSideSize;
                        else
                            Limit.Width = 0;
                    }
                    else
                    {
                        if (El.DrivenSize.Height != MinimalSideSize)
                            Limit.Height = MinimalSideSize;
                        else
                            Limit.Height = 0;
                    }

                    DrawElements(El, Context, Pos, Limit);
                }
            }

            return null;
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