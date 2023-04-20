using ParserLibrary.Interfaces;
using ParserLibrary.Interfaces.Macros;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ParserLibrary.Models.Media;
using static ParserLibrary.Helpers.TechnoHelper;
using System.Linq;

namespace ParserLibrary.Models
{
    public class ProgramContext : IProgramContext
    {
        public ProgramContext()
        {

        }

        public double xMin { get; set; } = double.PositiveInfinity;
        public double xMax { get; set; } = double.NegativeInfinity;
        public double yMin { get; set; } = double.PositiveInfinity;
        public double yMax { get; set; } = double.NegativeInfinity;
        public double zMin { get; set; } = double.PositiveInfinity;
        public double zMax { get; set; } = double.NegativeInfinity;



        public IToolpathEntity ReferenceMove { get; set; }
        public IToolpathEntity LastEntity { get; set; }
        public ELineType ContourLineType { get; set; }
        public Point3D LastHeadPosition { get; set; }

        public Point3D CenterRotationPoint { get; set; }

        public int FirstSourceLine => Moves.Count != 0 ? Moves.FirstOrDefault().SourceLine : 0;
        public int LastSourceLine => Moves.Count != 0 ? Moves.LastOrDefault().SourceLine : 0;

        public int SourceLine { get; set; }
        public bool IsIncremental { get; set; }
        public bool InMainProgram { get; set; }
        public bool IsBeamOn { get; set; }
        public bool IsMarkingProgram { get; set; }
        public bool IsInchProgram { get; set; }
        public bool Is3DProgram { get; set; }
        public bool Is2DProgram { get; set; }
        public bool IsTubeProgram { get; set; }
        public bool IsWeldProgram { get; set; }
        public IList<IBaseEntity> Moves { get; set; } = new List<IBaseEntity>();
        public IRotoTranslation RotoTranslation { get; set; }
        public int TextLineCount { get; set; }

        public void UpdateProgramCenterPoint()
        {
            if (LastEntity != null && IsBeamOn)
            {
                if (LastEntity is IMacro macro)
                {
                    foreach (var item in macro.Movements)
                    {
                        CalculateMinMaxFromBaseEntity(item);
                    }
                }

                else
                {
                    CalculateMinMaxFromBaseEntity(LastEntity);
                }

                CenterRotationPoint = new Point3D((yMin + yMax) / 2, (xMin + xMax) / 2, (zMin + zMax) / 2);
            }
        }

        private void CalculateMinMaxFromBaseEntity(IToolpathEntity BaseEntity)
        {
            xMin = Math.Min(BaseEntity.StartPoint.X, xMin);
            xMin = Math.Min(BaseEntity.EndPoint.X, xMin);
            xMax = Math.Max(BaseEntity.StartPoint.X, xMax);
            xMax = Math.Max(BaseEntity.EndPoint.X, xMax);
            yMin = Math.Min(BaseEntity.StartPoint.Y, yMin);
            yMin = Math.Min(BaseEntity.EndPoint.Y, yMin);
            yMax = Math.Max(BaseEntity.StartPoint.Y, yMax);
            yMax = Math.Max(BaseEntity.EndPoint.Y, yMax);
            zMin = Math.Min(BaseEntity.StartPoint.Z, zMin);
            zMin = Math.Min(BaseEntity.EndPoint.Z, zMin);
            zMax = Math.Max(BaseEntity.StartPoint.Z, zMax);
            zMax = Math.Max(BaseEntity.EndPoint.Z, zMax);

            if (BaseEntity is IArc)
            {

                xMin = Math.Min((BaseEntity as IArc).ViaPoint.X, xMin);
                xMax = Math.Max((BaseEntity as IArc).ViaPoint.X, xMax);
                yMin = Math.Min((BaseEntity as IArc).ViaPoint.Y, yMin);
                yMax = Math.Max((BaseEntity as IArc).ViaPoint.Y, yMax);
                zMin = Math.Min((BaseEntity as IArc).ViaPoint.Z, zMin);
                zMax = Math.Max((BaseEntity as IArc).ViaPoint.Z, zMax);
            }
        }
    }
}
