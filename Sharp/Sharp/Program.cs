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
        static void Main(string[] args)
        {
            Row r1 = new Row(),
                r2 = new Row();

            Column c1 = new Column(),
                c2 = new Column();

            Picture p1 = new Picture("0.jpg"),
                   p2 = new Picture("1.jpg"),
                   p3 = new Picture("2.jpg"),
                   p4 = new Picture("3.jpg"),
                   p5 = new Picture("4.jpg"),
                   p6 = new Picture("5.jpg");

            r1.Add(p1).Add(c1).Add(p3);
            c1.Add(p2).Add(p4).Add(r2);
            r2.Add(p5).Add(c2).Add(p6);
            c2.Add(p1).Add(p3);

            Canvas.Draw(r1, 3000, new Canvas.Padding(0, 0, 0, 0),x=>
            {
                if (x)
                    Console.WriteLine("Well done");
                else
                    Console.WriteLine("Some error! See Logs...");
            });

            Console.ReadLine();
        }
    }
}
