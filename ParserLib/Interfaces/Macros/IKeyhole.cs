﻿using ParserLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ParserLib.Interfaces.Macros
{
    public interface IKeyhole : IMacro
    {
        Point3D Center1 { get; set; }
        Point3D Center2 { get; set; }
        Point3D Normal { get; set; }
        double BigRadius { get; set; }
        double SmallRadius { get; set; }

    }
}
