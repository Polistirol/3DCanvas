using ParserLib.Interfaces;
using ParserLib.Interfaces.Macros;
using ParserLib.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WizardViewer.Components;
using WizardViewer.ViewModel;
using static ParserLib.Helpers.TechnoHelper;

namespace WizardViewer
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {

        public string resourcePath = @"D:\_DEV\VS_DEV\Repos\3DCanvas\WizardViewer\resources\";
        public string imagFileExtension = ".png";
        public string[] defaultFormat = new string[3] {"X","Y","Z"};
        public Path inputPath { get; set; }
        public EditingButtonsControl ButtonsControl {get; set;}
        public UserControl1()
        {
            InitializeComponent();
            this.Visibility = Visibility.Collapsed;
            GeometryStack.IsEnabled = false;
            AssignButtonActions();

        }

        private void AssignButtonActions()
        {
            Buttons.PreviewBtn_Clicked += OnPreviewBtnClick;
            Buttons.UnlockBtn_Clicked += OnUnlockBtnClick;
            Buttons.SaveBtn_Clicked += OnSaveBtnClick;
            Buttons.RevertBtn_Clicked += OnRevertBtnClick;
        }



        public void Reset()
        {
            this.Visibility = Visibility.Collapsed;
            img.Source = null;
            GeometryStack.Children.Clear();
            GeometryStack.IsEnabled = false;
            Buttons.ResetButtons();

        }
        
        public void SetWizard(Path p)
        {
            Reset();
            this.Visibility = Visibility.Visible;
            var entity = p.Tag as IBaseEntity;
            if (entity is IMacro macro)
            {

                switch (macro.EntityType)
                {
                    case EEntityType.Hole:
                        BuildHoleWizard(entity as IHole);
                        break;
                    case EEntityType.Slot:
                        BuildSlotWizard(entity as ISlot);
                        break; 
                    case EEntityType.Keyhole:
                        BuildKeyholeWizard(entity as IKeyhole);
                        break;
                    case EEntityType.Poly:
                        BuildPolyWizard(entity as IPoly);
                        break;
                    case EEntityType.Rect:
                        BuildRectWizard(entity as IRect);
                        break;
                    default: break;
                }
            }
            else if (entity is ArcMove)
                BuildArcMoveWizard(entity as ArcMove);
            else if (entity is LinearMove)
                BuildLinearMoveWizard(entity as LinearMove);
            else
                return;
            SetStandardValues(entity);        
        }

        private void BuildLinearMoveWizard(LinearMove linearMove)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "LINE" + imagFileExtension));
            //AddControl("Start Point", linearMove.StartPoint, defaultFormat, false);
            AddControl("End Point", linearMove.OriginalEndPoint, defaultFormat);
        }
        private void BuildArcMoveWizard(ArcMove g104)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "A3P" + imagFileExtension));
            //AddControl("Start Point", g104.StartPoint, defaultFormat,false);
            AddControl("Via Point", g104.OriginalViaPoint, new string[] { "I", "J", "K" });
            AddControl("End Point", g104.OriginalEndPoint, defaultFormat);
            
        }
        private void BuildHoleWizard(IHole hole)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "HOLE" + imagFileExtension));
            AddControl("Center", hole.Center, defaultFormat);
            AddControl("Normal", hole.Normal, defaultFormat);
            AddControl("Radius", hole.Radius);


        }
        private void BuildSlotWizard(ISlot slot)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "SLOT" + imagFileExtension));
            AddControl("Center1", slot.Center1, defaultFormat);
            AddControl("Center2", slot.Center2, defaultFormat);
            AddControl("Normal", slot.Normal, defaultFormat);
            AddControl("Radius", slot.Radius);
        }
        private void BuildRectWizard(IRect rect)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "RECT" + imagFileExtension));
            AddControl("Center", rect.CenterPoint, defaultFormat);
            AddControl("Vertex", rect.VertexPoint, defaultFormat);
            AddControl("Side", rect.SidePoint, defaultFormat);

        }

        private void BuildPolyWizard(IPoly poly)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "POLY" + imagFileExtension));
            AddControl("Center", poly.CenterPoint, defaultFormat);
            AddControl("Vertex", poly.VertexPoint, defaultFormat);
            AddControl("Side", poly.NormalPoint, defaultFormat);
            AddControl("Sides", poly.Sides);
        }
        private void BuildKeyholeWizard(IKeyhole keyhole)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "KEYHOLE" + imagFileExtension));
            AddControl("Center1", keyhole.Center1, defaultFormat);
            AddControl("Center2", keyhole.Center2, defaultFormat);
            AddControl("Normal", keyhole.Normal, defaultFormat);
            AddControl("Radius1", keyhole.BigRadius);
            AddControl("Radius2", keyhole.SmallRadius);
        }



        private SolidColorBrush GetLineColor(ELineType lineColor)
        {
            switch (lineColor)
            {
                case ELineType.CutLine1:
                    return Brushes.Green;

                case ELineType.CutLine2:
                    return Brushes.RoyalBlue;

                case ELineType.CutLine3:
                    return Brushes.Red;

                case ELineType.CutLine4:
                    return Brushes.Violet;

                case ELineType.CutLine5:
                    return Brushes.Aqua;

                case ELineType.Marking:
                    return Brushes.Yellow;

                case ELineType.Microwelding:
                case ELineType.Rapid:
                    return Brushes.Gray;

                default:
                    return Brushes.White;
            }
        }

        private void SetStandardValues(IBaseEntity entity)
        {
            AddControl("Cutting Line", (int)entity.LineColor);
        }

        private void AddControl(string pointName , Point3D point , string[] axisFormat) {
            var vM = new Point3DCoordinatesViewModel();
            vM.PointName = pointName;
            vM.X = point.X;
            vM.Y = point.Y;
            vM.Z = point.Z;
            vM.XName = axisFormat[0];
            vM.YName = axisFormat[1];
            vM.ZName = axisFormat[2];
            var control = new Point3DCoordinatesControl(vM);
            GeometryStack.Children.Add(control);
        }

        private void AddControl(string pointName, double value )
        {
            var vM = new Point3DCoordinatesViewModel();
            vM.PointName = pointName;
            vM.X = value;
            vM.Y = null;
            vM.Z = null;
            var control = new Point3DCoordinatesControl(vM);
            GeometryStack.Children.Add(control);
        }

        private void AddControl(string pointName, Point point, string[] axisFormat)
        {
            var vM = new Point3DCoordinatesViewModel();
            vM.PointName = pointName;
            vM.X = point.X;
            vM.Y = point.Y;
            //vM.Z = point.Z;
            vM.XName = axisFormat[0];
            vM.YName = axisFormat[1];
            //vM.ZName = axisFormat[2];
            var control = new Point3DCoordinatesControl(vM);
            GeometryStack.Children.Add(control);
        }

        private void OnUnlockBtnClick()
        {
            GeometryStack.IsEnabled = Buttons.vM.IsUnlocked;
        }

        private void OnPreviewBtnClick()
        {
            move
        }

        private void OnRevertBtnClick()
        {
            
        }

        private void OnSaveBtnClick()
        {
           
        }
    }
}
