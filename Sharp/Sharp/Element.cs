using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace Sharp
{
    public class Element : IEnumerable
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
        public Image Image
        {
            get
            {
                return InnerImg;
            }
        }
        private Queue<Element> InnerMarkup = new Queue<Element>();

        public Element Add(Element Element)
        {
            InnerMarkup.Enqueue(Element);
            return this;
        }

        public IEnumerable<Element> GetTree()
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

        public IEnumerator<Element> GetEnumerator()
        {
            return InnerMarkup.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected Tag _ElementType;
        public Tag GetTag()
        {
            return _ElementType;
        }

        public int CountInner()
        {
            return InnerMarkup.Count;
        }

        //public SizeF GetResize()
        //{
        //    if (_ElementType == Tag.Row)
        //    {
        //        float MinHeightInThisRow = 0,
        //            PreviosBranches = 0;

        //        //берём по минимальному
        //        //foreach (var El in this)
        //        //    if (El.GetTag() != ElementType.Content && (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0))
        //        //        MinHeightInThisRow = El.GetResize().Height;

        //        //попроуем брать по самому внутреннему
        //        foreach (var El in this)
        //            if (El.GetTag() != Tag.Content && !El.InnerOnlyImage())
        //                if (El.GetTreeCount() > PreviosBranches)
        //                {
        //                    PreviosBranches = El.GetTreeCount();
        //                    MinHeightInThisRow = El.GetResize().Height;
        //                }
                
        //        //из самых внутренних определить какой меньший если у них одинаковый уровень вложенности
        //        var ElementsWithSameBranches = this.Where(x => x.GetTag() != Tag.Content).Where(x => !x.InnerOnlyImage()).Where(x => x.GetTreeCount() == PreviosBranches);
        //        if (ElementsWithSameBranches.Count() > 1)
        //            MinHeightInThisRow = ElementsWithSameBranches.Min(x => x.GetResize().Height);

        //        if (MinHeightInThisRow == 0)
        //            foreach (var El in this)
        //                if (El.GetResize().Height < MinHeightInThisRow || MinHeightInThisRow == 0)
        //                    MinHeightInThisRow = El.GetResize().Height;

        //        float SumOfResizedWidth = 0,
        //            HeighOfAllImages = 0;

        //        foreach (var El in this)
        //        {
        //            if (El.GetTag() == Tag.Content)
        //            {
        //                var imgS = El.GetImage().Size;
        //                var img = imgS.ScaleSize(int.MaxValue, MinHeightInThisRow);                        
        //                SumOfResizedWidth += img.Width;
        //                HeighOfAllImages = img.Height;
        //            }
        //            else
        //            {
        //                var resize = El.GetResize();
        //                SumOfResizedWidth += resize.Width;
        //                HeighOfAllImages = resize.Height;
        //            }
        //        }

        //        return new SizeF(SumOfResizedWidth, HeighOfAllImages);
        //    }
        //    if (_ElementType == Tag.Column)
        //    {
        //        float MinWidthInThisColumn = 0,
        //            PreviosBranches=0;

        //        //берём по самому минимальному
        //        //foreach (var El in this)
        //        //    if (El.GetTag() != ElementType.Content && (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0))
        //        //        MinWidthInThisColumn = El.GetResize().Width;

        //        //попробуем брать по самому внутреннему
        //        foreach (var El in this)
        //            if (El.GetTag() != Tag.Content && !El.InnerOnlyImage())
        //                if (El.GetTreeCount() > PreviosBranches)
        //                {
        //                    PreviosBranches = El.GetTreeCount();
        //                    MinWidthInThisColumn = El.GetResize().Width;
        //                }
                
        //        //из самых внутренних определить какой меньший если у них одинаковый уровень вложенности
        //        var ElementsWithSameBranches = this.Where(x => x.GetTag() != Tag.Content).Where(x => !x.InnerOnlyImage()).Where(x => x.GetTreeCount() == PreviosBranches);
        //        if (ElementsWithSameBranches.Count() > 1)
        //            MinWidthInThisColumn = ElementsWithSameBranches.Min(x => x.GetResize().Width);

        //        if (MinWidthInThisColumn == 0)
        //            foreach (var El in this)
        //                if (El.GetResize().Width < MinWidthInThisColumn || MinWidthInThisColumn == 0)
        //                    MinWidthInThisColumn = El.GetResize().Width;

        //        float SumOfResizedHeight = 0,
        //            WidthOfAllImages = 0;

        //        foreach (var El in this)
        //            if (El.GetTag() == Tag.Content)
        //            {
        //                var imgS = El.GetImage().Size;
        //                var img = imgS.ScaleSize(MinWidthInThisColumn, int.MaxValue);
        //                WidthOfAllImages = img.Width;
        //                SumOfResizedHeight += img.Height;
        //            }
        //            else
        //            {
        //                var resize= El.GetResize();
        //                SumOfResizedHeight += resize.Height;
        //                WidthOfAllImages = resize.Width;
        //            }

        //        return new SizeF(WidthOfAllImages, SumOfResizedHeight);
        //    }
        //    if (_ElementType == Tag.Content)
        //    {
        //        return InnerImg.Size;
        //    }
        //    return new SizeF();
        //}

        public string Identificator;
        public string GetIdentificator()
        {
            return Identificator;
        }

        public Tag Tag
        {
            get { return _ElementType; }
        }

        public float InnerSideSize
        {
            get
            {
                if (Tag == Tag.Row)
                    return _InnerSize().Height;
                else
                    return _InnerSize().Width;
            }
        }
        public SizeF InnerSize
        {
            get
            {
                return _InnerSize();
            }
        }
        private SizeF _InnerSize()
        {
            SizeF AverageByMinimal = new SizeF();

            if (Tag == Tag.Content)
                AverageByMinimal = Image.Size;
            else
                foreach (var Element in this)
                    AverageByMinimal += Element._InnerSize();

            return AverageByMinimal;
        }

        /// <summary>
        /// Возвращает размер стороны к которой приводятся все элементы в данном элементе, для строки ширина будет равна нулю, для столбца высота
        /// </summary>
        public float LeadingSize
        {
            get
            {
                float MinimalSideSize = 0;

                foreach (var El in this)
                    if (El.InnerSideSize != 0)
                        if (Tag == Tag.Row)
                        {
                            if (El.InnerSize.Height < MinimalSideSize || MinimalSideSize == 0)
                                MinimalSideSize = El.InnerSize.Height;
                        }
                        else
                        {
                            if (El.InnerSize.Width < MinimalSideSize || MinimalSideSize == 0)
                                MinimalSideSize = El.InnerSize.Width;
                        }

                return MinimalSideSize;
            }            
        }

        /// <summary>
        /// Возвращает размер всего элемента приведённого к LeadingSize
        /// </summary>
        public SizeF DrivenSize
        {
            get
            {
                SizeF DrivenSummary = new SizeF();

                List<Element> TempCollection = new List<Element>(InnerMarkup);
                if (Image != null)
                    TempCollection.Add(new Picture(Image));

                foreach (var El in TempCollection)
                {
                    if (El.Tag == Tag.Content)
                    {
                        var Img = El.GetImage();
                        if (Tag == Tag.Row)
                        {
                            if (Img.Width != LeadingSize)
                                Img = Img.ScaleImage(int.MaxValue, LeadingSize);
                        }
                        else if (this.Tag == Tag.Column)
                            if (Img.Height != LeadingSize)
                                Img = Img.ScaleImage(LeadingSize, int.MaxValue);

                        if (Tag == Tag.Row)
                        {
                            DrivenSummary.Width += Img.Width;
                            DrivenSummary.Height = Img.Height;
                        }
                        else
                        {
                            DrivenSummary.Height += Img.Height;
                            DrivenSummary.Width = Img.Width;
                        }
                    }
                }

                return DrivenSummary;
            }
        }
    }

    public class Row : Element
    {
        public Row()
            : base()
        { _ElementType = Tag.Row; }
    }

    public class Column : Element
    {
        public Column()
            : base()
        { _ElementType = Tag.Column; }
    }

    public class Picture : Element
    {
        public Picture(string Image)
            : base(Image)
        { _ElementType = Tag.Content; }

        public Picture (Image Image)
            :base ()
        {
            this.InnerImg = Image;
        }
    }
}