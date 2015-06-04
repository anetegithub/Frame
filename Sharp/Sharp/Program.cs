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
               p6 = new Picture("5.jpg"),
                p7 = new Picture("6.jpg");

        static void Main(string[] args)
        {
            Canvas.Draw(Example2(),3000, new Canvas.Padding(0, 0, 0, 0),x=>
            {
                if (x)
                    Console.WriteLine("Well done");
                else
                    Console.WriteLine("Some error! See Logs...");
            });

            Console.ReadLine();
        }

        /// <summary>
        /// +
        /// </summary>
        /// <returns></returns>
        static IElement EasyCol()
        {
            return c1.Add(p1).Add(p2).Add(p3).Add(p3).Add(p4).Add(p5).Add(p6);
        }

        /// <summary>
        /// +
        /// </summary>
        /// <returns></returns>
        static IElement MediumCol()
        {
            r1.Add(p1).Add(p2);
            r2.Add(p6).Add(p6);
            return c1.Add(p3).Add(r1).Add(r2).Add(p4);
        }

        /// <summary>
        /// +
        /// </summary>
        /// <returns></returns>
        static IElement EasyRow()
        {
            return r1.Add(p1).Add(p2).Add(p3).Add(p3).Add(p4).Add(p5).Add(p6);
        }

        /// <summary>
        /// +
        /// </summary>
        /// <returns></returns>
        static IElement MediumRow()
        {
            c1.Add(p4).Add(p2).Add(r2);
            r2.Add(p6).Add(p5).Add(c2);
            c2.Add(p3).Add(p6);

            return r1.Add(p5).Add(c1).Add(p4);
        }

        /// <summary>
        /// +
        /// </summary>
        /// <returns></returns>
        static IElement Example1()
        {
            r1.Add(p1).Add(c1).Add(p5);
            c1.Add(r2).Add(p3);
            r2.Add(p4).Add(c2);
            c2.Add(r3).Add(p2);
            r3.Add(p6).Add(p3);

            return r1;
        }

        /// <summary>
        /// -
        /// </summary>
        /// <returns></returns>
        static IElement Example1HALF()
        {
            r1.Add(p1).Add(c1).Add(c2).Add(p5);

            c1.Add(p1).Add(p2).Add(p3);
            c2.Add(c3).Add(p2);

            c3.Add(p7).Add(p7);

            return r1;
        }

        /// <summary>
        /// -
        /// </summary>
        /// <returns></returns>
        static IElement Example2()
        {
            r1.Add(p1).Add(c1).Add(c2).Add(p5);
            c2.Add(p6).Add(p5);
            c1.Add(r2).Add(r3).Add(p6);
            r2.Add(c3).Add(p2);
            c3.Add(r4).Add(p1);
            r4.Add(p1).Add(p1);
            r3.Add(p2).Add(c4).Add(c5);
            c4.Add(r5).Add(p3).Add(r6);
            r5.Add(p2).Add(p3).Add(p3);
            r6.Add(p4).Add(p4);
            c5.Add(r7).Add(p6);
            r7.Add(p5).Add(p5);

            return r1;
        }
    }
}


//r1.Add(p1).Add(c1).Add(c2).Add(p5);
//c2.Add(p6).Add(p5);
//c1.Add(r2).Add(r3).Add(p6);
//r2.Add(c3).Add(p2);
//c3.Add(r4).Add(p1);
//r4.Add(p1).Add(p1);
//r3.Add(p2).Add(c4).Add(c5);
//c4.Add(r5).Add(p3).Add(r6);
//r5.Add(p2).Add(p3).Add(p3);
//r6.Add(p4).Add(p4);
//c5.Add(r7).Add(p6);
//r7.Add(p5).Add(p5);