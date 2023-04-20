using System.Windows.Markup;



namespace ParserLibrary.Models.Media
{
    public class Vector3DValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (!(value is Vector3D))
            {
                return false;
            }

            return true;
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            if (value != null)
            {
                return Vector3D.Parse(value);
            }

            return base.ConvertFromString(value, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value is Vector3D)
            {
                //return ((Vector3D)value).ConvertToString(null, System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS);
            }

            return base.ConvertToString(value, context);
        }
    }
}


