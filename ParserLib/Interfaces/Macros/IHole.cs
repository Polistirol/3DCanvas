using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ParserLib.Models;

namespace ParserLib.Interfaces.Macros
{
    public interface IHole : IMacro
    {
        Point3D Center { get; set; }
        double Radius { get; }

    }
}
