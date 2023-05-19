using ParserLibrary.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Helpers
{



    public  class InstructionsMap
    {

        //public Dictionary<EInstructionType, string[]> Categories = new Dictionary<EInstructionType, string[]>() {
        //    { EInstructionType.Movement, new string [] { "G00", "G01", "G104", "APPROACH_MC", "RETRACT_MC","G02","G03" ,} },
        //    { EInstructionType.Rototranslation, new string[]{ "G93", "G94", "G113", "G96"  } },
        //    { EInstructionType.Process, new string[]{ "WORK_ON","WORK_OFF","MARKING_PIECE" } },
        //    { EInstructionType.Modal, new string[]{ "G90","G91" } },
        //    { EInstructionType.Macro, new string[]{ "HOLE","SLOT","RECT","POLY","KEYHOLE" } },
        //    { EInstructionType.Variable, new string[]{ "LOC","VAR" } },
        //};

        //public Dictionary<EEntityType, string[]> EntityTypes = new Dictionary<EEntityType, string[]>() {
        //    {EEntityType.Line, new string [5] { "G00", "G01", "APPROACH_MC", "RETRACT_MC", "LIN_MOVE" } },
        //    {EEntityType.Arc, new string [3] { "G104", "G02", "G03" } },
        //    {EEntityType.Hole, new string [1] { "HOLE",  } },
        //    {EEntityType.Slot, new string [1] { "SLOT", } },
        //    {EEntityType.Rect, new string [1] { "RECT", } },
        //    {EEntityType.Poly, new string [1] { "POLY", } },
        //    {EEntityType.Keyhole, new string [1] { "KEYHOLE", } },
        //};


        public static string[] LinearAxisNeeded => new string[3] { "X", "Y", "Z" };
        public static string[] CircularAxisNeeded => new string[6] { "X", "Y", "Z","I","J","K" };
        public static string[] HoleAxisNeeded => new string[7] { "XC1", "YC1", "ZC1","XN","YN","ZN","Radius" };
        public static string[] SlotAxisNeeded => new string[10] { "XC1", "YC1", "ZC1", "XC2", "YC2", "ZC2","XN","YN","ZN","Radius" };
        public static string[] RectAxisNeeded => new string[9] { "XC1", "YC1", "ZC1", "XV1", "YV1", "ZV1","XS1","YS1","ZS1" };
        public static string[] PolyAxisNeeded => new string[10] { "XC1", "YC1", "ZC1", "XV1", "YV1", "ZV1", "XN", "YN", "ZN", "Sides" };
        public static string[] KeyholeAxisNeeded => new string[11] { "XC1", "YC1", "ZC1", "XC2", "YC2", "ZC2","XN","YN","ZN" , "BigRadius", "SmallRadius" };
        public static string[] RototraslationAxisNeeded => new string[6] { "X", "Y", "Z", "Alpha", "Beta", "Gamma" };



        public static string[] GetAxisNeeded(EInstructionMode? InstructionMode)
        {
            switch (InstructionMode)
            {
                case EInstructionMode.Linear:
                    return LinearAxisNeeded;

                case EInstructionMode.Arc3Points:
                case EInstructionMode.ArcClockwise:
                case EInstructionMode.ArcCounterClockwise:
                    return CircularAxisNeeded;

                case EInstructionMode.Hole:
                    return HoleAxisNeeded;

                case EInstructionMode.Slot:
                    return SlotAxisNeeded;

                case EInstructionMode.Rect:
                    return RectAxisNeeded;

                case EInstructionMode.Poly: 
                    return PolyAxisNeeded;

                case EInstructionMode.Keyhole:
                    return KeyholeAxisNeeded;

                case EInstructionMode.GlobalRotoTrasl:
                case EInstructionMode.LocalRotoTrasl:
                    return RototraslationAxisNeeded;

            }
            throw new NotImplementedException("Axis Needed List could not be found! Invalid Instruction Mode !");
            

        }
    }
    
}
