﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace ParserLibrary.Models.Media
{
    public sealed class QuaternionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            string text = value as string;
            if (text != null)
            {
                return Quaternion.Parse(text);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != null && value is Quaternion)
            {
                Quaternion quaternion = (Quaternion)value;
                if (destinationType == typeof(string))
                {
                    return quaternion.ConvertToString(null, culture);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}


