using ParserLib;
using ParserLib.Interfaces;
using ParserLib.Interfaces.Macros;
using ParserLib.Models;
using ParserLib.Services.Parsers;
using PrimaPower.Converters;
using PrimaPower.Resource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static ParserLib.Helpers.TechnoHelper;

namespace PrimaPower
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class Siemens3DViewer : UserControl
    {

        private List<IBaseEntity> moves;
        
        public string Filename { get; set; }

        private bool CanvasEventsEnabled { get; set; }
        private bool UseDefaultEvents { get; set; }
        

        private Point previousCoordinate;
        private Point3D centerRotation = new Point3D(150, 150, 0);
        
        private From3DTo2DPointConversion from3Dto2DPointConversion = null;
        private Brush _originalColor = null;

        private Axes axes = new Axes();

       
         


        public Siemens3DViewer()
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-EN");
            from3Dto2DPointConversion = new From3DTo2DPointConversion();
            axes.BuildAxes();
            
        }
        
        public void DrawProgram(string fullName, bool isRedrawing = false)
        {
            //Stopwatch ost = new Stopwatch();
            //st.Start();        
           Filename= fullName;
            try
            {
                //ost.Start();
                Parser parser = null;
                var extension = System.IO.Path.GetExtension(fullName).ToLower().Trim();
                if (extension == ".iso")
                    parser = new Parser(new ParseIso(fullName));
                else if (extension == ".mpf")
                    parser = new Parser(new ParseMpf(fullName));
                else
                    MessageBox.Show("File extension invalid");
                

                var programContext = parser.GetProgramContext();
                moves = (List<IBaseEntity>)programContext.Moves;
                //ost.Stop();
                //Console.WriteLine($"Time to obtain moves of: {System.IO.Path.GetFileName(fullName)} is {ost.ElapsedMilliseconds}ms");

                Stopwatch st = Stopwatch.StartNew();

                if (moves == null) return;

                centerRotation = programContext.CenterRotationPoint;

                foreach (var item in moves)
                {
                    if (item.IsBeamOn == false)
                    {
                        if (item.EntityType == EEntityType.Line && moves.Count < 2000)
                        {
                            DrawLine(item as LinearMove, true);
                        }
                    }
                    else
                    {
                        if (item is IMacro macro)
                        {
                            foreach (var move in macro.Movements)
                            {
                                if (move is LinearMove)
                                    DrawLine(move as LinearMove);
                                else
                                    DrawArc(move as ArcMove);
                            }
                        }
                        else if (item.EntityType == EEntityType.Line)
                        {
                            DrawLine(item as LinearMove);
                        }
                        else if (item.EntityType == EEntityType.Arc)
                        {
                            DrawArc(item as ArcMove);
                        }

                    }
                }

                st.Stop();
                InitialTransform();
                Console.WriteLine($"Program: {System.IO.Path.GetFileName(fullName)} is completed in {st.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DrawAxes(string plane = "XY" )
        {
            
            canvasAxes.Children.Clear();
            axes.BuildAxes();
            Matrix3D U, Un;
            U = Matrix3D.Identity;
            Un = Matrix3D.Identity;
            
            Quaternion planeQuat = SetViewPlane(plane);
            
            U.RotateAt(planeQuat, axes.OriginPoint);          
            Un.RotateAt(planeQuat, new Point3D(0, 0, 0));

            foreach (var item in axes)
            {
                DrawLine(item as LinearMove,false,true);
                item.Render(U, Un, false, 1);
            }

        }


        public void InitialTransform(string plane="")
        {
            Matrix3D U, Un;
            Point3D cor;
            SetViewPoint(plane, out U, out Un, out cor);

            double xMin, xMax, yMin, yMax;
            ShiftToCenter(ref U, ref Un, ref cor, out xMin, out xMax, out yMin, out yMax);
            #region Zoom drawing into Canvas

            SetZoom(U, Un, cor, xMin, xMax, yMin, yMax);
            #endregion Zoom drawing into Canvas
            DrawAxes(plane);
        }

        private void SetViewPoint(string plane, out Matrix3D U, out Matrix3D Un, out Point3D cor)
        {
            #region Top View of the drawing

            U = Matrix3D.Identity;
            Un = Matrix3D.Identity;
            cor = new Point3D(centerRotation.Y, centerRotation.X, centerRotation.Z);
            Quaternion planeQuat = SetViewPlane(plane);
            U.RotateAt(planeQuat, centerRotation);
            Un.RotateAt(planeQuat, new Point3D(0, 0, 0));

            foreach (var item in moves)
            {
                item.Render(U, Un, false, 1);
            }

            #endregion Top View of the drawing
        }

        private void SetZoom(Matrix3D U, Matrix3D Un, Point3D cor, double xMin, double xMax, double yMin, double yMax)
        {
            U.SetIdentity();
            Un.SetIdentity();

            double dX = xMax - xMin;
            double dY = yMax - yMin;
            double margin = 50;

            double newZ = (dX > dY) ? (canvas1.ActualWidth - margin) / dX : (canvas1.ActualHeight - margin) / dY;

            U.ScaleAt(new Vector3D(newZ, newZ, newZ), cor);

            foreach (var item in moves)
            {
                item.Render(U, Un, false, newZ);
            }
        }

        private void ShiftToCenter(ref Matrix3D U, ref Matrix3D Un, ref Point3D cor, out double xMin, out double xMax, out double yMin, out double yMax)
        {
            #region Shift Drawing in the center of the canvas

            xMin = double.PositiveInfinity;
            xMax = double.NegativeInfinity;
            yMin = double.PositiveInfinity;
            yMax = double.NegativeInfinity;
            foreach (var item in moves)
            {
                if (!item.IsBeamOn) { continue; }
                var entity = item; //(item as IToolpathEntity);
                if (double.IsNegativeInfinity(entity.BoundingBox.Item1) || double.IsInfinity(entity.BoundingBox.Item2) || double.IsNegativeInfinity(entity.BoundingBox.Item3) || double.IsInfinity(entity.BoundingBox.Item4))
                    continue;

                xMin = Math.Min(entity.BoundingBox.Item1, xMin);
                xMax = Math.Max(entity.BoundingBox.Item2, xMax);
                yMin = Math.Min(entity.BoundingBox.Item3, yMin);
                yMax = Math.Max(entity.BoundingBox.Item4, yMax);
            }

            double xMed = (xMax + xMin) / 2;
            double yMed = (yMax + yMin) / 2;

            double yMedCanvas = canvas1.ActualHeight / 2;
            double xMedCanvas = canvas1.ActualWidth / 2;

            U.SetIdentity();
            Un.SetIdentity();

            U.OffsetX = xMedCanvas - xMed;
            U.OffsetY = yMedCanvas - yMed;

            cor = U.Transform(cor);

            centerRotation.X = yMedCanvas;
            centerRotation.Y = xMedCanvas;
            centerRotation.Z = cor.Z;

            cor = new Point3D(centerRotation.Y, centerRotation.X, centerRotation.Z);
            //EllipseGeometry centro = new EllipseGeometry(new Point(cor.X, cor.Y), 10.0, 10.0);
            //Ellipse e = new Ellipse();

            //canvas1.Children.Add(new Ellipse( )
            foreach (var item in moves)
            {
                item.Render(U, Un, false, 1);
            }

            #endregion Shift Drawing in the center of the canvas
        }



        private Quaternion SetViewPlane(string plane)
        {
            if (plane == "XY")
            {
                return new Quaternion(new Vector3D(1, 0, 0), 0);
            }
            else if (plane == "XZ")
            {
                return (new Quaternion(new Vector3D(1, 0, 0), -90) * new Quaternion(new Vector3D(0, 0, 1), -90) ) ;
            }
            else if (plane == "YZ")
            {
                return new Quaternion(new Vector3D(1, 0, 0), -90);
            }

            return new Quaternion(new Vector3D(1, 0, 0), 180);
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

       

        private void DrawArc(ArcMove arcMove)
        {
            PathFigure pf = new PathFigure();
            ArcSegment ls = new ArcSegment();

            //BindingBase sourceBinding = new Binding { Source = arcMove, Path = new PropertyPath("StartPoint"), Converter = from3Dto2DPointConversion };
            BindingOperations.SetBinding(pf, PathFigure.StartPointProperty, new Binding { Source = arcMove, Path = new PropertyPath("StartPoint"), Converter = from3Dto2DPointConversion });

            pf.Segments.Add(ls);

            //BindingBase destinationBindingPoint = new Binding { Source = arcMove, Path = new PropertyPath("EndPoint"), Converter = from3Dto2DPointConversion };
            //BindingBase destinationBindingSize = new Binding { Source = arcMove, Path = new PropertyPath("ArcSize") };
            //BindingBase destinationBindingRotationAngle = new Binding { Source = arcMove, Path = new PropertyPath("RotationAngle") };
            //BindingBase destinationBindingIsLargeArc = new Binding { Source = arcMove, Path = new PropertyPath("IsLargeArc") };
            //BindingBase destinationBindingIsStroked = new Binding { Source = arcMove, Path = new PropertyPath("IsStroked") };
            //BindingBase destinationBindingSweepDirection = new Binding { Source = arcMove, Path = new PropertyPath("ArcSweepDirection") };

            BindingOperations.SetBinding(ls, ArcSegment.PointProperty, new Binding { Source = arcMove, Path = new PropertyPath("EndPoint"), Converter = from3Dto2DPointConversion });
            BindingOperations.SetBinding(ls, ArcSegment.SizeProperty, new Binding { Source = arcMove, Path = new PropertyPath("ArcSize") });
            BindingOperations.SetBinding(ls, ArcSegment.RotationAngleProperty, new Binding { Source = arcMove, Path = new PropertyPath("RotationAngle") });
            BindingOperations.SetBinding(ls, ArcSegment.IsLargeArcProperty, new Binding { Source = arcMove, Path = new PropertyPath("IsLargeArc") });
            BindingOperations.SetBinding(ls, ArcSegment.IsStrokedProperty, new Binding { Source = arcMove, Path = new PropertyPath("IsStroked") });
            BindingOperations.SetBinding(ls, ArcSegment.SweepDirectionProperty, new Binding { Source = arcMove, Path = new PropertyPath("ArcSweepDirection") });

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(pf);
            arcMove.GeometryPath = geometry;

            Path p = new Path
            {
                StrokeThickness = 1,
                Tag = arcMove,
                Stroke = GetLineColor(arcMove.LineColor),
                Data = geometry
            };

            if (CanvasEventsEnabled)
            {
                p.MouseDown += MouseClickEntity;
                p.MouseEnter += MouseEnterEntity;
                p.MouseLeave += MouseLeaveEntity;
            }
            else
            {
                p.MouseDown -= MouseClickEntity;
                p.MouseEnter -= MouseEnterEntity;
                p.MouseLeave -= MouseLeaveEntity;
            }

            

            canvas1.Children.Add(p);
        }


        private void DrawLine(LinearMove linearMove, bool isRapid = false, bool isAxes = false)
        {
            PathFigure pf = new PathFigure();
            LineSegment ls = new LineSegment();

            BindingBase sourceBinding = new Binding { Source = linearMove, Path = new PropertyPath("StartPoint"), Converter = from3Dto2DPointConversion };
            BindingOperations.SetBinding(pf, PathFigure.StartPointProperty, sourceBinding);

            pf.Segments.Add(ls);

            BindingBase destinationBinding = new Binding { Source = linearMove, Path = new PropertyPath("EndPoint"), Converter = from3Dto2DPointConversion };
            BindingOperations.SetBinding(ls, LineSegment.PointProperty, destinationBinding);

            PathGeometry geometry = new PathGeometry();

            geometry.Figures.Add(pf);

            Path p = new Path();
            p.StrokeThickness = 1;

            if (isRapid) p.StrokeDashArray = new DoubleCollection() { 4, 2 };
            p.Tag = linearMove;
            linearMove.GeometryPath = geometry;
            p.Data = geometry;

            if (isRapid == false)
            {
                p.StrokeThickness = 1;

                p.Stroke = GetLineColor(linearMove.LineColor);

                if (CanvasEventsEnabled)
                {
                    p.MouseDown += MouseClickEntity;
                    p.MouseEnter += MouseEnterEntity;
                    p.MouseLeave += MouseLeaveEntity;
                }
                else
                {
                    p.MouseDown -= MouseClickEntity;
                    p.MouseEnter -= MouseEnterEntity;
                    p.MouseLeave -= MouseLeaveEntity;
                }
            }
            else
            {
                p.Stroke = Brushes.DarkGray;
                p.Opacity = 0.2;
            }
            if (!isAxes)
                canvas1.Children.Add(p);
            else
            {
                canvasAxes.Children.Add(p);
            }
        }
        

        private void MouseClickEntity(object sender, MouseButtonEventArgs e)
        {
            var p = ((Path)sender);
            if (p.Tag != null)
            {
                if (axes.Contains(p.Tag as IBaseEntity))
                {
                    OnAxisClicked(p);

                }
                else 
                {   
                    OnEntityClicked(p);
                }
                
            }
        }


        private void MouseEnterEntity(object sender, MouseEventArgs e)
        {
            _originalColor = ((Path)sender).Stroke;
            ((Path)sender).Stroke = Brushes.Orange;
            ((Path)sender).StrokeThickness = 5;
        }

        private void MouseLeaveEntity(object sender, MouseEventArgs e)
        {
            ((Path)sender).Stroke = _originalColor;
            ((Path)sender).StrokeThickness = 1;
        }



        private void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        { 
            var canvas = sender as Canvas;
        
            if (canvas.Name == "canvas1")
            {
                previousCoordinate = Mouse.GetPosition(canvas1);
            }
            
        }

        public void RapidsOnOff()
        {

            foreach ( var child in canvas1.Children)
            {
                try
                {
                    Path c = child as Path;
                    ToolpathEntity element = c.Tag as ToolpathEntity;
                    if (!element.IsBeamOn)
                    {
                        c.Visibility = c.Visibility == Visibility.Visible ? Visibility.Hidden  : Visibility.Visible;
                    }
                }
                catch {continue;}
            }
        }


        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            //lbl.Text = $"X:{Mouse.GetPosition(canvas1).X.ToString("N0")} Y:{Mouse.GetPosition(canvas1).Y.ToString("N0")}";

            if (moves == null) return;

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                double rotSpeed = 200;

                if (Keyboard.IsKeyDown(Key.LeftShift)) { rotSpeed = rotSpeed / 100; }

                double vX = (Mouse.GetPosition(canvas1).Y - previousCoordinate.Y) / canvas1.ActualHeight;
                double vY = (Mouse.GetPosition(canvas1).X - previousCoordinate.X) / canvas1.ActualWidth;

                Matrix3D U = Matrix3D.Identity;
                Matrix3D Un = Matrix3D.Identity;

                double qRotAngle = Math.Pow(Math.Pow(vX, 2) + Math.Pow(vY, 2), 0.5) * rotSpeed;

                Vector3D vQ = new Vector3D(vX, -vY, 0);

                if (vQ.Length != 0)
                {
                    Quaternion Q = new Quaternion(vQ, qRotAngle);

                    U.RotateAt(Q, new Point3D(centerRotation.Y, centerRotation.X, centerRotation.Z));
                    Un.RotateAt(Q, new Point3D(0, 0, 0));

                    foreach (var item in moves)
                    {
                        item.Render(U, Un, true, 1);
                    }

                    U = Matrix3D.Identity;
                    Un = Matrix3D.Identity;
                    
                    U.RotateAt(Q, axes.OriginPoint);
                    Un.RotateAt(Q, new Point3D(0, 0, 0));
                    foreach (var item in axes)
                    {

                        item.Render(U, Un, true, 1);
                    }
                    
                }
                previousCoordinate = Mouse.GetPosition(canvas1);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Point3D cor = new Point3D(centerRotation.Y, centerRotation.X, centerRotation.Z);

                double vY = (Mouse.GetPosition(canvas1).Y - previousCoordinate.Y);
                double vX = (Mouse.GetPosition(canvas1).X - previousCoordinate.X);

                Matrix3D U = Matrix3D.Identity;
                Matrix3D Un = Matrix3D.Identity;

                U.OffsetX = vX;
                U.OffsetY = vY;

                cor = U.Transform(cor);

                centerRotation.X = cor.Y;
                centerRotation.Y = cor.X;
                centerRotation.Z = cor.Z;

                foreach (var item in moves)
                {
                    item.Render(U, Un, false, 1);
                }

                previousCoordinate = Mouse.GetPosition(canvas1);
            }
        }


        private void canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (moves == null) return;
            double Z = 1;

            if (e.Delta > 0)
            {
                Z = 1.1;
            }
            else if (e.Delta < 0)
            {
                Z = 1 / 1.1;
            }

            Point3D cor = new Point3D(centerRotation.Y, centerRotation.X, centerRotation.Z);

            Matrix3D U = Matrix3D.Identity;
            Matrix3D Un = Matrix3D.Identity;
            Point3D PZ = new Point3D(Mouse.GetPosition(canvas1).X, Mouse.GetPosition(canvas1).Y, cor.Z);

            U.ScaleAt(new Vector3D(Z, Z, Z), PZ);

            cor = U.Transform(cor);
            centerRotation.X = cor.Y;
            centerRotation.Y = cor.X;
            centerRotation.Z = cor.Z;

            foreach (var item in moves)
            {
                item.Render(U, Un, false, Z);
            }
        }


        #region Utility methods 
        public void ClearCanvas()
        {
            if (canvas1.Children != null)
                canvas1.Children.Clear();
            if (canvasAxes.Children != null)
                canvasAxes.Children.Clear();
        }

        public void SetCanvasEventsEnabled(bool newStatus)
        {
            CanvasEventsEnabled = newStatus;
        }

        public void SetUseDefaultEvents(bool use)
        {
            UseDefaultEvents= use;  
        }

        public void SetViewFromPlane(string plane)
        {
            if (isValidPlaneString(plane))
            {
                axes = new Axes();
                axes.BuildAxes();
                canvas1.Children.Clear();
                DrawProgram(Filename, false);
                InitialTransform(plane);            
            }

        }

        public bool isValidPlaneString(string plane) {
            return (plane == "XY" || plane == "XZ" || plane == "YZ") ? true : false;
        }

        #endregion


        #region Actions Default Logic 
        public void OnAxisClicked(Path p = null)
        {
            if (UseDefaultEvents)
            {
                if(p != null)
                {
                    IBaseEntity axe = p.Tag as IBaseEntity;
                    axes = new Axes();
                    axes.BuildAxes();
                    canvas1.Children.Clear();
                    DrawProgram(Filename, false);
                    InitialTransform(axe.Tag.ToString());
                }
            }

            AxisClicked?.Invoke(p);
        }

        private void OnEntityClicked(Path p)
        {
            if (UseDefaultEvents)
            {

            }
            EntityClicked?.Invoke(p);
        }

        public void TestRot()
        {
            Matrix3D U, Un;
            Point3D cor;
            U = Matrix3D.Identity;
            Un = Matrix3D.Identity;
            cor = new Point3D(centerRotation.Y, centerRotation.X, centerRotation.Z);
            Quaternion planeQuat = SetViewPlane("XZ");
            U.RotateAt(planeQuat, centerRotation);
            Un.RotateAt(planeQuat, new Point3D(0, 0, 0));

            foreach (var item in moves)
            {
                item.Render(U, Un, false, 1);
            }
        }

        #endregion

        #region Actions
        public Action<Path> EntityClicked;
        public Action<Path> AxisClicked;
        public Action KeyPressed;

        #endregion


    }
}
