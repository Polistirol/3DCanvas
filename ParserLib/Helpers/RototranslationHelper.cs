using ParserLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ParserLib.Helpers
{
    public static class RototranslationHelper
    {

        public static Vector3D CalculateNewTranslation(SortedDictionary<char, double?> axesDict)
        {
            Vector3D translationFromDict = new Vector3D(0,0,0);
            if (axesDict.ContainsKey('X')) translationFromDict.X = axesDict['X'] ?? 0;
            if (axesDict.ContainsKey('Y')) translationFromDict.Y = axesDict['Y'] ?? 0;
            if (axesDict.ContainsKey('Z')) translationFromDict.Z = axesDict['Z'] ?? 0;
            return translationFromDict;
        }

        public static void AddTranslation( ref List<Point3D> points,ProgramContext programContext)
        {

            Vector3D components = programContext.RotoTranslation.ActiveTranslation.Components;
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = Point3D.Add(points[i], components);
            }

        }        
        public static Point3D AddTranslation( Point3D point,ProgramContext programContext)
        {

             return Point3D.Add(point,programContext.RotoTranslation.ActiveTranslation.Components );


        }
    }
}
