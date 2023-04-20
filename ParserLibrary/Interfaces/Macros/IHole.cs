using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLibrary.Models.Media;
using ParserLibrary.Models;

namespace ParserLibrary.Interfaces.Macros
{
    public interface IHole : IMacro
    {
        Point3D Center { get; set; }

        Point3D Normal { get; set; }
        double Radius { get; set; }

        int Smooth { get; set; }



    }
}
