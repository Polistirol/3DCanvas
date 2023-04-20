using ParserLibrary.Models.Media;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Models
{
    public class LinearMove : ToolpathEntity
    {
        public override EEntityType EntityType { get => EEntityType.Line; }

        public override void Render(Matrix3D U, Matrix3D Un, bool isRot, double Zradius)
        {
            StartPoint = U.Transform(StartPoint);
            EndPoint = U.Transform(EndPoint);
            OnPropertyChanged("StartPoint");
            OnPropertyChanged("EndPoint");
        }

        public override string ToString()
        {
            return $"Line sp: {StartPoint} ep: {EndPoint}";
        }
    }
}