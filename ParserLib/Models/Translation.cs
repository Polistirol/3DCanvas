using ParserLib.Helpers;
using ParserLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static ParserLib.Helpers.TechnoHelper;

namespace ParserLib.Models
{
    public class Translation : ITranslation


        ////ricominciare da qui! , creare una classe traslation globale e una diversa per il context, questa implementata è per il context, quella globale deve poter sommarsi ada altre tranlsation ecc
        ///quella attuale poer il context potreppe in realtà essere rototraslation
    {
        public Translation(bool isGlobal) {
            this.isGlobal = isGlobal;
        }

        public Vector3D Components { get; set; }
        public bool isGlobal { get; set; }

        public void AddTraslations(ITranslation TranstoAdd)
        {
            Components = Vector3D.Add(this.Components,TranstoAdd.Components);
        }

        public void ResetTranslation()
        {
            Components = new Vector3D(0,0,0);
        }

        public void UpdateComponents(Vector3D newValues)
        {
            Components = newValues;
        }
    }
}
