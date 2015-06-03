using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp
{
    class Program
    {
        static Row r1 = new Row(),
                r2 = new Row(),
                r3 = new Row(),
                r4 = new Row(),
                r5 = new Row(),
                r6 = new Row(),
                r7 = new Row();

        static Column c1 = new Column(),
            c2 = new Column(),
            c3 = new Column(),
            c4 = new Column(),
            c5 = new Column();

        static Picture p1 = new Picture("0.jpg"),
               p2 = new Picture("1.jpg"),
               p3 = new Picture("2.jpg"),
               p4 = new Picture("3.jpg"),
               p5 = new Picture("4.jpg"),
               p6 = new Picture("5.jpg");

        static void Main(string[] args)
        {
            Canvas.Draw(MediumRow(), 3000, new Canvas.Padding(0, 0, 0, 0),x=>
            {
                if (x)
                    Console.WriteLine("Well done");
                else
                    Console.WriteLine("Some error! See Logs...");
            });

            Console.ReadLine();
        }

        static IElement EasyCol()
        {
            return c1.Add(p1).Add(p2).Add(p3).Add(p3).Add(p4).Add(p5).Add(p6);
        }

        static IElement MediumCol()
        {
            r1.Add(p1).Add(p2);
            r2.Add(p6).Add(p6);
            return c1.Add(p3).Add(r1).Add(r2).Add(p4);
        }

        static IElement EasyRow()
        {
            return r1.Add(p1).Add(p2).Add(p3).Add(p3).Add(p4).Add(p5).Add(p6);
        }

        static IElement MediumRow()
        {
            c1.Add(p1).Add(p2);
            c2.Add(p6).Add(p5);
            return r1.Add(p3).Add(c1).Add(c2).Add(p4);
        }
    }
}
