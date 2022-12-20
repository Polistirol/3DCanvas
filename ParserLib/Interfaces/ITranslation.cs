using ParserLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static ParserLib.Helpers.TechnoHelper;

namespace ParserLib.Interfaces
{
    public interface ITranslation
    {

        bool isGlobal { get; set; }
        Vector3D Components { get; set; }

        
        void UpdateComponents(Vector3D components);

        void AddTraslations(ITranslation T1);
        void ResetTranslation();
    }
}
