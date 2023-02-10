using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WizardViewer.ViewModel;

namespace WizardViewer.Components
{
    /// <summary>
    /// Logica di interazione per G104Control.xaml
    /// </summary>
    public partial class Point3DCoordinatesControl : UserControl
    {
        public Point3DCoordinatesControl(Point3DCoordinatesViewModel vM)
        {
            InitializeComponent();
            this.DataContext= vM;
            
        }        


 

    }
}
