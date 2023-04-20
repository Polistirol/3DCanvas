using ParserLibrary.Models;
using System;
using System.Collections.Generic;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Helpers
{
    public static class RototranslationHelper
    {


        public static void AddTranslation( ref List<Point3D> points,ProgramContext programContext)
        {

            Vector3D components = programContext.RotoTranslation.ActiveTranslationComponents;
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = Point3D.Add(points[i], components);
            }

        }        
        public static Point3D TranslatePoint( Point3D point,ProgramContext programContext)
        {
             return TranslatePoint(point,programContext.RotoTranslation.ActiveTranslationComponents );
        }
        public static Point3D TranslatePoint(Point3D point, Vector3D vector)
        {
            return Point3D.Add(point, vector);
        }
        public static Point3D RotatePoint(ProgramContext programContext, Point3D point)
        {
            Vector3D rotation = programContext.RotoTranslation.ActiveRotationComponents;
            double alpha = rotation.X * Math.PI / 180;
            double beta = rotation.Y * Math.PI / 180;
            double gamma = rotation.Z * Math.PI / 180;

            double x1 = point.X * Math.Cos(alpha)- point.Y*Math.Sin(alpha);
            double y1 = point.X * Math.Sin(alpha) + point.Y*Math.Cos(alpha);
            
            double x2 = x1 *Math.Cos(beta)- point.Z*Math.Sin(beta);
            double z2 = x1 *Math.Sin(beta)+ point.Z*Math.Cos(beta);

            double y3 = y1 *Math.Cos(gamma)- z2*Math.Sin(gamma);
            double z3 = x1 *Math.Sin(gamma) + z2*Math.Cos(gamma);

            return new Point3D(x2, y3, z3);

        }

        public static Point3D GetRotoTranslatedPoint(ProgramContext programContext, double X, double Y, double Z)
        {
            return GetRotoTranslatedPoint(programContext, new Point3D(X, Y, Z));

        }

        public static Point3D GetRotoTranslatedPoint(ProgramContext programContext, Point3D point)
        {
            Point3D initialPoint = point;
            Point3D rotated = RotatePoint(programContext,initialPoint) ;
            Point3D translated = TranslatePoint(rotated, programContext);
            
            return translated;
        }

         

    }
}
