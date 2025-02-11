﻿using ParserLibrary.Helpers;
using ParserLibrary.Interfaces;
using ParserLibrary.Interfaces.Macros;
using ParserLibrary.Models;
using ParserLibrary.Models.Macros;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParserLibrary.Models.Media;
using static ParserLibrary.Helpers.GeoHelper;
using static ParserLibrary.Helpers.TechnoHelper;
using ParserLibrary.Entities.Parsers;

namespace ParserLibrary.Services.Parsers
{
    public class ParseIso : IParser
    {
        public string PartProgramFilePath { get; set; }
        private Regex SubCallRegex;
        private Regex GcodeAxesQuotaRegex;
        private Regex VariableDeclarationRegex;
        private Regex MacroParsRegex;
        private double _conversionValue = 1;
        private Dictionary<string, List<Tuple<int, string>>> _dicSubprograms;
        private static int _indexOfM30;
        private static int _programEndAtLine = 0;

        //private string GcodeAxesQuotaPattern = @"[A-Za-z]+\=?[A-Za-z]*[+-]?\d*\.?\d*([+-]?[*:]?[A-Za-z]*[+-]?\d*\.?\d*)+\b";
        private string GcodeAxesQuotaPattern = @"[A-Za-z]+\=?[A-Za-z]*[+-]?\d*\.?\d*([+-]?[*:]?[A-Za-z]*[+-]?\d*\.?\d*)+\b";
        private string VariableDeclarationPattern = @"\b[A-Za-z]+[\d]+([\=]?[A-Za-z]*[+-]?[*:]?\d*\.?\d*)+\b";
        private string macroParsPattern = @"([+-]?[*:]?\d+\.?\d*)+";
        private string variableRegexPattern = @"^(P\d+|LV\d+)";
        private string subCallPattern = @"^(Q\d+|N\d+)";
        private char[] mathSymbols = new char[] { '-', '+', '*', ':' };

        public ParseIso(string fileName)
        {
            PartProgramFilePath = fileName;
            SubCallRegex = new Regex(subCallPattern, RegexOptions.IgnoreCase);
            GcodeAxesQuotaRegex = new Regex(GcodeAxesQuotaPattern, RegexOptions.IgnoreCase);
            VariableDeclarationRegex = new Regex(VariableDeclarationPattern, RegexOptions.IgnoreCase);
            MacroParsRegex = new Regex(macroParsPattern, RegexOptions.IgnoreCase);
            testxml();
        }

        public void testxml()
        {
            
            //ParseXML pxml = new ParseXML();
            //pxml.PartProgramFilePath = "D:\\_DEV\\VS_DEV\\zz_RES\\ISO\\ATR_77_001_Fiera_NP.xml";
            //pxml.ReadFile();
            //pxml.ProgramContext.Moves = pxml.GetMoves();

        }

        public IProgramContext GetProgramContext()
        {
            var programContext = new ProgramContext
            {
                ReferenceMove = new LinearMove
                {
                    EndPoint = new Point3D(0, 0, 0)// programContext.ReferenceMove != null ? new Point3D(programContext.ReferenceMove.EndPoint.X, programContext.ReferenceMove.EndPoint.Y, programContext.ReferenceMove.EndPoint.Z) : new Point3D(0, 0, 0);
                },

                LastEntity = new LinearMove
                {
                    EndPoint = new Point3D(0, 0, 0),
                },

                RotoTranslation = new RotoTranslation(),



            };

            programContext.Moves = GetMoves(programContext);

            return programContext;
        }

        private IList<IBaseEntity> GetMoves(ProgramContext programContext)
        {
            var lstMoves = ReadAndFilterLinesFromFile();
            programContext.TextLineCount = lstMoves.Count;  
            IList<IBaseEntity> moves = GetMoves(programContext, lstMoves);
            return moves;
        }

        private Dictionary<int, string> ReadAndFilterLinesFromFile()
        {
            var startT = Stopwatch.StartNew();

            Dictionary<string, string> dicVariables = new Dictionary<string, string>();
            Dictionary<int, string> dic = new Dictionary<int, string>();

            if (_dicSubprograms != null) _dicSubprograms = null;
            _dicSubprograms = new Dictionary<string, List<Tuple<int, string>>>();

            try
            {
                var dic_LineNumber_Line = ReadProgramFile();

                //If M30 index is different from the end of the list Size, then probrably there will be subprograms
                if (_indexOfM30 != dic_LineNumber_Line.Count)
                {
                    var lst = dic_LineNumber_Line.Values.ToList();

                    ReadSubprograms(lst, _indexOfM30);
                }

                var insertInLinesDictionary = false;
                foreach (var line in dic_LineNumber_Line)
                {
                    var lineCleaned = line.Value;
                    var lineNumber = line.Key;
                    insertInLinesDictionary = true;

                    #region SubProgram

                    lineCleaned = HandleSubCallLine(lineCleaned);

                    #endregion SubProgram

                    #region Handle Variables declaration

                    insertInLinesDictionary = GetVariablesDeclarationLine(dicVariables, insertInLinesDictionary, lineCleaned);

                    #endregion Handle Variables declaration

                    #region Handle line that use variables

                    lineCleaned = ReplaceVariablesInLine(dicVariables, lineCleaned);

                    #endregion Handle line that use variables

                    if (insertInLinesDictionary)
                        dic.Add(lineNumber, lineCleaned);

                    if (lineCleaned.StartsWith("M30")) break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex.Message);
            }
            finally
            {
                startT.Stop();
                Console.WriteLine($"{PartProgramFilePath} ReadAndFilterLinesFromFile reading of the file: {startT.Elapsed}");
            }

            return dic;
        }

        private string HandleSubCallLine(string lineCleaned)
        {
            if (lineCleaned.StartsWith("Q"))
            {
                //Be Sure to take just the Q command and not comments or other stuff
                var subCallCommand = SubCallRegex.Match(lineCleaned).Value;
                lineCleaned = subCallCommand;
            }

            return lineCleaned;
        }

        private string ReplaceVariablesInLine(Dictionary<string, string> dicVariables, string lineCleaned)
        {
            if (dicVariables.Count > 0)
            {
                var varUsedInLine = Regex.Matches(lineCleaned, variableRegexPattern);

                foreach (var item in varUsedInLine)
                {
                    if (dicVariables.ContainsKey(item.ToString()))
                    {
                        lineCleaned = lineCleaned.Replace(item.ToString(), dicVariables[item.ToString()]);
                    }
                }
            }

            return lineCleaned;
        }

        private bool GetVariablesDeclarationLine(Dictionary<string, string> dicVariables, bool insertInLineList, string lineCleaned)
        {
            if ((lineCleaned.StartsWith("P") || lineCleaned.StartsWith("LV")) && lineCleaned.Contains("=") && lineCleaned.Contains("$S25") == false)
            {
                MatchCollection m = VariableDeclarationRegex.Matches(lineCleaned);

                foreach (var item in m)
                {
                    var varDeclaration = item.ToString().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    double.TryParse(varDeclaration[1].Trim(), out double val);
                    if (dicVariables.ContainsKey(varDeclaration[0].Trim()) == false && varDeclaration != null)
                    {
                        dicVariables.Add(varDeclaration[0].Trim(), val.ToString());
                    }
                }

                return false;
            }
            else if (lineCleaned.Contains("$S25"))
            {
                return false;
            }

            return true;
        }

        public Dictionary<int, string> ReadProgramFile()
        {
            // Crea un nuovo dizionario per contenere le linee del file
            Dictionary<int, string> lines = new Dictionary<int, string>();
            Stopwatch dt = Stopwatch.StartNew();
            // Usa uno stream reader per leggere il file linea per linea
            using (StreamReader reader = new StreamReader(PartProgramFilePath))
            {
                // Inizializza un contatore per numerare le linee
                int lineNumber = 1;
                int indexOfCleanedLines = 0;
                string regexPattern = @"^(G08|G09|\/\/|\(\*|;|<ma|G40|G41|G42|EI|F1|F2|GO\*|\$\(CLEANTIP|\$\(OPTIMIZER)";
                // Continua a leggere linee finché non si raggiunge la fine del file
                while (!reader.EndOfStream)
                {
                    // Leggi la linea corrente dal file
                    string line = reader.ReadLine().ToUpper().Trim();
                    if (string.IsNullOrWhiteSpace(line)) { lineNumber++; continue; }
                    // Verifica se la linea inizia con "G08", "G09" o "//". In caso contrario,
                    // aggiungila al dizionario usando il numero della linea come chiave.
                    if (!Regex.IsMatch(line, regexPattern))
                    {
                        indexOfCleanedLines++;
                        lines.Add(lineNumber, line);

                        if (line == "M30")
                        {
                            _indexOfM30 = indexOfCleanedLines;
                            _programEndAtLine = lineNumber;
                        }
                    }

                    // Incrementa il contatore per la linea successiva
                    lineNumber++;
                }
            }

            dt.Stop();
            Console.WriteLine($"{PartProgramFilePath} First reading of the file: {dt.ElapsedMilliseconds} ms");
            // Restituisce il dizionario contenente le linee del file
            return lines;
        }

        private IList<IBaseEntity> GetMoves(ProgramContext programContext, Dictionary<int, string> lstMoves)
        {
            Stopwatch dt = Stopwatch.StartNew();

            List<IBaseEntity> moves = new List<IBaseEntity>();

            try
            {
                foreach (var lineReaded in lstMoves)
                {
                    programContext.SourceLine = lineReaded.Key;
                    var line = lineReaded.Value;

                    if (line.StartsWith("Q"))
                    {
                        var n = SubCallRegex.Match(line).Value;
                        var labelToFind = n.Replace("Q", "N");
                        if (_dicSubprograms.ContainsKey(labelToFind))
                        {
                            var subprogramMoves = _dicSubprograms[labelToFind];

                            foreach (var subLine in subprogramMoves)
                            {
                                programContext.SourceLine = subLine.Item1;
                                var subLineString = subLine.Item2;

                                IBaseEntity subProgramEntity = null;
                                ParseLine(ref programContext, moves, subLineString, ref subProgramEntity);
                            }
                            continue;
                        }
                    }

                    IBaseEntity entity = null;
                    ParseLine(ref programContext, moves, line, ref entity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error... " + ex.Message);
                
            }
            finally
            {
                dt.Stop();
                Console.WriteLine($"{PartProgramFilePath} GetMoves: {dt.ElapsedMilliseconds} ms");
            }

            return moves;
        }

        private void ParseLine(ref ProgramContext programContext, List<IBaseEntity> moves, string lineString, ref IBaseEntity baseEntity)
        {
            //Stopwatch dt = Stopwatch.StartNew();
            

            if (lineString.StartsWith("$"))
            {
                ParseMacro(lineString, ref programContext, ref baseEntity);
            }
            else if (lineString.StartsWith("G") && !lineString.ToUpper().Contains("O"))
            {
                ParseGLine(lineString, ref programContext, ref baseEntity);
            }

            if (baseEntity != null)
            {
                baseEntity.Is2DProgram = programContext.Is2DProgram;

                if (baseEntity is IMacro macro)
                    programContext.LastHeadPosition = macro.Movements.LastOrDefault(x => x.IsLeadIn).EndPoint;

                else
                    programContext.LastHeadPosition = (baseEntity as ToolpathEntity).EndPoint;

                programContext.LastEntity = baseEntity as IToolpathEntity;

                programContext.UpdateProgramCenterPoint();
                moves.Add(baseEntity);
            }

            //dt.Stop();
            //Console.WriteLine($"ParseLine() - {dt.ElapsedTicks} - {lineString}");
        }

        private void ReadSubprograms(IList<string> lst, int lineNumber)
        {
            try
            {
                //Non ci sono sottoprogrammi
                //if (lineNumber+3 > lst.Count) return;

                var lstSubInstructions = new List<Tuple<int, string>>();
                var readingSub = false;

                for (int i = lineNumber++; i < lst.Count; i++)
                {
                    var line = lst[i];
                    //if (line == "" || line.StartsWith("//") || line.StartsWith("(*")) continue;
                    if (line == "" || line.StartsWith("$(FILM_") || line.StartsWith("$(ICON_") || line.StartsWith("$(MICROJOINT"))
                        continue;

                    //if (line.StartsWith("M30"))
                    //{
                    //    readingSub = false;
                    //    //break;
                    //}

                    if (line[0] == 'N')
                    {
                        var n = SubCallRegex.Match(line).Value;
                        lstSubInstructions = new List<Tuple<int, string>>();
                        _dicSubprograms.Add(n, lstSubInstructions);
                        readingSub = true;
                        continue;
                    }

                    if (line.StartsWith("M01") || line.StartsWith("M02"))
                    {
                        readingSub = false;
                    }

                    if (readingSub)
                    {
                        lstSubInstructions.Add(new Tuple<int, string>(i, line));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ParseGLine(string line, ref ProgramContext programContext, ref IBaseEntity entity)
        {

            var code = line.Substring(0, 3);
            if (code == "G90" || code == "G91" || code == "G70" || code == "G71")
            {

                if (code == "G70" || code == "G71")
                {
                    _conversionValue = code == "G70" ? 25.4 : 1;
                    programContext.IsInchProgram = _conversionValue == 25.4;
                }

                if (code == "G90" || code == "G91")
                {
                    programContext.IsIncremental = code == "G91";
                }
                return;
            }



            //if (line.StartsWith("G01") || line.StartsWith("G00") || line.StartsWith("G02") || line.StartsWith("G03") || line.StartsWith("G103") || line.StartsWith("G102") || line.StartsWith("G104"))
            //{
            var m = GcodeAxesQuotaRegex.Matches(line);

            if (m[0].Value.Length > 1)
            {
                var gCodeNumber = int.Parse(m[0].Value.Substring(1));

                switch (gCodeNumber)
                {
                    case 0:
                    case 1:
                        //if (m.Count == 1) return;
                        entity = new LinearMove { OriginalLine = line };
                        entity = CalculateStartAndEndPoint(programContext, entity, m);
                        break;

                    case 2:
                        // clockwise turn
                        // g02 Xend yEnd zEnd I J K center relative to startpoint
                        entity = new ArcMove { OriginalLine = line };
                        entity = CalculateG02G03(programContext, entity, m, true);
                        break;

                    case 3:
                        // cointer-clockwise turn
                        // g03 Xend yEnd zEnd I J K center relative to startpoint
                        entity = new ArcMove { OriginalLine = line };
                        entity = CalculateG02G03(programContext, entity, m, false);
                        break;

                    case 92:
                        break;
                    case 93:
                        //sets new rototraslation, overreides previous one, if empty resets all rototraslations
                        
                        //(Vector3D translationComponents, Vector3D rotationComponents) = ParseRotoTranslComponents(m);

                        //programContext.RotoTranslation.UpdateRotoTraslComponents(rotationComponents,translationComponents);
                        
                        break;

                    case 94:
                        //set additive rototraslation, if empy resets only additive rototraslations
                        //(Vector3D _translationComponents, Vector3D _rotationComponents) = ParseRotoTranslComponents(m);
                        

                        //programContext.RotoTranslation.UpdateRotoTraslComponents(_rotationComponents, _translationComponents,false);
                        break;
                    case 113:

                        var refMove = programContext.ReferenceMove != null ? programContext.ReferenceMove : new LinearMove();

                        //It's like to not have axes in the instruction such as G93
                        //if (m.Count == 1)
                        //{
                        //    refMove.EndPoint = new Point3D(0, 0, 0);
                        //}
                        //else
                        //{
                        //    BuildMove(ref refMove, new Point3D(0, 0, 0), m, programContext);
                        //}
                        //programContext.ReferenceMove = refMove;

                        break;



                    case 100:
                        break;

                    case 102:
                    case 103:
                        break;

                    case 104:
                        entity = new ArcMove { OriginalLine = line };
                        entity = CalculateStartAndEndPoint(programContext, entity, m);
                        break;

                    default:
                        break;
                }
            }
            //}
        }


        private Tuple<Vector3D, Vector3D> ParseRotoTranslComponents(MatchCollection regexMatches)
        {
            Dictionary<char, double?> axesDictionary = GetValuesFromLine(regexMatches);
            //translation
            Vector3D translationFromDict = new Vector3D(0, 0, 0);
            if (axesDictionary.ContainsKey('X')) translationFromDict.X = axesDictionary['X'] ?? 0;
            if (axesDictionary.ContainsKey('Y')) translationFromDict.Y = axesDictionary['Y'] ?? 0;
            if (axesDictionary.ContainsKey('Z')) translationFromDict.Z = axesDictionary['Z'] ?? 0;
            //rotation
            Vector3D rotationFromDict = new Vector3D(0, 0, 0);
            if (axesDictionary.ContainsKey('A')) rotationFromDict.X = axesDictionary['A'] ?? 0;
            if (axesDictionary.ContainsKey('B')) rotationFromDict.Y = axesDictionary['B'] ?? 0;
            if (axesDictionary.ContainsKey('C')) rotationFromDict.Z = axesDictionary['C'] ?? 0;
            return new Tuple<Vector3D, Vector3D>(translationFromDict, rotationFromDict);

        }

        private IBaseEntity CalculateG02G03(ProgramContext programContext, IBaseEntity entity, MatchCollection regexMatches, bool isClockwise)
        {
            entity.SourceLine = programContext.SourceLine;
            entity.IsBeamOn = programContext.IsBeamOn;
            entity.LineColor = programContext.ContourLineType;
            ArcMove e = (entity as ArcMove);
            e.StartPoint = Create3DPoint(programContext, programContext.LastEntity.EndPoint);
            BuildMove(ref entity, regexMatches, programContext);
            e.EndPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, (entity as IToolpathEntity).EndPoint, programContext.IsIncremental ? programContext.LastEntity.EndPoint : new Point3D(0, 0, 0));

            GeoHelper.Add2DMoveProperties(ref e, isClockwise);
            return e;
        }

        private IBaseEntity CalculateStartAndEndPoint(ProgramContext programContext, IBaseEntity entity, MatchCollection regexMatches)
        {
            entity.SourceLine = programContext.SourceLine;
            entity.IsBeamOn = programContext.IsBeamOn;
            entity.LineColor = programContext.ContourLineType;
            IToolpathEntity e = (entity as IToolpathEntity);
            e.StartPoint = programContext.LastHeadPosition;//Create3DPoint(programContext, programContext.LastHeadPosition);
            BuildMove(ref entity, regexMatches, programContext);
            e.EndPoint = Create3DPoint(programContext, e.EndPoint, programContext.IsIncremental ? programContext.LastHeadPosition : new Point3D(0, 0, 0));
            if (!programContext.IsIncremental)
                e.EndPoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, e.EndPoint);

            if (entity is ArcMove)
            {
                var arcMove = (entity as ArcMove);
                arcMove.ViaPoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, arcMove.ViaPoint);
                GeoHelper.AddCircularMoveProperties(ref arcMove);

                //If radius is greater then 99000 for sure it is a linear movement.
                if (arcMove.Radius > 99000)
                {
                    entity = new LinearMove { StartPoint = arcMove.StartPoint, EndPoint = arcMove.EndPoint, OriginalLine = arcMove.OriginalLine, SourceLine = arcMove.SourceLine, IsBeamOn = arcMove.IsBeamOn, LineColor = arcMove.LineColor };
                }
            }

            return entity;
        }

        private Point3D Create3DPoint(ProgramContext programContext, Point3D p1, Point3D p2 = default, Point3D p3 = default)
        {
            if (p3 != null)
            {
                return new Point3D((p1.X + p2.X + p3.X), (p1.Y + p2.Y + p3.Y), (p1.Z + p2.Z + p3.Z));
            }
            else if (p2 != null)
            {
                return new Point3D((p1.X + p2.X), (p1.Y + p2.Y), (p1.Z + p2.Z));
            }
            else if (p1 != null)
            {
                return new Point3D((p1.X), (p1.Y), p1.Z);
            }

            return new Point3D(0, 0, 0);
        }

        private double Converter(double value)
        {
            return value * _conversionValue;
        }

        private double Converter(string value)
        {
            if (value.IndexOfAny(mathSymbols, 1) != -1)
            {
                var eval = StringToFormula.Eval(value);
                return Converter(eval);
            }
            return Converter(double.Parse(value));
        }

        private void ParseMacro(string line, ref ProgramContext programContext, ref IBaseEntity entity)
        {
            if (line.StartsWith("$(WORK_ON)") || line.StartsWith("$(MARKING)") || line.StartsWith("$(BEAM_ON)"))
            {
                programContext.IsBeamOn = true;

                if (programContext.IsMarkingProgram == false)
                {
                    if (line.StartsWith("$(WORK_ON)"))
                    {
                        var lineType = int.Parse(line.Replace("$(WORK_ON)", string.Empty).Replace("(", "").Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        programContext.ContourLineType = (ELineType)lineType;
                    }
                    else if (line.StartsWith("$(MARKING)"))
                    {
                        programContext.ContourLineType = ELineType.Marking;
                    }
                }
                else
                {
                    programContext.ContourLineType = ELineType.Marking;
                }
            }
            else if (line.StartsWith("$(WORK_OFF)") || line.StartsWith("$(END_MARKING)") || line.StartsWith("$(BEAM_OFF)"))
            {
                programContext.IsBeamOn = false;
                programContext.ContourLineType = ELineType.Rapid;
            }
            else if (line.StartsWith("$(MARKING_PIECE)"))
            {
                programContext.IsMarkingProgram = true;
            }
            else if (line.StartsWith("$(HOLE)"))
            {
                var macroParFounded = MacroParsRegex.Matches(line);

                var cX = Converter(macroParFounded[0].Value);
                var cY = Converter(macroParFounded[1].Value);
                var cZ = Converter(macroParFounded[2].Value);
                Point3D pC = RototranslationHelper.GetRotoTranslatedPoint(programContext, cX, cY, cZ);
                var nX = Converter(macroParFounded[3].Value);
                var nY = Converter(macroParFounded[4].Value);
                var nZ = Converter(macroParFounded[5].Value);
                Point3D pN = RototranslationHelper.GetRotoTranslatedPoint(programContext, nX, nY, nZ);

                var radius = Converter(macroParFounded[6].Value);


                entity = new HoleMoves
                {
                    SourceLine = programContext.SourceLine,
                    IsBeamOn = programContext.IsBeamOn,
                    LineColor = programContext.ContourLineType,
                    OriginalLine = line,
                    Radius = radius,
                    Normal = pN,
                    Center = pC,
                    Movements = new List<ToolpathEntity>()
                    {
                        new LinearMove
                        {
                            SourceLine = programContext.SourceLine,
                            OriginalLine = line,
                            StartPoint = programContext.LastHeadPosition,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            IsLeadIn = true
                           
                        },
                        new ArcMove
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                            IsStroked = true,
                            IsLargeArc = false,
                            Radius = Math.Abs(radius),//Aggiunto math.abs perchè... Se si vedessero comportamenti strani, verificare che la direzione della normale segua la regola della mano sinistra rispetto al verso di percorrenza dell'arco.
                            CenterPoint = pC,
                            NormalPoint = pN,
                        }

                    }
                };


                var holeMacro = entity as HoleMoves;
                GeoHelper.GetMoveFromMacroHole(ref holeMacro);
            }
            else if (line.StartsWith("$(SLOT)"))
            {
                var macroParFounded = MacroParsRegex.Matches(line);

                var c1X = Converter(macroParFounded[0].Value);
                var c1Y = Converter(macroParFounded[1].Value);
                var c1Z = Converter(macroParFounded[2].Value);
                Point3D pC1 = RototranslationHelper.GetRotoTranslatedPoint(programContext, c1X, c1Y, c1Z);

                var c2X = Converter(macroParFounded[3].Value);
                var c2Y = Converter(macroParFounded[4].Value);
                var c2Z = Converter(macroParFounded[5].Value);
                Point3D pC2 = RototranslationHelper.GetRotoTranslatedPoint(programContext, c2X, c2Y, c2Z);

                var nX = Converter(macroParFounded[6].Value);
                var nY = Converter(macroParFounded[7].Value);
                var nZ = Converter(macroParFounded[8].Value);
                Point3D pN = RototranslationHelper.GetRotoTranslatedPoint(programContext, nX, nY, nZ);

                var radius = Converter(macroParFounded[9].Value);

                entity = new SlotMove()
                {
                    SourceLine = programContext.SourceLine,
                    IsBeamOn = programContext.IsBeamOn,
                    LineColor = programContext.ContourLineType,
                    Center1= pC1,
                    Center2= pC2,
                    OriginalLine = line,
                    Radius = radius,
                    Normal = pN,
                    Movements = new List<ToolpathEntity>()
                    {
                        new LinearMove()
                        {
                            SourceLine = programContext.SourceLine,
                            OriginalLine = line,
                            StartPoint = programContext.LastHeadPosition,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            IsLeadIn = true

                        }, 
                        new LinearMove()
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                        },
                        new ArcMove
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                            IsStroked = true,
                            IsLargeArc = true,
                            Radius = Math.Abs(radius),
                            CenterPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pC1),
                            NormalPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pN),
                        },                        
                        new LinearMove()
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                        },
                        new ArcMove
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                            IsStroked = true,
                            IsLargeArc = true,
                            Radius = Math.Abs(radius),
                            CenterPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pC2),
                            NormalPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pN),
                        },


                    }
                };

                var slotMove = entity as SlotMove;
                GeoHelper.GetMovesFromMacroSlot(ref slotMove);
            }


            else if (line.StartsWith("$(KEYHOLE)"))
            {

                var macroParFounded = MacroParsRegex.Matches(line);

                var c1X = Converter(macroParFounded[0].Value);
                var c1Y = Converter(macroParFounded[1].Value);
                var c1Z = Converter(macroParFounded[2].Value);
                Point3D pC1 = RototranslationHelper.GetRotoTranslatedPoint(programContext, c1X, c1Y, c1Z);

                var c2X = Converter(macroParFounded[3].Value);
                var c2Y = Converter(macroParFounded[4].Value);
                var c2Z = Converter(macroParFounded[5].Value);
                Point3D pC2 = RototranslationHelper.GetRotoTranslatedPoint(programContext, c2X, c2Y, c2Z);

                var nX = Converter(macroParFounded[6].Value);
                var nY = Converter(macroParFounded[7].Value);
                var nZ = Converter(macroParFounded[8].Value);
                Point3D pN = RototranslationHelper.GetRotoTranslatedPoint(programContext, nX, nY, nZ);

                var radius1 = Converter(macroParFounded[9].Value);
                var radius2 = Converter(macroParFounded[10].Value);


                entity = new KeyholeMoves()
                {
                    Center1 = pC1,
                    Center2 = pC2,
                    BigRadius = radius1,
                    SmallRadius = radius2,
                    Normal = pN,
                    SourceLine = programContext.SourceLine,
                    IsBeamOn = programContext.IsBeamOn,
                    LineColor = programContext.ContourLineType,
                    OriginalLine = line,
                    Movements = new List<ToolpathEntity> {
                        new LinearMove()
                        {
                            SourceLine = programContext.SourceLine,
                            OriginalLine = line,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            StartPoint = programContext.LastHeadPosition,
                            IsLeadIn = true

                        },
                        new ArcMove
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                            IsStroked = true,
                            IsLargeArc = true,
                            Radius = Math.Abs(radius1),
                            CenterPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pC1),
                            NormalPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pN),
                        },
                        new ArcMove
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                            IsStroked = true,
                            IsLargeArc = true,
                            Radius = Math.Abs(radius2),
                            CenterPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pC2),
                            NormalPoint = Create3DPoint(programContext, programContext.ReferenceMove.EndPoint, pN),
                        },

                        new LinearMove()
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                        },
                        new LinearMove()
                        {
                            SourceLine = programContext.SourceLine,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            OriginalLine = line,
                        },

                    }

                };


                var keyholeMove = entity as KeyholeMoves;
                GeoHelper.GetMovesFromMacroKeyhole(ref keyholeMove);

            }


            else if (line.StartsWith("$(POLY)"))
            {
                var macroParFounded = MacroParsRegex.Matches(line);

                var c1X = Converter(macroParFounded[0].Value);
                var c1Y = Converter(macroParFounded[1].Value);
                var c1Z = Converter(macroParFounded[2].Value);
                Point3D centerPoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, c1X, c1Y, c1Z);

                var v1X = Converter(macroParFounded[3].Value);
                var v1Y = Converter(macroParFounded[4].Value);
                var v1Z = Converter(macroParFounded[5].Value);
                Point3D vertexPoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, v1X, v1Y, v1Z);
                var nX = Converter(macroParFounded[6].Value);
                var nY = Converter(macroParFounded[7].Value);
                var nZ = Converter(macroParFounded[8].Value);
                Point3D normalPoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, nX, nY, nZ);

                int.TryParse(macroParFounded[9].Value, out int sides);

                var radius = (macroParFounded.Count < 11) ? 0.0 : Converter(macroParFounded[10].Value);
                entity = new PolyMoves()
                {
                    SourceLine = programContext.SourceLine,
                    IsBeamOn = programContext.IsBeamOn,
                    LineColor = programContext.ContourLineType,
                    OriginalLine = line,
                    Sides = sides,

                    VertexPoint = vertexPoint,
                    NormalPoint = normalPoint,
                    CenterPoint = centerPoint,
                    Movements = new List<ToolpathEntity>()
                    {
                        new LinearMove()
                        {
                        SourceLine = programContext.SourceLine,
                        OriginalLine = line,
                        StartPoint = programContext.LastHeadPosition,
                        IsLeadIn = true,
                        IsBeamOn = programContext.IsBeamOn,
                        LineColor = programContext.ContourLineType,
                        }
                    }

                };

                var polyMove = entity as PolyMoves;
                GeoHelper.GetMovesFromMacroPoly(ref polyMove);
            }
            else if (line.StartsWith("$(RECT)"))
            {
                var macroParFounded = MacroParsRegex.Matches(line);

                var c1X = Converter(macroParFounded[0].Value);
                var c1Y = Converter(macroParFounded[1].Value);
                var c1Z = Converter(macroParFounded[2].Value);
                Point3D centerPoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, c1X, c1Y, c1Z);

                var v1X = Converter(macroParFounded[3].Value);
                var v1Y = Converter(macroParFounded[4].Value);
                var v1Z = Converter(macroParFounded[5].Value);
                Point3D vertexPoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, v1X, v1Y, v1Z);

                var sX = Converter(macroParFounded[6].Value);
                var sY = Converter(macroParFounded[7].Value);
                var sZ = Converter(macroParFounded[8].Value);
                Point3D sidePoint = RototranslationHelper.GetRotoTranslatedPoint(programContext, sX, sY, sZ);



                entity = new RectMoves()
                {
                    SourceLine = programContext.SourceLine,
                    IsBeamOn = programContext.IsBeamOn,
                    LineColor = programContext.ContourLineType,
                    OriginalLine = line,
                    SidePoint = sidePoint,
                    VertexPoint = vertexPoint,
                    CenterPoint = centerPoint,
                    Movements = new List<ToolpathEntity>()
                    {
                        new LinearMove()
                        {
                            SourceLine = programContext.SourceLine,
                            OriginalLine = line,
                            StartPoint = programContext.LastHeadPosition,
                            IsBeamOn = programContext.IsBeamOn,
                            LineColor = programContext.ContourLineType,
                            IsLeadIn = true
                        }
                    }

                };


                var rectMove = entity as RectMoves;
                GeoHelper.GetMovesFromMacroRect(ref rectMove);
            }
            else if (line.StartsWith("$(WORK_TYPE)"))
            {
                var macroParFounded = MacroParsRegex.Match(line).Value;
                programContext.Is2DProgram = macroParFounded == "1";
                programContext.Is3DProgram = macroParFounded == "2";
                programContext.IsTubeProgram = macroParFounded == "3";
                programContext.IsWeldProgram = macroParFounded == "4";
            }
            else if (line.StartsWith("$(APPROACH_MC)") || line.StartsWith("$(RETRACT_MC)"))
            {
                var macroParFounded = MacroParsRegex.Matches(line);

                var X = Converter(macroParFounded[0].Value);
                var Y = Converter(macroParFounded[1].Value);
                var Z = Converter(macroParFounded[2].Value);

                Point3D target = RototranslationHelper.GetRotoTranslatedPoint(programContext, X, Y, Z);

                entity = new LinearMove()
                {
                    StartPoint = programContext.LastHeadPosition,
                    EndPoint = target,
                    SourceLine = programContext.SourceLine,
                    IsBeamOn = false,
                    LineColor = ELineType.Rapid,
                    OriginalLine = line
                };
            }
            else
            {
                //var macroName = line.Substring(2, line.IndexOf(")") - 2).Trim();
                //if (macroNotConverted.Add(macroName))
                //{
                //}
                //Console.WriteLine(macroName);
            }
        }

        private string CleanLine(string lineToClean)
        {
            if (lineToClean.Contains("//") || lineToClean.Contains("(*"))
            {
                int splitcomment = lineToClean.IndexOf("//") != -1 ? lineToClean.IndexOf("//") : lineToClean.IndexOf("(*") != -1 ? lineToClean.IndexOf("(*") : 0;
                lineToClean = lineToClean.Substring(0, splitcomment);
            }
            if (lineToClean.Contains("#")) lineToClean = lineToClean.Substring(0, lineToClean.IndexOf("#"));
            if (lineToClean.Contains("  ")) lineToClean = lineToClean.Replace("  ", " ");
            //if (lineToClean.Contains(") (")) lineToClean = lineToClean.Replace(") (", ")(");
            if (lineToClean.Contains(" =") || lineToClean.Contains("= ")) lineToClean = lineToClean.Replace(" =", "=").Replace("= ", "=");
            if (lineToClean.Contains("LF=")) lineToClean = lineToClean.Substring(0, lineToClean.IndexOf("LF"));
            if (lineToClean.Contains("F=")) lineToClean = lineToClean.Substring(0, lineToClean.IndexOf("F="));

            return lineToClean.Trim();
        }
        /// <summary>
        /// Takes the regex findings and returns a sorted dict of the axes values
        /// </summary>
        private Dictionary<char, double?> GetValuesFromLine(MatchCollection matches)
        {
            Dictionary<char, double?> output = new Dictionary<char, double?>()
            {
                {'X', null},
                {'Y', null},
                {'Z', null},
                {'A', null},
                {'B', null},
                {'C', null},
                {'I', null},
                {'J', null},
                {'K', null},

            };

            for (int i = 1; i < matches.Count; i++)
            {
                var ax = matches[i].ToString();
                char axName = ax[0];
                if (output.ContainsKey(axName))
                {
                    var axValue = ax.Substring(1);

                    if (axValue.Contains("="))
                        axValue = axValue.Replace("=", "");

                    if (char.IsLetter(axValue[0]) == true) axValue = "0";

                    var axValueD = GetQuotaValue(axValue);
                    output[axName] = axValueD;
                }
                else continue;
            }
            var keys = output.Keys.ToList();
            foreach (var key in keys)
            {
                if (!output[key].HasValue) output.Remove(key);
            }
            return output;
        }

        private void BuildMove(ref IBaseEntity entity, MatchCollection matches, ProgramContext programContext)
        {
            var endPoint = new Point3D(0, 0, 0);
            var viaPoint = new Point3D(0, 0, 0);
            var axisArray = new char[] { 'X', 'Y', 'Z', 'I', 'J', 'K' };
            var n = matches.Count;
            for (int i = 1; i < n; i++)
            {
                var ax = matches[i].ToString();
                char axName = ax[0];
                if (Array.IndexOf(axisArray, axName) == -1) continue;
                //if (axName != 'X' && axName != 'Y' && axName != 'Z' && axName != 'I' && axName != 'J' && axName != 'K') continue;
                var axValue = ax.Substring(1);
                if (axValue.Contains("="))
                    axValue = axValue.Replace("=", "");

                if (char.IsLetter(axValue[0]) == true) axValue = "0";

                var axValueD = GetQuotaValue(axValue, programContext);

                if (axName == 'X') endPoint.X = axValueD;
                else if (axName == 'Y') endPoint.Y = axValueD;
                else if (axName == 'Z') endPoint.Z = programContext.Is2DProgram ? 0.0 : axValueD;
                else if (axName == 'I') viaPoint.X = axValueD;
                else if (axName == 'J') viaPoint.Y = axValueD;
                else if (axName == 'K') viaPoint.Z = programContext.Is2DProgram ? 0.0 : axValueD;

            }

            (entity as IToolpathEntity).EndPoint = endPoint;
            (entity as IToolpathEntity).OriginalEndPoint = new Point3D(endPoint.X,endPoint.Y,endPoint.Z);


            if (entity.EntityType == EEntityType.Arc || entity.EntityType == EEntityType.Circle)
            {
                (entity as ArcMove).ViaPoint = viaPoint;
                (entity as ArcMove).OriginalViaPoint = new Point3D(viaPoint.X, viaPoint.Y, viaPoint.Z);
            }
        }

        private double GetQuotaValue(string axValue, ProgramContext programContext)
        {
            if (double.TryParse(axValue, out var quota))
            {
                return Converter(quota);
            }
            else
            {
                //if (axValue.Contains("#"))
                //{
                //    var searchingForLabel = int.Parse(Regex.Match(axValue, @"\d+\.+\d+").Value);
                //}

                bool result = axValue.Any(x => !char.IsLetter(x));

                if (result)
                {
                    string formula = axValue;
                    var eval = StringToFormula.Eval(formula);
                    return Converter(eval);
                }
            }

            if (axValue == "MVX" || axValue == "MVY" || axValue == "MVZ")
            {
                return 0.0;
            }

            if (axValue.Contains("P") || axValue.Contains("LV"))
            {
            }
            return double.Parse(axValue);
        }
        private double GetQuotaValue(string axValue)
        {
            //overload of GetQuotaValue senza Progcontext..era usato ?
            if (double.TryParse(axValue, out var quota))
            {
                return Converter(quota);
            }
            else
            {
                //if (axValue.Contains("#"))
                //{
                //    var searchingForLabel = int.Parse(Regex.Match(axValue, @"\d+\.+\d+").Value);
                //}

                bool result = axValue.Any(x => !char.IsLetter(x));

                if (result)
                {
                    string formula = axValue;
                    var eval = StringToFormula.Eval(formula);
                    return Converter(eval);
                }
            }

            if (axValue == "MVX" || axValue == "MVY" || axValue == "MVZ")
            {
                return 0.0;
            }

            if (axValue.Contains("P") || axValue.Contains("LV"))
            {
            }
            return double.Parse(axValue);
        }

        private SortedDictionary<int, string> ReadAndFilterLinesFromFile(bool trueq)
        {
            Stopwatch dt = Stopwatch.StartNew();
            try
            {
                var dic_LineNumber_Line = ReadProgramFile();

                _dicSubprograms = new Dictionary<string, List<Tuple<int, string>>>();

                Dictionary<int, string> dic = new Dictionary<int, string>();
                Dictionary<string, string> dicVariables = new Dictionary<string, string>();
                //Dictionary<int, List<string>> dicOperationsInLabel = new Dictionary<int, List<string>>();
                //List<string> lstLabelOperations = new List<string>();
                var variableRegex = @"[P]\d+|LV\d+";

                //If M30 index is different from the end of the list Size, then probrably there will be subprograms
                if (_indexOfM30 != dic_LineNumber_Line.Count)
                {
                    var lst = dic_LineNumber_Line.Values.ToList();

                    ReadSubprograms(lst, _indexOfM30);
                }

                Parallel.ForEach(dic_LineNumber_Line.Keys, (key, state) =>
                {
                    if (key > _programEndAtLine) return;

                    try
                    {
                        var lineToClean = dic_LineNumber_Line[key];
                        var lineNumber = key;

                        var lineCleaned = CleanLine(lineToClean);

                        if (lineCleaned.StartsWith("M30"))
                        {
                            lock (dic)
                            {
                                dic.Add(lineNumber, lineCleaned);
                            }
                            state.Break();
                        }

                        #region SubProgram

                        if (lineCleaned.StartsWith("Q"))
                        {
                            //Be Sure to take just the Q command and not comments or other stuff
                            var q = SubCallRegex.Match(lineCleaned).Value;
                            lock (dic)
                            {
                                dic.Add(lineNumber, q);
                            }
                            return;
                        }

                        #endregion SubProgram

                        #region Handle GO

                        if (lineCleaned.StartsWith("GO"))
                        {
                            //    //Is not reading label but if it was reset to it
                            //    readingLabel = false;
                            //    //Need to search the label xx in the GOxx
                            //    jumpingIntoLabel = true;

                            //    lineCleaned = lineCleaned.Replace(" ", "");

                            //    //Take just the integer part
                            //    searchingForLabel = int.Parse(Regex.Match(lineCleaned, @"\d+").Value);

                            //    //If the label operations are already present in the dictionary then print it. Better to not enter here
                            //    //Because I have to set that the file reading index is at the end of this foreach
                            //    if (dicOperationsInLabel.ContainsKey(searchingForLabel))
                            //    {
                            //        var lst = dicOperationsInLabel[searchingForLabel];
                            //        foreach (var item in lst)
                            //        {
                            //            lock (dic)
                            //            {
                            //                dic.Add(lineNumber, lineCleaned);
                            //            }

                            //        }
                            //    }

                            //    //Get the next line
                            //    return;
                            //}

                            //if (lineCleaned.StartsWith("N") && lineCleaned.Contains("P200=") == false)
                            //{
                            //    readingLabel = true;
                            //    lineCleaned = lineCleaned.Replace(" ", "");

                            //    var labelFounded = int.Parse(Regex.Match(lineCleaned, @"\d+").Value);

                            //    //If is searching for a label from GOXX and this is the label searched then
                            //    //Stop searching.
                            //    if (searchingForLabel == int.Parse(Regex.Match(lineCleaned, @"\d+").Value))
                            //    {
                            //        jumpingIntoLabel = false;
                            //    }

                            //    if (lstLabelOperations.Count > 0)
                            //        lstLabelOperations = new List<string>();

                            //    if (dicOperationsInLabel.ContainsKey(labelFounded) == false)
                            //    {
                            //        lock (dicOperationsInLabel)
                            //        {
                            //            dicOperationsInLabel.Add(labelFounded, lstLabelOperations);
                            //        }
                            //    }

                            //    return;
                        }

                        //if (jumpingIntoLabel)
                        //{
                        //    return;
                        //}

                        //if (readingLabel)
                        //{
                        //    lock (lstLabelOperations)
                        //    {
                        //        lstLabelOperations.Add(lineCleaned);
                        //    }
                        //}

                        #endregion Handle GO

                        #region Handle Variables

                        if ((lineCleaned.StartsWith("P") || lineCleaned.StartsWith("LV")) && lineCleaned.Contains("=") && lineCleaned.Contains("$S25") == false)
                        {
                            lineCleaned = lineCleaned.Replace(" =", "=").Replace("= ", "=").Replace(" = ", "=");
                            MatchCollection m = VariableDeclarationRegex.Matches(lineCleaned);

                            foreach (var item in m)
                            {
                                var varDeclaration = item.ToString().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                                double.TryParse(varDeclaration[1].Trim(), out double val);
                                if (dicVariables.ContainsKey(varDeclaration[0].Trim()) == false && varDeclaration != null)
                                {
                                    lock (dicVariables)
                                    {
                                        dicVariables.Add(varDeclaration[0].Trim(), val.ToString());
                                    }
                                }
                            }

                            return;
                        }
                        else if ((lineCleaned.StartsWith("P") || lineCleaned.StartsWith("LV")) && lineCleaned.Contains("$S25"))
                        {
                            return;
                        }

                        #endregion Handle Variables

                        #region Handle line that use variables

                        var varFounded1 = (Regex.Matches(lineCleaned, variableRegex));

                        foreach (var item in varFounded1)
                        {
                            if (dicVariables.ContainsKey(item.ToString()))
                            {
                                lineCleaned = lineCleaned.Replace(item.ToString(), dicVariables[item.ToString()]);
                            }
                        }

                        #endregion Handle line that use variables

                        lock (dic)
                        {
                            if (lineCleaned == "M30") return;
                            dic.Add(lineNumber, lineCleaned);
                        }

                        //}
                        //else { Console.WriteLine($"{lineToClean}"); }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });

                return new SortedDictionary<int, string>(dic);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex.Message);
            }
            finally
            {
                dt.Stop();

                Console.WriteLine($"{PartProgramFilePath} ReadAndFilterLinesFromFile reading of the file: {dt.ElapsedMilliseconds} ms");
            }

            return null;
        }

        public Dictionary<int, string> ReadProgramFile(string a)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            //var fileTextContent = new string[40000];
            Stopwatch dt = Stopwatch.StartNew();
            int counterSkippedAtTheBeginning = 0;
            const Int32 BufferSize = 1024;
            using (var fileStream = File.OpenRead(PartProgramFilePath))
            using (var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8, true, BufferSize))
            {
                string line;
                int lineNumberFromReadedFile = 0;
                int indexOfCleanedLines = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line == string.Empty || line.StartsWith("//", true, null) || line.StartsWith(";", true, null) || line.StartsWith("(*", true, null) || line == "G08" || line == "G09")
                    {
                        counterSkippedAtTheBeginning++;
                        lineNumberFromReadedFile++;
                        continue;
                    }

                    line = line.TrimStart().ToUpper();

                    if (
                        //line.StartsWith("//",true,System.Globalization.CultureInfo.CurrentCulture) ||
                        //line.StartsWith("(*", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        //line.StartsWith(";", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        //line.StartsWith("G09", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        //line.StartsWith("G08", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("F=", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("G40", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("G41", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("G42", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("EI", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("GO*", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("<MA", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("F1", true, System.Globalization.CultureInfo.CurrentCulture) ||
                        line.StartsWith("F2", true, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        lineNumberFromReadedFile++;
                        continue;
                    }

                    if (line == "M30")
                    {
                        indexOfCleanedLines++;
                        _indexOfM30 = indexOfCleanedLines;
                        _programEndAtLine = lineNumberFromReadedFile;
                        dic[lineNumberFromReadedFile] = line;
                    }
                    else
                    {
                        indexOfCleanedLines++;
                        dic[lineNumberFromReadedFile] = line;
                    }
                    lineNumberFromReadedFile++;
                    // Process line
                }
            }
            dt.Stop();

            Console.WriteLine($"{PartProgramFilePath} First reading of the file: {dt.ElapsedMilliseconds} ms");
            return dic;
        }

        public void ReadFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}