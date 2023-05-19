using ParserLibrary.Helpers;
using ParserLibrary.Interfaces;
using ParserLibrary.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Entities
{
    public class MacroValueMap : IValuesMap
    {
        public EInstructionType InstructionType { get; set; }

        public EInstructionMode? InstructionMode { get; set; }

        public string[] AxisNedded => InstructionsMap.GetAxisNeeded(InstructionMode);
        public string Name { get; set; }
        public int SourceLine { get; set; }
        public string OriginalLine { get; set; }

        public string Radius { get; set; }

        public string BigRadius { get; set; }
        public string SmallRadius { get; set; }

        public string XC1 { get; set; }
        public string YC1 { get; set; }
        public string ZC1 { get; set; }

        public string XC2 { get; set; }
        public string YC2 { get; set; }
        public string ZC2 { get; set; }

        public string XC3 { get; set; }
        public string YC3 { get; set; }
        public string ZC3 { get; set; }

        public string XN { get; set; }
        public string YN { get; set; }
        public string ZN { get; set; }

        public string XS1 { get; set; }
        public string YS1 { get; set; }
        public string ZS1 { get; set; }
        
        public string XV1 { get; set; }
        public string YV1 { get; set; }
        public string ZV1 { get; set; }

        public string XV2 { get; set; }
        public string YV2 { get; set; }
        public string ZV2 { get; set; }
        public string Sides { get; set; }

        public Point3D Normal => PointFromString(XN, YN, ZN);
        public Point3D Center1 => PointFromString(XC1, YC1, ZC1);
        public Point3D Center2 => PointFromString(XC2, YC2, ZC2);
        public Point3D Vertex1 => PointFromString(XV1, YV1, ZV1);
        public Point3D Vertex2 => PointFromString(XV2, YV2, ZV2);
        public Point3D Side1 => PointFromString(XS1, YS1, ZS1);



        public string GetAttribute(string axis)
        {
            switch (axis.ToUpper())
            {
                case "RADIUS": return Radius;
                case "BIGRADIUS": return BigRadius;
                case "SMALLRADIUS": return SmallRadius;
                case "SIDES":return Sides;
                case "XC1": return XC1;
                case "YC1": return YC1;
                case "ZC1": return ZC1; 
                case "XC2": return XC2;
                case "YC2": return YC2;
                case "ZC2": return ZC2;
                case "XC3": return XC3;
                case "YC3": return YC3;
                case "ZC3": return ZC3;
                case "XN": return XN ;
                case "YN": return YN ;
                case "ZN": return ZN ;
                case "XS1": return XS1;
                case "YS1": return YS1;
                case "ZS1": return ZS1;
                case "XV1": return XV1;
                case "YV1": return YV1;
                case "ZV1": return ZV1;
                case "XV2": return XV2;
                case "YV2": return YV2;
                case "ZV2": return ZV2;
                default:
                    return null; // throw new NotImplementedException("Macro Parameter Not Valid!");    
            }
        }

        public double ToDouble(string name)
        {
            if (double.TryParse(name, out double nameD))
            {
                return nameD;
            }
            else
                throw new InvalidCastException("Could not convert axes value to double");
        }

        public Point3D PointFromString(string x, string y, string z)
        {
            if (double.TryParse(x, out double xd) && double.TryParse(y, out double yd) && double.TryParse(z, out double zd))
            {
                return new Point3D(xd, yd, zd);
            }
            else
                throw new InvalidCastException("Could not convert axes value to double");
        }

        public Vector3D VectorFromString(string x, string y, string z)
        {
            if (double.TryParse(x, out double xd) && double.TryParse(y, out double yd) && double.TryParse(z, out double zd))
            {
                return new Vector3D(xd, yd, zd);
            }
            else
                throw new InvalidCastException("Could not convert axes value to double");
        }
        public void SetAttribute(string axis, string value)
        {
            switch (axis.ToUpper())
            {
                case "RADIUS": Radius = value; break;
                case "BIGRADIUS": BigRadius = value; break;
                case "SMALLRADIUS": SmallRadius = value; break;
                case "SIDES": Sides = value; break;
                case "XC1":  XC1=value;break;
                case "YC1":  YC1=value;break;
                case "ZC1":  ZC1=value;break;
                case "XC2":  XC2=value;break;
                case "YC2":  YC2=value;break;
                case "ZC2":  ZC2=value;break;
                case "XC3":  XC3=value;break;
                case "YC3":  YC3=value;break;
                case "ZC3":  ZC3=value;break;
                case "XN":  XN= value;break;
                case "YN":  YN= value;break;
                case "ZN":  ZN= value;break;
                case "XS1":  XS1=value;break;
                case "YS1":  YS1=value;break;
                case "ZS1":  ZS1=value;break;
                case "XV1":  XV1=value;break;
                case "YV1":  YV1=value;break;
                case "ZV1":  ZV1=value;break;
                case "XV2":  XV2=value;break;
                case "YV2":  YV2=value;break;
                case "ZV2":  ZV2=value;break;
                default:
                    break;// throw new NotImplementedException("Macro Parameter Not Valid!");
            }
        }
    }
}
