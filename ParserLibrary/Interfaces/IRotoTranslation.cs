using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLibrary.Models.Media;
using ParserLibrary.Models;

namespace ParserLibrary.Interfaces
{
    public interface IRotoTranslation
    {

        Vector3D LocalTranslationComponents { get; set; }
        Vector3D GlobalTranslationComponents { get; set; }
        Vector3D ActiveTranslationComponents { get; set; }
        Vector3D LocalRotationComponents { get; set; }
        Vector3D GlobalRotationComponents { get; set; }

        Vector3D ActiveRotationComponents { get; set; }

        void UpdateTranslation(Vector3D newTranslation, bool isGlobal = true );
        void UpdateRotation(Vector3D newRotation, bool isGlobal = true );
        void UpdateRotoTraslComponents(Vector3D newRotation,Vector3D newTranslation, bool isGlobal = true );

        

    }
}
