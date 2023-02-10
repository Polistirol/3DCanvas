using ParserLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ParserLib.Helpers;
using System.Speech.Recognition.SrgsGrammar;
using System.Windows.Media.Animation;
using ParserLib.Interfaces;
using ParserLib.Models.Macros;
using ParserLib.Interfaces.Macros;

namespace ParserLib.Helpers
{
    public static class GeoHelper
    {
        public static double GetRadiusForCircularMove(Point3D A, Point3D B, Point3D C, out double CB, out double CA, out double AB)
        {
            //segments
            CB = Point3D.Subtract(C, B).Length;
            CA = Point3D.Subtract(C, A).Length;
            AB = Point3D.Subtract(A, B).Length;

            //circumradius
            double s = (CB + CA + AB) / 2;
            return CB * CA * AB / 4 / Math.Sqrt(s * (s - CB) * (s - CA) * (s - AB));

        }

        public static void Add2DMoveProperties(ref ArcMove move, bool isClockwise)
        {
            var i = move.ViaPoint.X;
            var j = move.ViaPoint.Y;
            var k = move.ViaPoint.Z;

            if (k == 0 && move.EndPoint.Z == 0) //pioano XY
            {
                move.Normal = new Vector3D(0, 0, 1);
            }
            if (i == 0 && move.EndPoint.X == 0)
            {
                move.Normal = new Vector3D(1, 0, 0);
            }
            if (j == 0 && move.EndPoint.Y == 0)
            {
                move.Normal = new Vector3D(0, 1, 0);
            }

            if (isClockwise == true)
            {
                move.Normal *= -1;
            }



            //only two of those are != 0
            move.Radius = Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2) + Math.Pow(k, 2));
            move.CenterPoint = new Point3D(move.StartPoint.X + i, move.StartPoint.Y + j, move.StartPoint.Z + k);
            Vector3D cSVector = Point3D.Subtract(move.StartPoint, move.CenterPoint);
            Vector3D cEVector = Point3D.Subtract(move.EndPoint, move.CenterPoint);
            cSVector.Normalize();
            cEVector.Normalize();

            if (move.EndPoint == move.StartPoint)
            {
                //is a circle
                double alpha = 0.01;
                Vector3D rotatedVector = MathHelpers.RotateAround(cSVector, move.Normal, -alpha);
                move.EndPoint = Point3D.Add(move.CenterPoint, rotatedVector * move.Radius);
                move.IsLargeArc = true;
            }
            else
 
                move.IsLargeArc = MathHelpers.IsLargeAngle(cSVector, cEVector, move.Normal);

            move.IsStroked = true;

        }


        public static void AddCircularMoveProperties(ref ArcMove move)
        {
            var A = move.StartPoint;
            var B = move.ViaPoint;
            var C = move.EndPoint;

            double r = GetRadiusForCircularMove(A, B, C, out double CB, out double CA, out double AB);

            move.Radius = r;

            if (double.IsInfinity(r))
            {
                //Radius impossible it's a linear
                move.Radius = 100000;
                return;
            }

            //Circumcenter
            double b1 = GetCircumPoint(CB, CA, AB);
            double b2 = GetCircumPoint(CA, CB, AB);
            double b3 = GetCircumPoint(AB, CB, CA);

            Vector3D v = new Vector3D(b1, b2, b3);
            Vector3D p1 = new Vector3D(A.X, B.X, C.X);
            Vector3D p2 = new Vector3D(A.Y, B.Y, C.Y);
            Vector3D p3 = new Vector3D(A.Z, B.Z, C.Z);
            Point3D centerPoint = new Point3D(Vector3D.DotProduct(v, p1) / (b1 + b2 + b3), Vector3D.DotProduct(v, p2) / (b1 + b2 + b3), Vector3D.DotProduct(v, p3) / (b1 + b2 + b3));

            //Verifico se è un large arc:
            //se l'angolo tra OB e OA oppure l'angolo tra OB e OC è maggiore di angolo tra OA e OC allora è un large arc
            Vector3D vIni = Point3D.Subtract(centerPoint, A); //vettore OA
            Vector3D vMed = Point3D.Subtract(centerPoint, B); //vettore OB
            Vector3D vEnd = Point3D.Subtract(centerPoint, C); //vettore OC
            move.CenterPoint = centerPoint;
            move.IsStroked = true;
            move.IsLargeArc = false;

            if (Vector3D.AngleBetween(vMed, vIni) > Vector3D.AngleBetween(vIni, vEnd) || Vector3D.AngleBetween(vMed, vEnd) > Vector3D.AngleBetween(vIni, vEnd))
            {
                move.IsLargeArc = true;
            }

            move.Normal = Vector3D.CrossProduct(Point3D.Subtract(A, B), Point3D.Subtract(A, C));
        }

        private static double GetCircumPoint(double q1, double q2, double q3)
        {
            return q1 * q1 * (q2 * q2 + q3 * q3 - q1 * q1);
        }

        public static void GetMoveFromMacroHole(ref HoleMoves hole)
        {
            //const double alpha = -0.0001745329;
            //Se da fastidio questa tolleranza bisogna introdurre L'elemento ellipse geometry che fà un cerchio

            LinearMove leadIn = hole.LeadIn as LinearMove;
            ArcMove circle = hole.Movements[1] as ArcMove;

            double maxClosingGap = 0.01; //mm
            double alpha = -maxClosingGap / hole.Radius;

            //Se si vedessero comportamenti strani, verificare che la direzione della normale segua la regola della mano sinistra rispetto al verso di percorrenza dell'arco.

            var normalVector = Point3D.Subtract(circle.CenterPoint, circle.NormalPoint);
            normalVector.Normalize();

            var vectorUp = (circle.CenterPoint.X == circle.NormalPoint.X && circle.CenterPoint.Y == circle.NormalPoint.Y) ? new Vector3D(1, 0, 0) : new Vector3D(0, 0, 1);
            Vector3D versor = new Vector3D();
            if (leadIn.StartPoint == circle.CenterPoint)
            {
                versor = Vector3D.CrossProduct(normalVector, vectorUp);
                versor.Normalize();
                leadIn.EndPoint = Point3D.Add(leadIn.StartPoint, versor * circle.Radius);
            }
            else
            {
                Vector3D centerToApproach = Point3D.Subtract(leadIn.StartPoint, circle.CenterPoint);
                var CALenght = centerToApproach.Length;
                centerToApproach.Normalize();
                leadIn.EndPoint = Point3D.Add(leadIn.StartPoint, centerToApproach * (circle.Radius - CALenght));
                versor.Normalize();
                versor = centerToApproach;
            }

            var vr = Vector3D.Multiply(versor, hole.Radius);
            var rotatedVector = MathHelpers.RotateAround(vr, normalVector, alpha);
            circle.StartPoint = Vector3D.Add(vr, circle.CenterPoint);
            circle.ViaPoint = Vector3D.Add(-vr, circle.CenterPoint);
            circle.EndPoint = Vector3D.Add(rotatedVector, circle.CenterPoint);

            circle.Normal = normalVector;
            circle.IsLargeArc = true;
            circle.Tag = hole;

            leadIn.Tag = hole;
        }

        public static void GetMovesFromMacroSlot(ref SlotMove slot)
        {

            var Line1 = slot.Movements[1];
            var Arc1 = slot.Movements[2] as ArcMove;
            var Line2 = slot.Movements[3];
            var Arc2 = slot.Movements[4] as ArcMove;

            var c1C2Vector = Point3D.Subtract(Arc1.CenterPoint, Arc2.CenterPoint);

            var c2C1Vector = Point3D.Subtract(Arc2.CenterPoint, Arc1.CenterPoint);
            c2C1Vector.Normalize();

            Arc1.NormalPoint = Point3D.Add(Arc2.NormalPoint, c1C2Vector);
            c1C2Vector.Normalize();

            var normalVectorC1 = Point3D.Subtract(Arc1.CenterPoint, Arc1.NormalPoint);
            normalVectorC1.Normalize();

            var normalVectorC2 = Point3D.Subtract(Arc2.CenterPoint, Arc2.NormalPoint);
            normalVectorC2.Normalize();

            var cAVersor = Vector3D.CrossProduct(normalVectorC1, c2C1Vector);
            cAVersor.Normalize();

            var cAVector = cAVersor * Arc1.Radius;
            var arc1StartPoint = Arc1.CenterPoint + cAVector;

            var cBVector = cAVersor * -Arc1.Radius;
            var arc1EndPoint = Arc1.CenterPoint + cBVector;

            var arc1ViaPoint = Point3D.Add(Arc1.CenterPoint, c2C1Vector * -Arc1.Radius);

            var cCVersor = Vector3D.CrossProduct(normalVectorC2, c1C2Vector);
            cCVersor.Normalize();

            var cCVector = cCVersor * Arc2.Radius;
            var arc2StartPoint = Arc2.CenterPoint + cCVector;

            var cDVector = cCVersor * -Arc2.Radius;
            var arc2EndPoint = Arc2.CenterPoint + cDVector;

            var arc2ViaPoint = Point3D.Add(Arc2.CenterPoint, c1C2Vector * -Arc2.Radius);

            Arc1.StartPoint = arc1StartPoint;
            Arc1.EndPoint = arc1EndPoint;
            Arc1.ViaPoint = arc1ViaPoint;
            Arc1.Normal = normalVectorC1;
            Arc1.Tag = slot;

            Arc2.StartPoint = arc2StartPoint;
            Arc2.EndPoint = arc2EndPoint;
            Arc2.ViaPoint = arc2ViaPoint;
            Arc2.Normal = normalVectorC2;
            Arc2.Tag = slot;

            Line1.StartPoint = Arc1.EndPoint;
            Line1.EndPoint = Arc2.StartPoint;
            Line1.Tag = slot;

            Line2.StartPoint = Arc2.EndPoint;
            Line2.EndPoint = Arc1.StartPoint;
            Line2.Tag = slot;
            //lead in
            slot.LeadIn.EndPoint = MathHelpers.GetClosestPoint(slot.LeadIn.StartPoint, new List<Point3D> {
                Line1.StartPoint,
                Line1.EndPoint,
                Line2.StartPoint,
                Line2.EndPoint,
            });

            slot.LeadIn.Tag = slot;
            GetMacroOrderedArray(slot);
        }

        private static void GetMacroOrderedArray(Macro macro)
        {
            var leadin = macro.LeadIn;
            if (leadin != null)
            {
                List<ToolpathEntity> list = new List<ToolpathEntity>(macro.Movements);
                list[0] = leadin;
                list[1] = macro.Movements.FirstOrDefault(x => x.StartPoint == leadin.EndPoint);
                for (int i = 2; i < macro.Movements.Count; i++)
                {
                    list[i] = macro.Movements.FirstOrDefault(x => x.StartPoint == list[i - 1].EndPoint);
                }
                macro.Movements = list;
            }
        }
        public static void GetMovesFromMacroKeyhole(ref KeyholeMoves keyhole)
        {

            var Arc1 = keyhole.Movements[1] as ArcMove;
            var Arc2 = keyhole.Movements[2] as ArcMove;
            var Line1 = keyhole.Movements[3];
            var Line2 = keyhole.Movements[4];

            var c1C2Vector = Point3D.Subtract(Arc2.CenterPoint, Arc1.CenterPoint);
            var c2C1Vector = Point3D.Subtract(Arc1.CenterPoint, Arc2.CenterPoint);

            Arc1.NormalPoint = Point3D.Add(Arc2.NormalPoint, c2C1Vector);
            var normalVectorC1 = Point3D.Subtract(Arc1.CenterPoint, Arc1.NormalPoint);
            var normalVectorC2 = Point3D.Subtract(Arc2.CenterPoint, Arc2.NormalPoint);
            c1C2Vector.Normalize();
            normalVectorC1.Normalize();
            normalVectorC2.Normalize();

            var d2 = Vector3D.CrossProduct(c2C1Vector, normalVectorC2);
            d2.Normalize();
            var p2 = Point3D.Subtract(Arc1.CenterPoint, (Arc1.Radius * d2));
            var tmp = Math.Sqrt((Arc1.Radius * Arc1.Radius) - (Arc2.Radius * Arc2.Radius));

            var p3 = Point3D.Subtract((Arc1.CenterPoint + (c1C2Vector * tmp)), (Arc2.Radius * d2));
            var p4 = Point3D.Subtract(Arc2.CenterPoint, (Arc2.Radius * d2));
            var p5 = Point3D.Add(Arc2.CenterPoint, (Arc2.Radius * c1C2Vector));
            var p6 = Point3D.Add(Arc2.CenterPoint, (Arc2.Radius * d2));
            var p7 = Point3D.Add(p3, (2 * Arc2.Radius * d2));

            Arc1.StartPoint = p7;
            Arc1.EndPoint = p3;
            Arc1.ViaPoint = p2;
            Arc1.Normal = normalVectorC1;
            Arc1.Tag = keyhole;

            Line1.StartPoint = p3;
            Line1.EndPoint = p4;
            Line1.Tag = keyhole;

            Arc2.StartPoint = p4;
            Arc2.EndPoint = p6;
            Arc2.ViaPoint = p5;
            Arc2.Normal = normalVectorC2;
            Arc2.Tag = keyhole;

            Line2.StartPoint = p6;
            Line2.EndPoint = p7;
            Line2.Tag = keyhole;

            //lead in
            keyhole.LeadIn.EndPoint = MathHelpers.GetClosestPoint(keyhole.LeadIn.StartPoint, new List<Point3D> {
                Line1.StartPoint,
                Line1.EndPoint,
                Line2.StartPoint,
                Line2.EndPoint,

            });
            keyhole.LeadIn.Tag = keyhole;
            GetMacroOrderedArray(keyhole);

        }

        public static void GetMovesFromMacroPoly(ref PolyMoves poly)
        {
            //calcolo di quanti gradi devo ruotare il primo segmento
            double alpha = 2 * Math.PI / poly.Sides;

            //la normale della poly è presa rispetto al vertice, 

            //vettore dal centro al vertice 
            var vertexCenterVector = Point3D.Subtract(poly.CenterPoint, poly.VertexPoint);
            //centerVertexVector.Normalize();
            //vettore dal vertice al centro 
            var centerVertexVector = Point3D.Subtract(poly.VertexPoint, poly.CenterPoint);
            //vertexCenterVector.Normalize();
            //punto sulla nomrale partendo dal centro, sposto il punto normale parsificato sul centro della poly
            var normalCenterPoint = Point3D.Add(poly.NormalPoint, vertexCenterVector);
            //vettore dal centro alla nnuova normale
            var normalVector = Point3D.Subtract(normalCenterPoint, poly.CenterPoint);
            normalVector.Normalize();

            // calcolo i nuovi vettori

            var vertices = new Point3D[poly.Sides + 1];

            var angle = 0.0;
            for (int i = 0; i < poly.Sides; i++)
            {
                angle = alpha * i;
                var rotatedVector = centerVertexVector * Math.Cos(angle) + Vector3D.CrossProduct(normalVector, centerVertexVector) * Math.Sin(angle) + normalVector * (Vector3D.DotProduct(normalVector, centerVertexVector) * (1 - Math.Cos(angle)));
                var newVertex = Point3D.Add(poly.CenterPoint, rotatedVector);
                vertices[i] = newVertex;
            }
            // aggiungo in coda ai vertici il primo vertice
            //vertices[poly.Sides] = poly.VertexPoint; // puoi controllare che stia funzionando bene verificando che  poly.Vertices[0] == poly.VertexPoint;

            //lead in

            int index;
            (poly.LeadIn.EndPoint, index) = MathHelpers.GetClosestPointID(poly.LeadIn.StartPoint, vertices);
            poly.LeadIn.Tag = poly;
            vertices[poly.Sides] = poly.VertexPoint;
            while (poly.Movements.Count <= poly.Sides)
            {
                poly.Movements.Add(
                     new LinearMove()
                     {
                         StartPoint = vertices[index],
                         EndPoint = vertices[index + 1],
                         SourceLine = poly.SourceLine,
                         IsBeamOn = poly.IsBeamOn,
                         LineColor = poly.LineColor,
                         OriginalLine = poly.OriginalLine,
                         Tag = poly,
                     });
                index++;
                if (index >= poly.Sides) index = 0;
            }

        }

        public static void GetMovesFromMacroRect(ref RectMoves rect)
        {
            var cv1 = Point3D.Subtract(rect.CenterPoint, rect.VertexPoint);
            cv1.Normalize();
            var d1 = Point3D.Subtract(rect.SidePoint, rect.VertexPoint);
            d1.Normalize();


            var alpha = Math.Acos(Vector3D.DotProduct(d1, cv1));

            var normal = Vector3D.CrossProduct(cv1, d1);

            var d2 = Vector3D.CrossProduct(d1, normal);
            d2.Normalize();

            var modCV1 = Point3D.Subtract(rect.CenterPoint, rect.VertexPoint).Length;

            var l1 = modCV1 * Math.Cos(alpha);
            var l2 = modCV1 * Math.Sin(alpha);

            var mAB = Point3D.Subtract(rect.CenterPoint, (d2 * l2));
            var mBC = Point3D.Add(rect.CenterPoint, (d1 * l1));
            var mCD = Point3D.Add(rect.CenterPoint, (d2 * l2));
            var mDA = Point3D.Subtract(rect.CenterPoint, (d1 * l1));

            var vA = rect.VertexPoint;
            var vB = Point3D.Add(vA, (2 * l1 * d1));
            var vC = Point3D.Add(vB, (2 * l2 * d2));
            var vD = Point3D.Add(vC, (-2 * l1 * d1));

            Point3D[] movesTarget = new Point3D[7] { vA, vB, vC, vD, vA, vB, vC };
            rect.LeadIn.EndPoint = MathHelpers.GetClosestPoint(rect.LeadIn.StartPoint, new Point3D[] { mAB, mBC, mCD, mDA });
            int index = 0;
            Point3D _unused;
            (_unused, index) = MathHelpers.GetClosestPointID(rect.LeadIn.EndPoint, new Point3D[] { vA, vB, vC, vD }); ;

            rect.Movements.Add(
                new LinearMove()
                {
                    StartPoint = rect.LeadIn.EndPoint,
                    EndPoint = movesTarget[index],
                    SourceLine = rect.SourceLine,
                    IsBeamOn = rect.IsBeamOn,
                    LineColor = rect.LineColor,
                    OriginalLine = rect.OriginalLine,
                    Tag = rect
                }
            );

            for (int i = 0; i < 3; i++)
            {
                rect.Movements.Add(
                    new LinearMove()
                    {
                        StartPoint = movesTarget[index + i],
                        EndPoint = movesTarget[index + i + 1],
                        SourceLine = rect.SourceLine,
                        IsBeamOn = rect.IsBeamOn,
                        LineColor = rect.LineColor,
                        OriginalLine = rect.OriginalLine,
                        Tag = rect
                    }
                );
            }
            rect.Movements.Add(
                new LinearMove()
                {
                    StartPoint = movesTarget[index + 3],
                    EndPoint = rect.Movements[1].EndPoint,
                    SourceLine = rect.SourceLine,
                    IsBeamOn = rect.IsBeamOn,
                    LineColor = rect.LineColor,
                    OriginalLine = rect.OriginalLine,
                    Tag = rect
                });

            rect.LeadIn.Tag = rect;
        }

        public static class StringToFormula
        {
            private static string[] _operators = { "-", "+", ":", "*", "^" };
            private static Func<double, double, double>[] _operations = {
        (a1, a2) => a1 - a2,
        (a1, a2) => a1 + a2,
        (a1, a2) => a1 / a2,
        (a1, a2) => a1 * a2,
        (a1, a2) => Math.Pow(a1, a2)
        };

            public static double Eval(string expression)
            {
                List<string> tokens = getTokens(expression);
                Stack<double> operandStack = new Stack<double>();
                Stack<string> operatorStack = new Stack<string>();
                int tokenIndex = 0;

                if (expression.Contains("MVX")) expression = expression.Replace("MVX", "0.0");
                if (expression.Contains("MVY")) expression = expression.Replace("MVY", "0.0");
                if (expression.Contains("MVZ")) expression = expression.Replace("MVZ", "0.0");

                while (tokenIndex < tokens.Count)
                {
                    string token = tokens[tokenIndex];

                    if (token.Equals("MVX")) token = token.Replace("MVX", "0.0");
                    else if (token.Equals("MVY")) token = token.Replace("MVY", "0.0");
                    else if (token.Equals("MVZ")) token = token.Replace("MVZ", "0.0");

                    if (token == "(")
                    {
                        string subExpr = getSubExpression(tokens, ref tokenIndex);
                        operandStack.Push(Eval(subExpr));
                        continue;
                    }
                    if (token == ")")
                    {
                        throw new ArgumentException("Mis-matched parentheses in expression");
                    }
                    //If this is an operator
                    if (Array.IndexOf(_operators, token) >= 0)
                    {
                        while (operatorStack.Count > 0 && Array.IndexOf(_operators, token) < Array.IndexOf(_operators, operatorStack.Peek()))
                        {
                            string op = operatorStack.Pop();
                            double arg2 = operandStack.Pop();
                            double arg1 = operandStack.Pop();
                            operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                        }
                        operatorStack.Push(token);
                    }
                    else
                    {
                        operandStack.Push(double.Parse(token));
                    }
                    tokenIndex += 1;
                }

                while (operatorStack.Count > 0)
                {
                    string op = operatorStack.Pop();
                    double arg2 = operandStack.Pop();
                    double arg1 = operandStack.Pop();
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                }
                return operandStack.Pop();
            }

            private static string getSubExpression(List<string> tokens, ref int index)
            {
                StringBuilder subExpr = new StringBuilder();
                int parenlevels = 1;
                index += 1;
                while (index < tokens.Count && parenlevels > 0)
                {
                    string token = tokens[index];
                    if (tokens[index] == "(")
                        parenlevels += 1;
                    if (tokens[index] == ")")
                        parenlevels -= 1;
                    if (parenlevels > 0)
                        subExpr.Append(token);
                    index += 1;
                }

                if ((parenlevels > 0))
                    throw new ArgumentException("Mis-matched parentheses in expression");
                return subExpr.ToString();
            }

            private static List<string> getTokens(string expression)
            {
                string operators = "()^*:+-";
                List<string> tokens = new List<string>();
                StringBuilder sb = new StringBuilder();
                int counter = -1;
                foreach (char c in expression.Replace(" ", string.Empty))
                {
                    counter++;
                    if (operators.IndexOf(c) >= 0 && counter > 0)
                    {
                        if ((sb.Length > 0))
                        {
                            tokens.Add(sb.ToString());
                            sb.Length = 0;
                        }
                        tokens.Add(c.ToString());
                    }
                    else
                        sb.Append(c);
                }
                if ((sb.Length > 0))
                    tokens.Add(sb.ToString());
                return tokens;
            }
        }


    }
}
