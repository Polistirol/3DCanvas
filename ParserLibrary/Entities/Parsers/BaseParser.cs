using ParserLibrary.Helpers;
using ParserLibrary.Interfaces;
using ParserLibrary.Models;
using ParserLibrary.Models.Macros;
using ParserLibrary.Models.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Entities.Parsers
{
    public class BaseParser
    {
        public ProgramContext ProgramContext { get; private set; }
        private IValuesMap _valuesMap;


        public BaseParser()
        {
            ProgramContext = new ProgramContext
            {
                ReferenceMove = new LinearMove { EndPoint = new Point3D(0, 0, 0) },
                LastEntity = new LinearMove { EndPoint = new Point3D(0, 0, 0) },
                LastHeadPosition = new Point3D(0, 0, 0),
                RotoTranslation = new RotoTranslation(),
                VariablesDict = new Dictionary<string, string>(),
            };

        }

        #region Adderes
        public void AddMovementToMoves(ValuesMap valuesMap)
        {
            _valuesMap = valuesMap;

            FillNullValueFromTCP();
            GetValuesFromLocalVariables();

            ToolpathEntity move = new object() as ToolpathEntity;
            switch (_valuesMap.InstructionMode)
            {
                case EInstructionMode.Linear:
                    move = CreateLinearMove();
                    break;

                case EInstructionMode.Arc3Points:
                case EInstructionMode.ArcClockwise:
                case EInstructionMode.ArcCounterClockwise:
                    move = CreateArcMove();
                    if ((move as ArcMove).Radius > 99000)
                        move = GetLinearMoveFromCircular(move as ArcMove);
                    break;

                default:
                    throw new Exception("Movement type was unexpected ");
            };
            SyncMoveWithContext(ref move, _valuesMap);
        
            ProgramContext.Moves.Add(move);
            //sarebbe da triggerare evento 
            ProgramContext.LastHeadPosition = move.EndPoint;

        }



        public void AddMacroToMoves(MacroValueMap valueMap)
        {
            _valuesMap = valueMap;
            GetValuesFromLocalVariables();
            MacroValueMap vm = _valuesMap as MacroValueMap;
            Macro entity = null;
            // create leadIn
            LinearMove leadIn = GetSyncedNewToolpathEntity(EEntityType.Line, vm) as LinearMove;
            leadIn.IsLeadIn = true;

            switch (valueMap.InstructionMode)
            {
                case EInstructionMode.Hole:
                    entity = CreateHoleMoves(vm, leadIn);
                    break;

                case EInstructionMode.Slot:
                    entity = CreateSlotMoves(vm, leadIn);
                    break;

                case EInstructionMode.Rect:
                    entity = CreateRectMoves(vm, leadIn);
                    break;

                case EInstructionMode.Poly:
                    entity = CreatePolyMoves(vm, leadIn);   
                    break;

                case EInstructionMode.Keyhole:
                    entity = CreateKeyholeMoves(vm, leadIn);
                    break;

            }
            SyncMacroWithContext(ref entity, vm);
            ProgramContext.Moves.Add(entity);
        }





        #endregion

        private LinearMove CreateLinearMove()
        {
            LinearMove move = new LinearMove();
            double x, y, z;
            ValuesMap valueMap = _valuesMap as ValuesMap;
            x = double.Parse(valueMap.X);
            y = double.Parse(valueMap.Y);
            z = double.Parse(valueMap.Z);
            move.StartPoint = ProgramContext.LastHeadPosition;
            Point3D endPoint = new Point3D();
            if (ProgramContext.IsIncremental)
            {
                // if program is incremental, the valuesMap values are added to the starting position
                endPoint.X = ProgramContext.LastHeadPosition.X + x;
                endPoint.Y = ProgramContext.LastHeadPosition.Y + y;
                endPoint.Z = ProgramContext.LastHeadPosition.Z + z;
            }
            else
            {
                endPoint = new Point3D(x, y, z);
            }

            //Add rototranslation
            move.EndPoint = RotoTraslPoint(endPoint);
            return move;
        }


        private ToolpathEntity CreateArcMove()
        {
            ArcMove move = new ArcMove();
            double x, y, z, i, j, k;
            ValuesMap valueMap = _valuesMap as ValuesMap;
            x = double.Parse(valueMap.X);
            y = double.Parse(valueMap.Y);
            z = double.Parse(valueMap.Z);
            i = double.Parse(valueMap.I);
            j = double.Parse(valueMap.J);
            k = double.Parse(valueMap.K);
            move.StartPoint = ProgramContext.LastHeadPosition;
            Point3D viaPoint = new Point3D();
            Point3D endPoint = new Point3D();

                if (ProgramContext.IsIncremental)
                {
                    // if program is incremental, the valuesMap values are added to the starting position
                    endPoint.X = ProgramContext.LastHeadPosition.X + x;
                    endPoint.Y = ProgramContext.LastHeadPosition.Y + y;
                    endPoint.Z = ProgramContext.LastHeadPosition.Z + z;

                    if (valueMap.InstructionMode == EInstructionMode.Arc3Points)
                    {
                        viaPoint.X = ProgramContext.LastHeadPosition.X + i;
                        viaPoint.Y = ProgramContext.LastHeadPosition.Y + j;
                        viaPoint.Z = ProgramContext.LastHeadPosition.Z + k;
                    }
                    else
                        viaPoint = new Point3D(i, j, k);
                }
                else
                {
                    viaPoint = new Point3D(i, j, k);
                    endPoint = new Point3D(x, y, z);
                }
            
            //Add rototranslation
            move.ViaPoint =  RotoTraslPoint(viaPoint);
            move.EndPoint =  RotoTraslPoint(endPoint);

            switch (valueMap.InstructionMode)
            {
                case EInstructionMode.Arc3Points:
                    GeoHelper.AddCircularMoveProperties(ref move);
                    break;
                case EInstructionMode.ArcClockwise:
                    GeoHelper.Add2DMoveProperties(ref move, isClockwise : true);
                    break;                
                case EInstructionMode.ArcCounterClockwise:
                    GeoHelper.Add2DMoveProperties(ref move, isClockwise : false);
                    break;
            }
           
            return move;
        }




        /// <summary>
        /// Creates a linear move with Arcmove data, if the radius of the arcmove is too big
        /// </summary>
        /// <returns></returns>
        private LinearMove GetLinearMoveFromCircular(ArcMove move)
        {
            LinearMove newLine = new LinearMove();
            newLine.StartPoint = move.StartPoint;
            newLine.EndPoint = move.EndPoint;
            newLine.OriginalLine = move.OriginalLine;
            newLine.SourceLine = move.SourceLine;
            newLine.IsBeamOn = move.IsBeamOn;
            newLine.LineColor = move.LineColor;
            return newLine;
        }

        private HoleMoves CreateHoleMoves(MacroValueMap vm, LinearMove leadIn)
        {
            HoleMoves entity = new HoleMoves
            {
                Radius = double.Parse(vm.Radius),
                Normal = vm.Normal,
                Center = vm.Center1,
                Movements = new List<ToolpathEntity>()
            };

            //Add rototranslation
            Point3D rotatedCenter = RotoTraslPoint(vm.Center1);
            Point3D rotatedNormal = RotoTraslPoint(vm.Normal);

            //create hole arc movement
            ArcMove holeArc = GetSyncedNewToolpathEntity(EEntityType.Arc, vm) as ArcMove;
            holeArc.Radius = vm.ToDouble(vm.Radius);
            holeArc.CenterPoint = rotatedCenter;
            holeArc.NormalPoint = rotatedNormal;

            //add to hole leadin and arc movement
            entity.Movements.Add(leadIn);
            entity.Movements.Add(holeArc);
            //var holeMacro = entity as HoleMoves;
            GeoHelper.GetMoveFromMacroHole(ref entity);
            return entity;
        }
        private SlotMove CreateSlotMoves (MacroValueMap vm , LinearMove leadIn)
        {
            SlotMove entity = new SlotMove()
            {
                Center1 = vm.Center1,
                Center2 = vm.Center2,
                Radius = double.Parse(vm.Radius),
                Normal = vm.Normal,
                Movements = new List<ToolpathEntity>()
            };
            //Add rototranslation
            Point3D rotatedCenter1 = RotoTraslPoint(vm.Center1);
            Point3D rotatedCenter2 = RotoTraslPoint(vm.Center2);
            Point3D rotatedNormal = RotoTraslPoint(vm.Normal);

            LinearMove line1 = GetSyncedNewToolpathEntity(EEntityType.Line, vm) as LinearMove;
            ArcMove arc1 = GetSyncedNewToolpathEntity(EEntityType.Arc, vm) as ArcMove;
            arc1.Radius = vm.ToDouble(vm.Radius);
            arc1.CenterPoint = rotatedCenter1;
            arc1.NormalPoint = rotatedNormal;
            LinearMove line2 = GetSyncedNewToolpathEntity(EEntityType.Line, vm) as LinearMove;
            ArcMove arc2 = GetSyncedNewToolpathEntity(EEntityType.Arc, vm) as ArcMove;
            arc2.Radius = vm.ToDouble(vm.Radius);
            arc2.CenterPoint = rotatedCenter2;
            arc2.NormalPoint = rotatedNormal;
            entity.Movements.Add(leadIn);
            entity.Movements.Add(line1);
            entity.Movements.Add(arc1);
            entity.Movements.Add(line2);
            entity.Movements.Add(arc2);
            GeoHelper.GetMovesFromMacroSlot(ref entity);
            return entity;
        }

        private RectMoves CreateRectMoves (MacroValueMap vm , LinearMove leadIn)
        {
            RectMoves entity = new RectMoves
            {
                CenterPoint = vm.Center1,
                VertexPoint = vm.Vertex1,
                SidePoint = vm.Side1,
                Movements = new List<ToolpathEntity>()
            };
            //Add Rototraslation 
            entity.CenterPoint = RotoTraslPoint(vm.Center1);
            entity.VertexPoint = RotoTraslPoint(vm.Vertex1);
            entity.SidePoint = RotoTraslPoint(vm.Side1);

            entity.Movements.Add(leadIn);
            
            GeoHelper.GetMovesFromMacroRect(ref entity);
            return entity;
        }

        private PolyMoves CreatePolyMoves(MacroValueMap vm, LinearMove leadIn)
        {
            PolyMoves entity = new PolyMoves
            {
                CenterPoint = vm.Center1,
                VertexPoint = vm.Vertex1,
                NormalPoint = vm.Normal,
                Sides = int.Parse(vm.Sides),
                Movements = new List<ToolpathEntity>()
            };
            //Add Rototraslation 
            entity.CenterPoint = RotoTraslPoint(vm.Center1);
            entity.VertexPoint = RotoTraslPoint(vm.Vertex1);
            entity.NormalPoint = RotoTraslPoint(vm.Normal);

            entity.Movements.Add(leadIn);
            GeoHelper.GetMovesFromMacroPoly(ref entity);
            return entity;
        }

        private KeyholeMoves CreateKeyholeMoves(MacroValueMap vm , LinearMove leadIn)
        {
            KeyholeMoves entity = new KeyholeMoves()
            {
                Center1 = vm.Center1,
                Center2 = vm.Center2,
                BigRadius = double.Parse(vm.BigRadius),
                SmallRadius = double.Parse(vm.SmallRadius),
                Normal = vm.Normal,
                Movements = new List<ToolpathEntity>()
            };

            //Add rototranslation
            Point3D rotatedCenter1 = RotoTraslPoint(vm.Center1);
            Point3D rotatedCenter2 = RotoTraslPoint(vm.Center2);
            Point3D rotatedNormal = RotoTraslPoint(vm.Normal);

            ArcMove arc1 = GetSyncedNewToolpathEntity(EEntityType.Arc, vm) as ArcMove;
            arc1.Radius = vm.ToDouble(vm.BigRadius);
            arc1.CenterPoint = rotatedCenter1;
            arc1.NormalPoint = rotatedNormal;

            ArcMove arc2 = GetSyncedNewToolpathEntity(EEntityType.Arc, vm) as ArcMove;
            arc2.Radius = vm.ToDouble(vm.SmallRadius);
            arc2.CenterPoint = rotatedCenter2;
            arc2.NormalPoint = rotatedNormal;

            LinearMove line1 = GetSyncedNewToolpathEntity(EEntityType.Line, vm) as LinearMove;
            LinearMove line2 = GetSyncedNewToolpathEntity(EEntityType.Line, vm) as LinearMove;

            entity.Movements.Add(leadIn);
            entity.Movements.Add(arc1);
            entity.Movements.Add(arc2);
            entity.Movements.Add(line1);
            entity.Movements.Add(line2);
            GeoHelper.GetMovesFromMacroKeyhole(ref entity);
            return  entity;
        }


        /// <summary>
        /// Updates the program context to beam on !Piercing type not managed
        /// </summary>
        /// <param name="piercing"></param>
        /// <param name="color"></param>
        internal void AddProcessStart(int piercing, int color)
        {
            ProgramContext.IsBeamOn = true;
            if (ProgramContext.IsMarkingProgram == true)
                ProgramContext.ContourLineType = ELineType.Marking;
            else
                ProgramContext.ContourLineType = (ELineType)color;
        }

        internal void SetAbsoluteMode()
        { ProgramContext.IsIncremental = false; }

        internal void SetIncrementalMode()
        { ProgramContext.IsIncremental = true; }

        internal void SetProgrammingMode(EInstructionMode? mode)
        {
            if (mode == EInstructionMode.AbsoluteProg) ProgramContext.IsIncremental = false;
            else ProgramContext.IsIncremental = true;
        }

        internal void AddRototraslation(ValuesMap valuesMap, EInstructionMode? mode)
        {
            //GetValuesFromLocalVariables();
            //set all null values to 0
            foreach (string axe in InstructionsMap.GetAxisNeeded(mode))
            {
                if (valuesMap.GetAttribute(axe) == null)
                    valuesMap.SetAttribute(axe, "0");
            }

            Vector3D rotationVector = valuesMap.RotationVector;
            Vector3D translationVector = valuesMap.TranslationVector;
            if (mode == EInstructionMode.GlobalRotoTrasl)
            {
                ProgramContext.RotoTranslation.UpdateRotoTraslComponents(rotationVector, translationVector);
            }
            else //local
            {
                ProgramContext.RotoTranslation.UpdateRotoTraslComponents(rotationVector, translationVector, false);
            }
        }



        /// <summary>
        /// Changes the program contox contour line to rapid and sets isBeamOn to false
        /// </summary>
        /// <param name="color">Optional, Unused at the moment</param>
        internal void AddProcessStop(int color = 0)
        {
            ProgramContext.IsBeamOn = false;
            ProgramContext.ContourLineType = ELineType.Rapid;
        }




        /// <summary>
        /// Check that the values needed are not null, if they are , they are updatet with the TCP value
        /// </summary>
        /// <param name="axisNeeded"></param>
        private void FillNullValueFromTCP()
        {
            Point3D tcp = ProgramContext.LastHeadPosition;
            ValuesMap valueMap = _valuesMap as ValuesMap;
            if (ProgramContext.IsIncremental == false)
            {
                if (valueMap.GetAttribute("X") == null)
                    valueMap.X = tcp.X.ToString();
                if (valueMap.GetAttribute("Y") == null)
                    valueMap.Y = tcp.Y.ToString();
                if (valueMap.GetAttribute("Z") == null)
                    valueMap.Z = tcp.Z.ToString();

                if (_valuesMap.InstructionMode == EInstructionMode.Arc3Points)
                {
                    if (valueMap.GetAttribute("I") == null)
                        valueMap.I = tcp.X.ToString();
                    if (valueMap.GetAttribute("J") == null)
                        valueMap.J = tcp.Y.ToString();
                    if (valueMap.GetAttribute("K") == null)
                        valueMap.K = tcp.Z.ToString();
                }
            }
            else
            {
                foreach (string axe in valueMap.AxisNedded)
                {
                    if (valueMap.GetAttribute(axe) == null)
                        valueMap.SetAttribute(axe, "0");
                }
            }
        }
        /// <summary>
        /// Checks if the value of the axis needed are strings not double, if so, checks in the variables dict for the corresponding value, if not found: throws  
        /// </summary>
        /// <param name="axisNeeded"></param>
        /// <exception cref="Exception"></exception>
        private void GetValuesFromLocalVariables()
        {
            string[] axisNeeded = _valuesMap.AxisNedded;
            foreach (string axe in axisNeeded)
            {
                string value = _valuesMap.GetAttribute(axe);
                if (value != null)
                {
                    //check if is not a number already 
                    if (double.TryParse(value, out var doubleValue) == false)
                    {
                        // if  not a number, take the number from the variables
                        string variableName = value.ToLower();
                        if (ProgramContext.VariablesDict.ContainsKey(value))
                            _valuesMap.SetAttribute(axe, ProgramContext.VariablesDict[value]);

                        else
                        //TODO : check also the machine variables before throwing
                        // the value is a string not in the dict, could be a written math expression
                        {
                            double eval = EvaluateMathExpression(value);
                        }

                            
                            throw new Exception("Local variable not found in dict ");
                    }
                }
            }
        }

        private Point3D RotoTraslPoint(Point3D point)
        {
          return   RototranslationHelper.GetRotoTranslatedPoint(ProgramContext, point);
        }

        /// <summary>
        /// Returns a linerMove or a Arcmove from the MacroValueMap, is used to create the single movements of a macro
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        private ToolpathEntity GetSyncedNewToolpathEntity(EEntityType entityType, MacroValueMap vm)
        {
            if (entityType == EEntityType.Line)
            {
                ToolpathEntity newLine = new LinearMove();
                SyncMoveWithContext(ref newLine, vm);
                return newLine;
            }
            else
            {
                ToolpathEntity newLine = new ArcMove();
                (newLine as ArcMove).IsLargeArc = true;
                (newLine as ArcMove).IsStroked = true;
                SyncMoveWithContext(ref newLine, vm);
                return newLine;
            }
        }

        /// <summary>
        /// Add to the Move all the Program Context data (beam on , color ecc), plus the original lines
        /// </summary>
        /// <param name="move"></param>
        /// <param name="valuesMap"></param>
        private void SyncMoveWithContext(ref ToolpathEntity move, IValuesMap valuesMap)
        {
            move.IsBeamOn = ProgramContext.IsBeamOn;
            move.LineColor = ProgramContext.ContourLineType;
            move.SourceLine = valuesMap.SourceLine;
            move.StartPoint = ProgramContext.LastHeadPosition;
            move.OriginalLine = valuesMap.OriginalLine;
            move.OriginalStartPoint = move.StartPoint;
            move.OriginalEndPoint = move.EndPoint;
        }

        /// <summary>
        /// Add to the Move all the Program Context data (beam on , color ecc), plus the original lines
        /// </summary>
        /// <param name="move"></param>
        /// <param name="valuesMap"></param>
        private void SyncMacroWithContext(ref Macro move, IValuesMap valuesMap)
        {
            move.IsBeamOn = ProgramContext.IsBeamOn;
            move.LineColor = ProgramContext.ContourLineType;
            move.SourceLine = valuesMap.SourceLine;
            move.OriginalLine = valuesMap.OriginalLine;
        }




        public static double EvaluateMathExpression(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }




    }
}
