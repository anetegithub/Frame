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