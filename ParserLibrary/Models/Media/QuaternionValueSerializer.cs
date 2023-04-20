using System.Windows.Markup;

namespace ParserLibrary.Models.Media
{
    public class QuaternionValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (!(value is Quaternion))
            {
                return false;
            }

            return true;
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            if (value != null)
            {
                return Quaternion.Parse(value);
            }

            return base.ConvertFromString(value, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value is Quaternion)
            {
                //return ((Quaternion)value).ConvertToString(null, System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS);
            }

            return base.ConvertToString(value, context);
        }
    }
}

