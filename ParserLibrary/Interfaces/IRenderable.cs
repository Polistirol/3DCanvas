using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Interfaces
{
    public interface IRenderable
    {
        void Render(Matrix3D U, Matrix3D Un, bool isRot, double Zradius);

        BoundingBox BoundingBox { get; set; }


    }
}
