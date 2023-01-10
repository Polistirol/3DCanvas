using ParserLib.Helpers;
using ParserLib.Interfaces.Macros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ParserLib.Models
{
    public abstract class Macro : IMacro
    {
        //public abstract ToolpathEntity LeadIn { get; set; }
        public TechnoHelper.ELineType LineColor { get; set; }
        public abstract TechnoHelper.EEntityType EntityType { get; }
        public int SourceLine { get; set; }
        public bool IsBeamOn { get; set; }
        public bool Is2DProgram { get; set; }
        public string OriginalLine { get; set; }

        public  ToolpathEntity LeadIn
        {
            get
            {
                return Movements.FirstOrDefault( x => x.IsLeadIn );
            }

        }

        public virtual List<ToolpathEntity> Movements { get; set; }
        //List<ToolpathEntity> IMacro.Movements { get; }

        public void Render(Matrix3D U, Matrix3D Un, bool isRot, double Zradius)
        {
            foreach (var movement in Movements)
            {
                movement.Render(U, Un, isRot, Zradius);
            }
        }

        public  Tuple<double, double, double, double> BoundingBox
        {
            get
            {
                double xMin = double.PositiveInfinity;
                double xMax = double.NegativeInfinity;
                double yMin = double.PositiveInfinity;
                double yMax = double.NegativeInfinity;

                foreach (var item in Movements)
                {
                    xMin = Math.Min(item.GeometryPath.Bounds.Left, xMin);
                    xMax = Math.Max(item.GeometryPath.Bounds.Right, xMax);
                    yMin = Math.Min(item.GeometryPath.Bounds.Bottom, yMin);
                    yMax = Math.Max(item.GeometryPath.Bounds.Top, yMax);
                }
                return new Tuple<double, double, double, double>(xMin, xMax, yMin, yMax);
            }
        }
    }
}
