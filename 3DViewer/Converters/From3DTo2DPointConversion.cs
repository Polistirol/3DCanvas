using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using custom=ParserLibrary.Models.Media;

namespace PrimaPower.Converters
{
    public class From3DTo2DPointConversion : IValueConverter
    {
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is custom.Point3D)
                {
                    custom.Point3D p3D = (custom.Point3D)value;
                    return new Point(p3D.X, p3D.Y);
                }
                else if (value is Point3D)
                {
                    Point3D p3D = (Point3D)value;
                    return new Point(p3D.X, p3D.Y);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
                
            }
            




        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
