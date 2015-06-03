using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace Sharp
{
    public class Element : IElement
    {
        public Element()
        { }

        public Element(string PathToImage)
        {
            try
            {
                InnerImg = Image.FromFile(PathToImage);
            }
            catch { }
        }

        public Image InnerImg = null;
        private Queue<IElement> InnerMarkup = new Queue<IElement>();

        public IElement Add(IElement Element)
        {
            InnerMarkup.Enqueue(Element);
            return this;
        }

        public IEnumerable<IElement> GetTree()
        {
            return InnerMarkup.SelectMany(c => c.GetTree()).Concat(new[] { this });
        }

        public Size GetMaxSize()
        {
            return GetTree().OrderByDescending(x => (x as Element).InnerImg?.Height).Select(x => x.GetImage()?.Size ?? new Size(0,0)).FirstOrDefault();
        }

        public Size GetMinSize()
        {
            return GetTree().OrderBy(x => (x as Element).InnerImg?.Height).Where(x=>x.GetImage()!=null).Select(x => x.GetImage()?.Size ?? new Size(0, 0)).FirstOrDefault();
        }

        public Size GetSize()
        {
            return (from x
                   in GetTree().Select(x => x.GetImage()?.Size).ToList()
                    group x by 1
                   into y
                    select new Size(y.Sum(z => z.HasValue ? z.Value.Width : 0), y.Sum(z => z.HasValue ? z.Value.Height : 0)))
                   .FirstOrDefault();
        }

        #region hide
        //public Size GetResize(SizeF Max,bool Scale=false)
        //{
        //    Size Result = new Size();

        //    if (this._ElementType == ElementType.Row)
        //    {
        //        var MinHeightInThisRow = 0;
        //        float SumResizedWidth = 0;

        //        foreach (var El in this)
        //            if (El.GetResize(Max).Height < MinHeightInThisRow || MinHeightInThisRow == 0)
        //                MinHeightInThisRow = El.GetResize(Max).Height;

        //        foreach (var El in this)
        //            SumResizedWidth += El.Resized(int.MaxValue, MinHeightInThisRow).Height;

        //        //var Multiple = Max.Height / SumResizedHeight;

        //        foreach (var El in this)
        //        {
        //            Image Img = new Bitmap(El.GetResize(Max).Width, El.GetResize(Max).Height);

        //            if (El.GetTag() == ElementType.Content)
        //                Img = El.GetImage();

        //            Img = Img.ScaleImage(int.MaxValue, MinHeightInThisRow);
        //            Img = Img.ScaleImage(Img.Width, int.MaxValue);

        //            if (El.GetTag() == ElementType.Content)
        //            {
        //                Result.Height = Img.Height;
        //                Result.Width += Img.Width;
        //            }

        //            if (El.GetTag() == ElementType.Row)
        //            {
        //                Result.Width += El.GetResize(Max).Width;
        //            }
        //        }
        //    }
        //    else if (_ElementType == ElementType.Column)
        //    {
        //        var MinWidthInThisColumn = 0;
        //        float SumResizedHeight = 0;

        //        foreach (var El in this)
        //            if (El.GetResize(Max).Height < MinWidthInThisColumn || MinWidthInThisColumn == 0)
        //                MinWidthInThisColumn = El.GetResize(Max).Height;

        //        foreach (var El in this)
        //            SumResizedHeight += El.Resized(int.MaxValue, MinWidthInThisColumn).Height;

        //        //var Multiple = Max.Height / SumResizedHeight;

        //        foreach (var El in this)
        //        {
        //            Image Img = new Bitmap(El.GetResize(Max).Width, El.GetResize(Max).Height);

        //            if (El.GetTag() == ElementType.Content)
        //                Img = El.GetImage();

        //            Img = Img.ScaleImage(MinWidthInThisColumn, int.MaxValue);
        //            Img = Img.ScaleImage(int.MaxValue, Img.Height);

        //            if (El.GetTag() == ElementType.Content)
        //            {
        //                Result.Height += Img.Height;
        //                Result.Width = Img.Width;
        //            }

        //            if (El.GetTag() == ElementType.Row)
        //            {
        //                Result.Height += El.GetResize(Max).Height;
        //            }
        //        }
        //    }
        //    else if (_ElementType == ElementType.Content)
        //    {
        //        if (Scale)
        //            Result = InnerImg.ScaleImage(Max.Width, Max.Height).Size;
        //        else
        //            Result = InnerImg.Size;
        //    }

        //    return Result;
        //}
        #endregion

        public Image GetImage()
        {
            return InnerImg;
        }

        public IEnumerator<IElement> GetEnumerator()
        {
            return InnerMarkup.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected ElementType _ElementType;
        public ElementType GetTag()
        {
            return _ElementType;
        }

        public int CountInner()
        {
            return InnerMarkup.Count;
        }

        public SizeF GetResize()
        {
            if (_ElementType == ElementType.Row)
            {
                float MinHeightInThisRow = 0;

                foreach (var El in this)
                    if (El.GetTag() != ElementType.Content && (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0))
                        MinHeightInThisRow = El.GetResize().Height;

                if (MinHeightInThisRow == 0)
                    foreach (var El in this)
                        if (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0)
                            MinHeightInThisRow = El.GetResize().Height;

                float SumOfResizedWidth = 0,
                    HeighOfAllImages = 0;

                foreach (var El in this)
                {
                    if (El.GetTag() == ElementType.Content)
                    {
                        var img = El.GetImage();
                        img = img.ScaleImage(int.MaxValue, MinHeightInThisRow);                        
                        SumOfResizedWidth += img.Width;
                        HeighOfAllImages = img.Height;
                    }
                    else
                    {
                        var resize = El.GetResize();
                        SumOfResizedWidth += resize.Width;
                        HeighOfAllImages = resize.Height;
                    }
                }

                return new SizeF(SumOfResizedWidth, HeighOfAllImages);
            }
            if (_ElementType == ElementType.Column)
            {
                float MinWidthInThisColumn = 0;

                foreach (var El in this)
                    if (El.GetTag() != ElementType.Content && (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0))
                        MinWidthInThisColumn = El.GetResize().Width;

                if (MinWidthInThisColumn == 0)
                    foreach (var El in this)
                        if (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0)
                            MinWidthInThisColumn = El.GetResize().Width;

                float SumOfResizedHeight = 0,
                    WidthOfAllImages = 0;

                foreach (var El in this)
                    if (El.GetTag() == ElementType.Content)
                    {
                        var img = El.GetImage();
                        img = img.ScaleImage(MinWidthInThisColumn, int.MaxValue);
                        WidthOfAllImages = img.Width;
                        SumOfResizedHeight += img.Height;
                    }
                    else
                    {
                        var resize= El.GetResize();
                        SumOfResizedHeight += resize.Height;
                        WidthOfAllImages = resize.Width;
                    }

                return new SizeF(WidthOfAllImages, SumOfResizedHeight);
            }
            if (_ElementType == ElementType.Content)
            {
                return InnerImg.Size;
            }
            return new SizeF();
        }
    }

    public class Row : Element
    {
        public Row()
            : base()
        { _ElementType = ElementType.Row; }
    }

    public class Column : Element
    {
        public Column()
            : base()
        { _ElementType = ElementType.Column; }
    }

    public class Picture : Element
    {
        public Picture(string Image)
            : base(Image)
        { _ElementType = ElementType.Content; }
    }
}