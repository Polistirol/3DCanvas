using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ParserLib.Interfaces
{
    public interface IRenderable
    {
        void Render(Matrix3D U, Matrix3D Un, bool isRot, double Zradius);

        Tuple<double, double, double, double> BoundingBox { get; }


    }
}
