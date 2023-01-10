using ParserLib.Interfaces;
using System.Data.SqlClient;
using System.Windows.Media.Media3D;

namespace ParserLib.Models
{
    public class RotoTranslation : IRotoTranslation
    {



        public RotoTranslation() { }
       

        public Vector3D LocalTranslationComponents { get; set; } = new Vector3D( 0, 0, 0 );
        public Vector3D GlobalTranslationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D ActiveTranslationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D LocalRotationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D GlobalRotationComponents { get; set; } = new Vector3D(0, 0, 0);
        public Vector3D ActiveRotationComponents { get; set; } = new Vector3D(0, 0, 0);

        public void UpdateRotation(Vector3D newRotation, bool isGlobal = true)
        {
            if (isGlobal) //G93
            {
                LocalRotationComponents = new Vector3D(0, 0, 0);
                GlobalRotationComponents = new Vector3D(newRotation.X, newRotation.Y, newRotation.Z);
            }
            else //G94
            {
                LocalRotationComponents = Vector3D.Add(LocalRotationComponents, newRotation);

            }
            ActiveRotationComponents = Vector3D.Add(LocalRotationComponents, GlobalRotationComponents);
        }



        public void UpdateTranslation(Vector3D newTranslation, bool isGlobal = true)
        {
            if (isGlobal) //G93
            {               
                LocalTranslationComponents = new Vector3D(0, 0, 0);
                GlobalTranslationComponents = new Vector3D(newTranslation.X, newTranslation.Y, newTranslation.Z);
            }
            else //G94
            {
                
                LocalTranslationComponents = Vector3D.Add(  LocalTranslationComponents , newTranslation);

            }
            ActiveTranslationComponents = Vector3D.Add(LocalTranslationComponents,GlobalTranslationComponents);

            
        }
    }
}
