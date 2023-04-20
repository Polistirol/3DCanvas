using ParserLibrary.Helpers;
using ParserLibrary.Interfaces.Macros;
using System;
using System.Collections.Generic;
using System.Linq;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Models
{
    public abstract class Macro : IMacro
    {
        public TechnoHelper.ELineType LineColor { get; set; }
        public abstract TechnoHelper.EEntityType EntityType { get; }
        public int SourceLine { get; set; }
        public bool IsBeamOn { get; set; }
        public bool Is2DProgram { get; set; }
        public string OriginalLine { get; set; }
        public int CheckScrap { get; set; }
        public int Repeat { get; set; }
        public Object Tag { get; set; }
        public BoundingBox BoundingBox { get; set; } = new BoundingBox();
        public  ToolpathEntity LeadIn
        {
            get
            {
                return Movements.FirstOrDefault( x => x.IsLeadIn );
            }

        }

        public virtual List<ToolpathEntity> Movements { get; set; }

        public void Render(Matrix3D U, Matrix3D Un, bool isRot, double Zradius)
        {
            foreach (var movement in Movements)
            {
                movement.Render(U, Un, isRot, Zradius);
            }
        }

    }
}
