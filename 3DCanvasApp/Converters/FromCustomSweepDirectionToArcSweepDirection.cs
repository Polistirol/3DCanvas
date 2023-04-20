using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Canvas3DViewer.Converters
{
    public class FromCustomSweepDirectionToArcSweepDirection : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (SweepDirection)value;
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
