using ParserLibrary.Interfaces;
using ParserLibrary.Models.Media;
using System;

namespace ParserLibrary.Models
{
    public class RotoTranslation : IRotoTranslation
    {



        public RotoTranslation() { }
       

        public Vector3D LocalTranslationComponents { get; set; } = new Vector3D( 0, 0, 0 );
        public Vector3D GlobalTranslationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D ActiveTranslationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D LocalRotationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D GlobalRotationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D ActiveRotationComponents { get; set; } = new Vector3D(0, 0, 0);
        
        

        public void UpdateRotation(Vector3D newRotation, bool isGlobal = true)
        {
            if (isGlobal) //G93
            {
                LocalRotationComponents = new Vector3D(0, 0, 0);
                GlobalRotationComponents = new Vector3D(newRotation.X, newRotation.Y, newRotation.Z);
            }
            else //G94
            {
                LocalRotationComponents = Vector3D.Add(LocalRotationComponents, newRotation);
            }
            ActiveRotationComponents = Vector3D.Add(LocalRotationComponents, GlobalRotationComponents);
        }

        public  Point3D RotatePoint( Point3D point)
        {
            Vector3D rotation = this.ActiveRotationComponents;
            double alpha = rotation.X * Math.PI / 180;
            double beta = rotation.Y * Math.PI / 180;
            double gamma = rotation.Z * Math.PI / 180;

            double x1 = point.X * Math.Cos(alpha) - point.Y * Math.Sin(alpha);
            double y1 = point.X * Math.Sin(alpha) + point.Y * Math.Cos(alpha);

            double x2 = x1 * Math.Cos(beta) - point.Z * Math.Sin(beta);
            double z2 = x1 * Math.Sin(beta) + point.Z * Math.Cos(beta);

            double y3 = y1 * Math.Cos(gamma) - z2 * Math.Sin(gamma);
            double z3 = x1 * Math.Sin(gamma) + z2 * Math.Cos(gamma);

            return new Point3D(x2, y3, z3);

        }

        public Vector3D RotateVector_Euler(Vector3D vector, Vector3D eulerComponents)
        {
            double alpha = eulerComponents.X * Math.PI / 180;
            double beta = eulerComponents.Y * Math.PI / 180;
            double gamma = eulerComponents.Z * Math.PI / 180;

            double x1 = vector.X * Math.Cos(alpha) - vector.Y * Math.Sin(alpha);
            double y1 = vector.X * Math.Sin(alpha) + vector.Y * Math.Cos(alpha);

            double x2 = x1 * Math.Cos(beta) - vector.Z * Math.Sin(beta);
            double z2 = x1 * Math.Sin(beta) + vector.Z * Math.Cos(beta);

            double y3 = y1 * Math.Cos(gamma) - z2 * Math.Sin(gamma);
            double z3 = x1 * Math.Sin(gamma) + z2 * Math.Cos(gamma);
            return new Vector3D(x2, y3, z3);

        }


    



    public void UpdateTranslation(Vector3D newTranslation, bool isGlobal = true)
        {
            if (isGlobal) //G93
            {               
                LocalTranslationComponents = new Vector3D(0, 0, 0);
                GlobalTranslationComponents = new Vector3D(newTranslation.X, newTranslation.Y, newTranslation.Z);
            }
            else //G94
            {
                    LocalTranslationComponents = Vector3D.Add(  LocalTranslationComponents , newTranslation);
                    var point = new Point3D(0,0,1);
                    point = RotatePoint(point);

            }
            ActiveTranslationComponents = Vector3D.Add(LocalTranslationComponents,GlobalTranslationComponents);

            
        }

        public void ResetLocalRotoTranslation()
        {
            LocalTranslationComponents = new Vector3D(0, 0, 0);
            LocalRotationComponents = new Vector3D(0, 0, 0);
        }

        public void UpdateRotoTraslComponents(Vector3D newRotation, Vector3D newTranslation, bool isGlobal = true)
        {
            if (isGlobal) 
            {
                ResetLocalRotoTranslation()
;               GlobalRotationComponents = new Vector3D(newRotation.X, newRotation.Y, newRotation.Z);
                GlobalTranslationComponents = new Vector3D(newTranslation.X, newTranslation.Y, newTranslation.Z);        
            }
            else
            {
                //g94 empty
                if(Vector3D.Equals(newTranslation, new Vector3D(0,0,0)) 
                    && Vector3D.Equals(newRotation, new Vector3D(0, 0, 0)) )
                {
                    ResetLocalRotoTranslation();
                }
                else
                {
                    LocalRotationComponents = Vector3D.Add(LocalRotationComponents, newRotation);
                    LocalTranslationComponents = Vector3D.Add(LocalTranslationComponents, newTranslation);
                     LocalTranslationComponents = RotateVector_Euler(LocalTranslationComponents, ActiveRotationComponents);
                   
                        }
            }
            ActiveTranslationComponents = Vector3D.Add(LocalTranslationComponents, GlobalTranslationComponents);
            ActiveRotationComponents = Vector3D.Add(LocalRotationComponents, GlobalRotationComponents);

        }
    }
}
