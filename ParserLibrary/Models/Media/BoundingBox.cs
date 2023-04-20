using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLibrary.Models.Media
{
    public class BoundingBox
    {

        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public BoundingBox() { }
        public BoundingBox(double left, double right, double bottom, double top)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;

        }
    }
}
