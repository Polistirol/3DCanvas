using ParserLib.Helpers;
using ParserLib.Interfaces.Macros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ParserLib.Models.Macros
{
    public class HoleMoves : Macro, IHole
    {

        public double Radius
        {
            get
            {
                if (base.Movements[1] is CircularEntity Circle)
                {
                    return Circle.Radius;
                }

                return double.NaN;

            }
        }
        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Hole;

        public Point3D Center { get; set; }



    }


}
