using System.Collections.Generic;
using ParserLibrary.Models.Media;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Interfaces
{
    public interface IProgramContext
    {
        IToolpathEntity ReferenceMove { get; set; }
        IToolpathEntity LastEntity { get; set; }
        Point3D CenterRotationPoint { get; set; }
        Point3D LastHeadPosition { get; set; }

        IRotoTranslation RotoTranslation { get; set; }

        int TextLineCount { get; set; } 
        
        int FirstSourceLine { get; }
        int LastSourceLine { get; }
        bool Is3DProgram { get; set; }
        bool Is2DProgram { get; set; }
        bool IsTubeProgram { get; set; }
        bool IsWeldProgram { get; set; }
        bool IsIncremental { get; set; }
        bool IsInchProgram { get; set; }
        bool InMainProgram { get; set; }
        bool IsBeamOn { get; set; }
        bool IsMarkingProgram { get; set; }
        int SourceLine { get; set; }
        ELineType ContourLineType { get; set; }

        IList<IBaseEntity> Moves { get; set; }

        void UpdateProgramCenterPoint();

    }
}