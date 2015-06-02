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