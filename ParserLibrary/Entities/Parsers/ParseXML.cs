using ParserLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Entities.Parsers
{
    public class ParseXML : BaseParser,IParser
    {
        public string PartProgramFilePath { get; set; }

        private IEnumerable<XElement> _xInstructions;

        private IEnumerable<XElement> _xCommDefInstructions;

        private const string CommDefFilePath = "C:\\FPSuite\\SharedFiles\\CommDefs\\VisualCommDef.xml";


        private bool _hasEnded = false;

        private int maxNestingLevel = 5;

        
        public ParseXML(string partProgramFilePath):base() {
            PartProgramFilePath = partProgramFilePath;
            ReadFile();
            BeginParsing();
        }
        public ParseXML(XElement preloadedProgram) : base()
        {
            _xInstructions = preloadedProgram.Elements();
            BeginParsing();
        }
        public IProgramContext GetProgramContext()
        {
            return ProgramContext;
        }

        public void ReadFile()
        {
            ReadFile(PartProgramFilePath);
        }

        public void ReadFile(string partProgramFilePath)
        {
            //check existance
            bool fileExists = File.Exists(partProgramFilePath);
            if (fileExists == false) 
                throw new FileNotFoundException();

            _xInstructions = XElement.Load(partProgramFilePath, LoadOptions.SetLineInfo).Elements();
        }

        private void BeginParsing()
        {
            _xCommDefInstructions = LoadCommDefs();
            GetMoves();
        }

        /// <summary>
        /// Gets all the moves from a list of xelement objects to fill te program Context
        /// Parse each xElement from the PDF , gets what istruction type is and calls the methods to pares eeach  single line
        /// </summary>
        private void GetMoves()
        {
            GetMoves(_xInstructions);
        }
        /// <summary>
        /// Gets all the moves from a list of xelement objects to fill te program Context
        /// Parse each xElement from the PDF , gets what istruction type is and calls the methods to pares eeach  single line
        /// </summary>
        /// <param name="sublist"> a list of xElement to cycle thru </param>
        /// <param name="endLabel">if a element has a -label- attribute == endalbel , the cycling will stop </param>
        private void GetMoves(IEnumerable<XElement> list, bool isSub = false, string endLabel="" )
        {
            foreach (XElement element in list)
            {
                //getting instruction type and mode
                string instructionName = element.Name.LocalName.ToUpper();
                EInstructionMode? instructionMode;
                EInstructionType instructionType = GetInstructionTypeAndMode(instructionName, out instructionMode   );

                switch (instructionType)
                {
                    case EInstructionType.Movement:
                        ParseMovement(element, instructionMode);
                        break;

                    case EInstructionType.Macro:
                        ParseMacro(element, instructionMode);
                        break;

                    case EInstructionType.Process:
                        ParseProcess(element,instructionMode);
                        break;

                    case EInstructionType.Rototranslation:
                        ParseRototranslation(element,instructionMode);
                        break;                       
                    case EInstructionType.Variable:
                        ParseVariable(element);
                        break;                       
                    case EInstructionType.Modal:
                        ParseModal(element,instructionMode);
                        break;

                    case EInstructionType.SubProgram:
                        if (isSub == true && instructionMode == EInstructionMode.SubEnd)
                            return;
                        ParseSubProgram(element, instructionMode);
                        break;

                    case EInstructionType.Siemens:
                        continue;
                        
                    case EInstructionType.Skippable:
                        continue;
                }
                if (_hasEnded)
                    return;
            }
        }

        private IEnumerable<XElement> LoadCommDefs()
        {
            bool fileExists = File.Exists(CommDefFilePath);
            if (fileExists == false)
                throw new FileNotFoundException();
            XName block = "block";
            XName commands = "commands";
            return XElement.Load(CommDefFilePath).Elements(block).Elements(commands).Elements();
        }


        #region Parsers
        private void ParseMovement(XElement element, EInstructionMode? instructionMode)
        {
           ValuesMap valueMap =  GetValuesMapFromElement(element,EInstructionType.Movement) as ValuesMap;
           valueMap.InstructionMode = instructionMode;
           AddMovementToMoves(valueMap);
        }

        private void ParseProcess(XElement element,EInstructionMode? mode)
        {
            int cuttingLine;
            int piercingType;
            switch (mode)
            {
                case EInstructionMode.StartProcess:
                    {
                        piercingType = int.Parse(element.Attribute("PiercingType").Value.ToString());
                        //$ marking is a process start with NoPiercing ecc
                        cuttingLine = int.Parse(element.Attribute("CuttingLine").Value.ToString());
                        AddProcessStart(piercingType, cuttingLine);

                        break;
                    }
                case EInstructionMode.StopProcess:
                    {
                        cuttingLine = int.Parse(element.Attribute("NextCuttingLine").Value.ToString());
                        AddProcessStop(cuttingLine);
                        break;
                    }
            }
        }


        private void ParseMacro(XElement element, EInstructionMode? instructionMode)
        {
            MacroValueMap valuesMap = GetValuesMapFromElement(element,EInstructionType.Macro) as MacroValueMap;
            valuesMap.InstructionMode= instructionMode;
            AddMacroToMoves(valuesMap);
        }

        private void ParseRototranslation(XElement element, EInstructionMode? instructionMode)
        {
            ValuesMap valueMap = GetValuesMapFromElement(element, EInstructionType.Rototranslation) as ValuesMap;
            valueMap.InstructionMode = instructionMode;
            AddRototraslation(valueMap, instructionMode);
        }

        private void ParseModal(XElement element, EInstructionMode? instructionMode)
        {
            switch (instructionMode)
            {
                case EInstructionMode.EndProgram:
                    _hasEnded = true ; return;
                case EInstructionMode.AbsoluteProg:case EInstructionMode.RelativeProg:
                    SetProgrammingMode(instructionMode); break;      
            }
           
        }

        private void ParseVariable(XElement element)
        {
            
        }

        private void ParseSubProgram(XElement element, EInstructionMode? instructionMode)
        {
            string SubName = element.Attribute("SubName").Value;  
            
            switch (instructionMode)
            {
                case EInstructionMode.SubCall:
                    IEnumerable<XElement> subList =  GetElementsOfSub(SubName);
                    GetMoves(subList,true);
                    break;
            }
        }

        #endregion


        private IEnumerable<XElement> GetElementsOfSub(string SubName)
        {
            List<XElement> instructionList = _xInstructions.ToList();

            int startIndex = instructionList.FindIndex(element =>  ( isMode(element,EInstructionMode.SubStart) && element.Attribute("SubName").Value  == SubName ) )  ;

            if (startIndex == -1)
                throw new Exception("Subname was not found in program!");

            instructionList = instructionList.GetRange(startIndex + 1, instructionList.Count - startIndex - 1);
            return instructionList;
        }


        private int GetSourceLine(XElement element)
        {
            return (element as IXmlLineInfo).LineNumber;
        }

        private bool isMode(XElement element, EInstructionMode targetMode)
        {
            string name = element.Name.LocalName;
            EInstructionMode? instructionMode;
            EInstructionType instructionType = GetInstructionTypeAndMode(name, out instructionMode);
            if (instructionMode == targetMode)
                return true;
            else 
                return false;
        }

        private EInstructionType GetInstructionTypeAndMode(string name , out EInstructionMode? InstrMode )
        {
            XElement el = _xCommDefInstructions.FirstOrDefault(x => x.Name.LocalName == name);
            if (el != null)
                el = el.Element("Prima_OPEN_Laser").Element("Preview");
            //se el c'è e ha l'elemento "Preview"
            if (el != null )
            {       
                XAttribute type = el.Attribute("InstructionType");
                XAttribute mode = el.Attribute("InstructionMode");
                if (type == null || mode == null)
                    throw new InvalidOperationException("Command to Preview has not type or Mode !");

                else
                {
                    if ( Enum.TryParse<EInstructionType>(type.Value.ToString(), out EInstructionType instructionType) &&
                        Enum.TryParse<EInstructionMode>(mode.Value.ToString(), out EInstructionMode instructionMode) ) {
                        InstrMode = instructionMode;
                        return (instructionType);
                    }
                    else
                        throw new InvalidOperationException("Command to Preview has not type or Mode !");
                }

            }
            else
            {
                InstrMode = null;
                return EInstructionType.Skippable  ;
            }
                
            
        }



        /// <summary>
        /// Cycles thru the element attributes and populates the 
        /// </summary>
        /// <param name="element"></param>
        private IValuesMap GetValuesMapFromElement(XElement element, EInstructionType type)
        {
            IValuesMap valuesMap;
            if (type == EInstructionType.Movement || type == EInstructionType.Rototranslation)
            {
                valuesMap = new ValuesMap();
            }
            else 
            {
                valuesMap = new MacroValueMap();   
            }

            valuesMap.Name = element.Name.LocalName.ToUpper();
            valuesMap.SourceLine = GetSourceLine(element);
            valuesMap.OriginalLine = element.ToString();

            foreach (XAttribute attribute in element.Attributes())
            {
                string attributeName = attribute.Name.LocalName;
                string attributeValue = attribute.Value;
                valuesMap.SetAttribute(attributeName, attributeValue);
            }
            return valuesMap;
        }

    } 

}
