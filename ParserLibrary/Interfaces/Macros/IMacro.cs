﻿using ParserLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLibrary.Interfaces.Macros
{
    public interface IMacro : IBaseEntity
    {
        List<ToolpathEntity> Movements { get; }
        ToolpathEntity LeadIn { get; }

        int CheckScrap { get; set; }
        int Repeat { get; set; }

       

    }
}
