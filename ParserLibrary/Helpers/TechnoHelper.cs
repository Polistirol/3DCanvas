using System.Collections.Generic;
using System.Collections.Specialized;

namespace ParserLibrary.Helpers
{
    public static class TechnoHelper
    {
        public enum ELineType
        {
            CutLine1 = 1,
            CutLine2 = 2,
            CutLine3 = 3,
            CutLine4 = 4,
            CutLine5 = 5,
            Marking = 6,
            Microwelding = 7,
            Rapid = 8
        }

        public enum EPiercingType
        {
            Standard = 1,
            NoPiercing = 2,
            Quick =3,
            PreHole = 4,
            N2P = 5,
        }

        public enum ETraceLineColor
        {
            tracing = 0,
            tracedCutLine1 = 1,
            tracedCutLine2,
            tracedCutLine3,
            tracedCutLine4,
            tracedCutLine5,
        }

        public enum EEntityType
        {
            Line = 1,
            Arc,
            Circle,
            Slot,
            Rapid,
            Poly,
            Rect,
            Keyhole,
            Hole
        }

        public enum EInstructionType
        {
            Skippable = 0,
            Movement,
            Rototranslation,
            Process,
            Macro,
            Variable,
            Modal,
            Siemens,
            SubProgram
        }

        public enum EInstructionMode
        {

            Linear = 1,
            Arc3Points,
            ArcClockwise,
            ArcCounterClockwise,
            Hole,
            Slot,
            Rapid,
            Poly,
            Rect,
            Keyhole,
            AbsoluteProg,
            RelativeProg,
            MarkingPiece,
            LocalRotoTrasl,
            GlobalRotoTrasl,
            EndProgram,
            StartProcess,
            StopProcess,
            SubStart,
            SubEnd,
            SubCall


        }


    }
}
