using Canvas3DViewer.Converters;
using Canvas3DViewer.Models;
using Canvas3DViewer.ViewModels;
using ParserLibrary.Interfaces;
using ParserLibrary.Models;
using ParserLibrary.Models.Media;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Canvas3DViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public string Filename { get; set; }

        private From3DTo2DPointConversion from3Dto2DPointConversion = null;

        public int ClickedLine { get; set; }    

        public Window1()
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-EN");
            this.DataContext = new CncFilesViewModel();
            from3Dto2DPointConversion = new From3DTo2DPointConversion();
            ClickedLine = 0;    

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Viewer3D.SetCanvasInteractionEnabled(true);
            Viewer3D.EntityClicked += OnEntityClicked;
            Viewer3D.AxisClicked += OnAxisClicked;

        }

        private string CreateStringPoint(Point3D p, string s)
        {
            return $"{s} X:{Math.Round(p.X, 3)} Y:{Math.Round(p.Y, 3)} Z:{Math.Round(p.Z, 3)}";
        }
        private void OnAxisClicked(Path p)
        {
            
        }
        private void OnEntityClicked(Path p,int line)
        {
            PopolateTextBox(p);
            lineClickedBox.Text = line.ToString();
        }




        public void PopolateTextBox(Path p)
        {
            if (p.Tag != null && p.Tag is IBaseEntity)
            {
                var entity = p.Tag as IToolpathEntity;
                if (p.Tag is Macro macro)
                {
                    entity = macro.Movements[0] as IToolpathEntity;
                }

                txtLine.Text = entity.OriginalLine.ToString();
                txtLineNumber.Text = $"Source line: {entity.SourceLine.ToString()}";

                txtSP.Text = CreateStringPoint(entity.StartPoint, "Start p:");

                if (entity is ArcMove)
                {
                    txtVP.Visibility = Visibility.Visible;
                    txtVP.Text = CreateStringPoint((entity as ArcMove).ViaPoint, "Via p:");
                }
                else
                {
                    txtVP.Visibility = Visibility.Collapsed;
                }
                txtEP.Text = CreateStringPoint(entity.EndPoint, "End p:");
            }
        }



        public void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Viewer3D.ClearCanvas();
            if (e.AddedItems.Count == 0) return;
            var fi = e.AddedItems[0] as CncFile;
            Filename = fi.FullPath;

            //Viewer3D.PreviewFromPath(Filename);

            XElement preload = XElement.Load(Filename, LoadOptions.SetLineInfo);
            Viewer3D.PreviewFromXElement(preload,Filename);
        }

        private void txtLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (Filename != string.Empty && txtLine.Text != "")
                {
                    var nppDir = @"C:\Program Files\Notepad++";
                    var nppExePath = System.IO.Path.Combine(nppDir, "Notepad++.exe");
                    var nppReadmePath = System.IO.Path.Combine(nppDir, Filename);
                    var line = int.Parse(txtLineNumber.Text.Replace("Source line: ", ""));
                    var sb = new StringBuilder();
                    sb.AppendFormat("\"{0}\" -n{1}", nppReadmePath, line);
                    Process.Start(nppExePath, sb.ToString());
                }
            }
        }



        private bool isLeftCtrlDown
        {
            get { return Keyboard.IsKeyDown(Key.LeftCtrl); }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine(e.Key.ToString());
            if (e.Key == Key.S && isLeftCtrlDown)
            {
                Viewer3D.ShowHideRapids();
            }
            if (e.Key == Key.J && isLeftCtrlDown)
            {
                Viewer3D.RotateViewToPlane("XY");  
            }
            if (e.Key == Key.K && isLeftCtrlDown)
            {
                Viewer3D.RotateViewToPlane("XZ");
            }
            if (e.Key == Key.L && isLeftCtrlDown)
            {
                Viewer3D.RotateViewToPlane("YZ");
            }
            if (e.Key == Key.Q && isLeftCtrlDown)
            {
                Viewer3D.TestRot();
            }
            if (e.Key == Key.H && isLeftCtrlDown)
            {
                DisplayHelpBox();
            }
            if (e.Key == Key.P && isLeftCtrlDown)
            {
                
                
            }

        }

        private void DisplayHelpBox()
        {
            MessageBox.Show("USAGE : \n" +
                "CTRL+S = Toggle Rapids \n" +
                "CTRL+J = XY view \n" +
                "CTRL+K = XZ view \n" +
                "CTRL+L = YZ view \n" +
                "\n" +
                "CTRL+H = Help");
        }

        private void restartBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string value = textBox.Text;
            int line;
            if (int.TryParse(value, out line))
            {
                Viewer3D.RestartTrace(line);
                //Viewer3D.UpdateProgressBar( line);
            }
        }

        

        private void listView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                var item = ((ListViewItem)sender).Content;
                //if (e.AddedItems.Count == 0) return;
                //var fi = e.AddedItems[0] as CncFile;
                //Filename = fi.FullPath;
                
            }
            

        }
    }
}
