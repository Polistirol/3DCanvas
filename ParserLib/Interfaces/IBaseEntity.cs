using System.Windows.Media.Media3D;
using static ParserLib.Helpers.TechnoHelper;

namespace ParserLib.Interfaces
{
    public interface IBaseEntity : IRenderable
    {
        ELineType LineColor { get; set; }//Line color is an int that rappresent the integer value of the line 1:Green 2:Blue 3:Red

        EEntityType EntityType { get; }

        int SourceLine { get; set; }//Line source from original file

        bool IsBeamOn { get; set; }

        bool Is2DProgram { get; set; }

        string OriginalLine { get; set; }

        object Tag { get; set; }
        
    }
}