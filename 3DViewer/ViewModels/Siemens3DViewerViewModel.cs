using FP.FPSuite.UI.Controls.TulusForLaser3D.Stores;
using ParserLib;
using ParserLibrary.Entities.Parsers;
using ParserLibrary.Interfaces;
using ParserLibrary.Services.Parsers;
using System.Windows;
using System.Xml.Linq;

namespace PrimaPower.ViewModels
{
    public class Siemens3DViewerViewModel : BaseViewModel
    {
        public string FilePath { get; set; }

        private readonly SelectedFilesListItemStore _selectedFilesListItemStore;

        
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



        public Siemens3DViewerViewModel(SelectedFilesListItemStore selectedFilesListItemStore)
        {
            _selectedFilesListItemStore = selectedFilesListItemStore;
            _selectedFilesListItemStore.SelectedItemChanged += OnSelectedItemChanged;
        }

        private void OnSelectedItemChanged()
        {
            FilePath = _selectedFilesListItemStore.SelectedItem.Path;
            ProgramXElement = _selectedFilesListItemStore.LoadedXElement;
            BuildProgramContext();
        }

        /// <summary>
        /// Updates the ProgramContext from the selected program XElement
        /// </summary>
        public void BuildProgramContext()
        {
            var parser = GetParser(_selectedFilesListItemStore.LoadedXElement);
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
    }


}




