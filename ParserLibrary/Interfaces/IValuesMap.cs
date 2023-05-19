using ParserLibrary.Helpers;
using ParserLibrary.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Interfaces
{
    public  interface IValuesMap
    {
        EInstructionType InstructionType { get; set; }
        EInstructionMode? InstructionMode { get; set; }

        string[] AxisNedded { get; }
        string Name { get; set; }
        string OriginalLine { get; set; }   

        int SourceLine { get; set; }


        string GetAttribute(string name);

        void SetAttribute(string name, string value);


        double ToDouble(string name);
        Point3D PointFromString(string x, string y, string z);

        Vector3D VectorFromString(string x, string y, string z);


    }
}
