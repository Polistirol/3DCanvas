using ParserLibrary.Models.Media;

namespace ParserLibrary.Interfaces
{
    public interface IToolpathEntity : IBaseEntity
    {
        Point3D EndPoint { get; set; }

        Point3D StartPoint { get; set; }
        Point3D OriginalEndPoint { get; set; }

        Point3D OriginalStartPoint { get; set; }

        object GeometryPath { get; set; }

        bool IsLeadIn { get; set; }

    }
}