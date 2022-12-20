using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ParserLib.Models;

namespace ParserLib.Interfaces
{
    public interface IRotoTranslation
    {
        
        Translation ActiveTranslation { get; set; }
        //Rotation activeRotation { get; set; }

        Vector3D TransLocalComponents { get; set; }
        Vector3D TransGlobalComponents { get; set; }
        //Vector3D LocalComponentsR { get; set; }
        //Vector3D GlobalComponentsR { get; set; }

        void UpdateTranslation(Translation translation);



    }
}
