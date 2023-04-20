using System.Windows.Media;
using static ParserLibrary.Helpers.TechnoHelper;

namespace PrimaPower.Resource
{
    public static class ColorsHelper
    {
        public static SolidColorBrush GetLineColor(ELineType lineColor)
        {
            switch (lineColor)
            {
                case ELineType.CutLine1:
                    return Brushes.Green;

                case ELineType.CutLine2:
                    return Brushes.RoyalBlue;

                case ELineType.CutLine3:
                    return Brushes.Red;

                case ELineType.CutLine4:
                    return Brushes.Violet;

                case ELineType.CutLine5:
                    return Brushes.Aqua;

                case ELineType.Marking:
                    return Brushes.DarkGoldenrod;

                case ELineType.Microwelding:
                case ELineType.Rapid:
                    return Brushes.Gray;

                default:
                    return Brushes.White;
            }
        }
        public static SolidColorBrush GetLineColorTrace(ELineType lineColor)
        {
            return Brushes.Gray;
            switch (lineColor)
            {
                case ELineType.CutLine1:
                    return Brushes.YellowGreen;

                case ELineType.CutLine2:
                    return Brushes.DodgerBlue;

                case ELineType.CutLine3:
                    return Brushes.Tomato;

                case ELineType.CutLine4:
                    return Brushes.Thistle;

                case ELineType.CutLine5:
                    return Brushes.PaleTurquoise;

                case ELineType.Marking:
                    return Brushes.Wheat;

                case ELineType.Microwelding:
                case ELineType.Rapid:
                    return Brushes.Gray;

                default:
                    return Brushes.White;
            }
        }

        
    }
}
