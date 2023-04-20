using PrimaPower;
using System.Windows;

namespace trow_Snapshotter
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Siemens3DViewer viewer = new Siemens3DViewer();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Siemens3DViewer viewer = new Siemens3DViewer();
            string prg = "D:\\_DEV\\VS_DEV\\zz_RES\\ISO\\Parabol_V3_Test.iso";
            string pic = "D:\\_DEV\\VS_DEV\\zz_RES\\pic.jpg";
            Size size = new Size(700, 700);
            viewer.TakeSnapshot(prg, pic,size);
        }
    }
}
