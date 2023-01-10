﻿using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ParserLib.Interfaces
{
    public interface IToolpathEntity : IBaseEntity
    {
        Point3D EndPoint { get; set; }

        Point3D StartPoint { get; set; }

        PathGeometry GeometryPath { get; set; }

        bool IsLeadIn { get; set; }

    }
}