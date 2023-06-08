using ParserLib;
using ParserLibrary.Entities.Parsers;
using ParserLibrary.Interfaces;
using ParserLibrary.Services.Parsers;
using PrimaPower.Stores;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PrimaPower.ViewModels
{
    public class Siemens3DViewerViewModel : BaseViewModel
    {

        public string FilePath { get; set; }

        private IBaseEntity _clickedEntity;
        private int _clickedEntityLineInfo;
        private Path _clickedPath { get; set; }
        

        private SelectedViewerElementsStore _selectedViewerElementsStore;

        public SelectedViewerElementsStore SelectedViewerElementsStore
        {
            get { return _selectedViewerElementsStore; }
        }



        private IProgramContext programContext;
        public IProgramContext ProgramContext
        {
            get { return programContext; }
            set { programContext = value;
                OnPropertyChanged(nameof(ProgramContext));
            }
        }

        private XElement _programXElement;

        public XElement ProgramXElement
        {
            get { return _programXElement; }
            set { _programXElement = value;
                OnPropertyChanged(nameof(ProgramXElement));
            }
        }

        private int[] _selectedLinesIDs;

        public int[] SelectedLinesIDs
        {
            get { return _selectedLinesIDs; }
            set { 
                _selectedLinesIDs = value;
                OnPropertyChanged(nameof(SelectedLinesIDs));    
            
            }
        }


        public Siemens3DViewerViewModel(SelectedViewerElementsStore selectedViewerElementsStore)
        {
            _selectedViewerElementsStore = selectedViewerElementsStore;

            
        }

        public void UpateSelectedProgram(string filepath, XElement loadedXElement)
        {
            FilePath = filepath;
            ProgramXElement = loadedXElement;
            //BuildProgramContext(loadedXElement);  //commented because unusued atm
        }

        /// <summary>
        /// Updates the ProgramContext from the selected program XElement
        /// </summary>
        public void BuildProgramContext(XElement loadedXElement)
        {
            var parser = GetParser(loadedXElement);
            ProgramContext = parser.GetProgramContext();
        }

        private Parser GetParser(XElement programXElement)
        {
            return new Parser(new ParseXML(programXElement));  
        }

        private Parser GetParser(string filePath)
        {
            Parser parser = null;
            var extension = System.IO.Path.GetExtension(filePath).ToLower().Trim();
            if (extension == ".iso")
                parser = new Parser(new ParseIso(filePath));
            else if (extension == ".mpf")
                parser = new Parser(new ParseMpf(filePath));
            else if (extension == ".xml")
                parser = new Parser(new ParseXML(filePath));
            else
                MessageBox.Show("File extension invalid");
            return parser;
        }

        public void OnClickedPath(Path clickedPath , IBaseEntity clickedPathEntity , int clickedEntitySourceLine )
        {
            _clickedPath = clickedPath;
            _clickedEntity = clickedPathEntity;
            _clickedEntityLineInfo = clickedEntitySourceLine;

            _selectedViewerElementsStore.ViewerElementClicked(_clickedPath, _clickedEntity, _clickedEntityLineInfo);
        }


    }


}




