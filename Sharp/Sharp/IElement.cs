using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Sharp
{
    public interface IElement : IEnumerable<IElement>
    {
        IElement Add(IElement Element);
        Image GetImage();
        Size GetMaxSize();
        Size GetMinSize();
        Size GetSize();
        string GetIdentificator();
        SizeF GetResize();
        ElementType GetTag();
        IEnumerable<IElement> GetTree();
        int CountInner();
    }

    public enum ElementType
    {
        Content = 0,
        Row,
        Column,
        Unknown
    }
}