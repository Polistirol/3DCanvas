using ParserLib.Interfaces;
using ParserLib.Models;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace PrimaPower.Resource
{
    public class Axes : List<IBaseEntity>
    {
        public Point3D OriginPoint { get; set; } = new Point3D(30, 40, 0);
        public double ShaftLenght { get; set; } = 50;
        public double ShaftThickness { get; set; }

        public void BuildAxes()
        {
            this.Clear();
            
            var XShaft = new LinearMove()
            {
                StartPoint = OriginPoint,
                
                EndPoint = new Point3D(OriginPoint.X +ShaftLenght, OriginPoint.Y, OriginPoint.Z),
                LineColor = ParserLib.Helpers.TechnoHelper.ELineType.CutLine3,
                IsBeamOn = true,
                Tag = "XZ"
            };     
            
            var YShaft = new LinearMove()
            {
                StartPoint = OriginPoint,
                EndPoint = new Point3D(OriginPoint.X, OriginPoint.Y+ ShaftLenght, OriginPoint.Z),
                LineColor = ParserLib.Helpers.TechnoHelper.ELineType.CutLine1,
                IsBeamOn = true,
                Tag = "YZ"
            };        
            
            var ZShaft = new LinearMove()
            {
                StartPoint = OriginPoint,
                EndPoint = new Point3D(OriginPoint.X, OriginPoint.Y, OriginPoint.Z -ShaftLenght),
                LineColor = ParserLib.Helpers.TechnoHelper.ELineType.CutLine2,
                IsBeamOn = true,
                Tag = "XY"
            };

            this.Add(XShaft);
            this.Add(YShaft);
            this.Add(ZShaft);

        }





    }
}
