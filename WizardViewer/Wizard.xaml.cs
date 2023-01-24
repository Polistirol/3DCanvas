using ParserLib.Interfaces;
using ParserLib.Interfaces.Macros;
using ParserLib.Models;
using System;
using System.Collections;
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
        public UserControl1()
        {
            InitializeComponent();
            this.Visibility = Visibility.Collapsed;
        }

        public void Reset()
        {
            this.Visibility = Visibility.Collapsed;
            img.Source = null;
            GeometryStack.Children.Clear();
            //ButtonStack.Children.Clear();
            //TechnologyStack.Children.Clear();
            

        }
        
        public void SetWizard(IBaseEntity entity)
        {
            this.Visibility = Visibility.Visible;
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
                    default: break;

                }


            }
            else if (entity is ArcMove)
                BuildArcMoveWizard(entity as ArcMove);
            else if (entity is LinearMove)
                BuildLinearMoveWizard(entity as LinearMove);
            else
                return;
        }

        private void BuildLinearMoveWizard(LinearMove linearMove)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "LINE" + imagFileExtension));
            AddStackPanel_NamePoint(GeometryStack, "EndPoint", linearMove.EndPoint);
            SetStandardTechnoStack(linearMove as IBaseEntity);
        }

        private void BuildHoleWizard(IHole hole)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "HOLE" + imagFileExtension));

            #region geometry Stack
            AddStackPanel_NamePoint(GeometryStack, "Center", hole.Center);
            AddStackPanel_NamePoint(GeometryStack, "Normal", hole.Normal);
            AddStackPanel_NameValue(GeometryStack, "Radius", hole.Radius);
            #endregion

            #region techno stack
            SetStandardTechnoStack(hole as IBaseEntity);
            #endregion

        }
        private void BuildSlotWizard(ISlot slot)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "SLOT" + imagFileExtension));
            AddStackPanel_NamePoint(GeometryStack, "Center1", slot.Center1);
            AddStackPanel_NamePoint(GeometryStack,"Center2", slot.Center2);
            AddStackPanel_NamePoint(GeometryStack,"Normal",slot.Normal );
            AddStackPanel_NameValue(GeometryStack,"Radius", slot.Radius);
            #region techno stack
            SetStandardTechnoStack(slot as IBaseEntity);
            #endregion
        }


        private void BuildArcMoveWizard(ArcMove g104)
        {
            img.Source = new BitmapImage(new Uri(resourcePath + "G104" + imagFileExtension));
            AddStackPanel_G104(GeometryStack, g104.StartPoint, g104.ViaPoint);
            SetStandardTechnoStack(g104 as IBaseEntity);
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

        private void SetStandardTechnoStack(IBaseEntity entity)
        {
            AddStackPanel_NameValue(GeometryStack, "Color", (int)entity.LineColor, GetLineColor(entity.LineColor));
            //AddStackPanel_NameValue(TechnologyStack, "at Line", entity.SourceLine);
            //AddStackPanel_NameValue(TechnologyStack, "Original Line", entity.OriginalLine);
        }
        private void AddStackPanel_G104(StackPanel SPtoAddTo,Point3D startPoint, Point3D viaPoint)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Background = Brushes.White;
            sp.Name = "G104SP";
            sp.Children.Add(new Label()
            {
                Content = "G104"
            });
            sp.Children.Add(new Label()
            {
                Content = "X",
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = startPoint.X.ToString(),
                Width = 60,

            });
            sp.Children.Add(new Label()
            {
                Content = "Y",
                Foreground = Brushes.Green,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = startPoint.Y.ToString(),
                Width = 60,
            });
            sp.Children.Add(new Label()
            {
                Content = "Z",
                Foreground = Brushes.Blue,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = startPoint.Z.ToString(),
                Width = 60,
            });
            sp.Children.Add(new Label()
            {
                Content = "I",
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = viaPoint.X.ToString(),
                Width = 60,
            }); 
            sp.Children.Add(new Label()
            {
                Content = "J",
                Foreground = Brushes.Green,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = viaPoint.Y.ToString(),
                Width = 60,
            });
            sp.Children.Add(new Label()
            {
                Content = "K",
                Foreground = Brushes.Blue,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = viaPoint.Z.ToString(),
                Width = 60,
            });

            SPtoAddTo.Children.Add(sp);
        }
        private void AddStackPanel_NamePoint(StackPanel SPtoAddTo, string pointName, Point3D point)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Background = Brushes.White;
            sp.Name = pointName+"SP";
            sp.Children.Add(new Label()
            {
                Content = pointName
            });
            sp.Children.Add(new Label()
            {
                Content = "X",
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = point.X.ToString(),
                Width = 60,

            });
            sp.Children.Add(new Label()
            {
                Content = "Y",
                Foreground = Brushes.Green,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = point.Y.ToString(),
                Width = 60,
            });
            sp.Children.Add(new Label()
            {
                Content = "Z",
                Foreground = Brushes.Blue,
                FontWeight = FontWeights.Bold,
            });
            sp.Children.Add(new TextBox()
            {
                Text = point.Z.ToString(),
                Width = 60,
            });
            SPtoAddTo.Children.Add(sp);
            

        }
        private void AddStackPanel_NameValue(StackPanel SPtoAddTo, string ItemName,double value,Brush textColor = null)
        {
            AddStackPanel_NameValue(SPtoAddTo, ItemName, value.ToString(), textColor);
        }        
        private void AddStackPanel_NameValue(StackPanel SPtoAddTo, string ItemName,string value,Brush textColor = null)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Background = Brushes.White;
            sp.Children.Add(new Label()
            {
                Content = ItemName
            });
            sp.Children.Add(new TextBox()
            {
                Text = value,
                
                MinWidth= 60,
                FontWeight= FontWeights.Bold,
                Foreground = textColor != null ? textColor : Brushes.Black,
            });

            SPtoAddTo.Children.Add(sp); 
        }

    }
}
