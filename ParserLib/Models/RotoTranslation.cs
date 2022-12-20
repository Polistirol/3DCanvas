using ParserLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ParserLib.Models
{
    public class RotoTranslation : IRotoTranslation
    {
        public Translation ActiveTranslation { get; set; }
        public Vector3D TransLocalComponents { get; set; }
        public Vector3D TransGlobalComponents { get; set; }

        public RotoTranslation()
        {
            ActiveTranslation = new Translation(true)
            {
                Components = new Vector3D(0, 0, 0)
            };
        }
        public void UpdateTranslation(Translation translation)
        {
            if (translation.isGlobal)
            {
                TransGlobalComponents = translation.Components;
            }
            else
            {
                if (translation.Components.X == 0 && translation.Components.Y == 0 && translation.Components.Z == 0)
                {

                    TransLocalComponents = new Vector3D(0,0,0);
                }
                else
                {
                    TransLocalComponents = Vector3D.Add(TransLocalComponents, translation.Components);
                }

            }

            ActiveTranslation.Components= Vector3D.Add(TransGlobalComponents,TransLocalComponents);
        }


    }
}
