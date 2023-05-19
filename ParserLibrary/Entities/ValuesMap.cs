using ParserLibrary.Helpers;
using ParserLibrary.Interfaces;
using ParserLibrary.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Entities
{
    public class ValuesMap : IValuesMap
    {
        public EInstructionType InstructionType { get; set; }

        public EInstructionMode? InstructionMode { get; set; } 
        public string OriginalLine { get; set; }
        public string Name { get; set; }
        public int SourceLine { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string I { get; set; }
        public string J { get; set; }
        public string K { get; set; }
        public string U { get; set; }
        public string V { get; set; }
        public string W { get; set; }

        public string Alpha    { get; set; }
        public string Beta     { get; set; }
        public string Gamma    { get; set; }

        public Vector3D RotationVector { 
            get { return VectorFromString(Alpha, Beta, Gamma); }
            set
            {

            }
                } 
        public Vector3D TranslationVector => VectorFromString(X,Y,Z);


        public string[] AxisNedded => InstructionsMap.GetAxisNeeded(InstructionMode);


        #region reflection getters 
        //public string GetAxe(string axe)
        //{
        //    return this.GetType().GetProperty(axe).GetValue(this).ToString();
        //}

        //public void SetAxe(string axe,string value)
        //{
        //    var prop = this.GetType().GetProperty(axe);
        //    if (prop != null)
        //    {
        //        prop.SetValue(this, value);
        //    }
        //}
        #endregion
        public string GetAttribute(string name)
        {
            switch (name.ToUpper())
            {
                case "X": return X;
                case "Y": return Y;
                case "Z": return Z;
                case "A": return A;
                case "B": return B;
                case "C": return C;                
                case "I": return I;
                case "J": return J;
                case "K": return K;                
                case "U": return U;
                case "V": return V;
                case "W": return W;             
                case "ALPHA":   return Alpha  ;        
                case "BETA":    return Beta   ;        
                case "GAMMA":   return Gamma  ;        
    
                default:
                    return null;// throw new NotImplementedException("Axe not Found!");
            }  
        }

        public void SetAttribute(string name, string value)
        {
            switch (name.ToUpper())
            {
                case "X":X=value;break;
                case "Y":Y=value;break;
                case "Z":Z=value;break;
                case "A":A=value;break;
                case "B":B=value;break;
                case "C":C=value;break;
                case "I":I=value;break;
                case "J":J=value;break;
                case "K":K=value;break;
                case "U":U=value;break;
                case "V":V=value;break;
                case "W":W=value;break;
                case "ALPHA":Alpha=value ;break;
                case "BETA": Beta=value ;break;   
                case "GAMMA":Gamma=value ;break;  
                                         
                default:
                    break;// throw new NotImplementedException("Axe not Found!");
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

    }
}
