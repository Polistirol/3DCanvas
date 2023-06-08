using ParserLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PrimaPower.Stores
{
    public class SelectedViewerElementsStore
    {
		private Path _clickedPath;
		public Path ClickedPath
		{
			get
			{
				return _clickedPath;
			}
			set
			{
				_clickedPath = value;
                ClickedPathChanged?.Invoke();
               
			}
		}

		private IBaseEntity _clickedEntity	;
		public IBaseEntity ClickedEntity
		{
			get
			{
				return _clickedEntity;
			}
			set
			{
                _clickedEntity = value;
                ClickedEntityChanged?.Invoke();
            }
		}

		private int _clickedEntityLineInfo;
		public int ClickedEntityLineInfo
		{
			get
			{
				return _clickedEntityLineInfo;
			}
			set
			{
				_clickedEntityLineInfo = value;
				ClickedEntityLineInfoChanged?.Invoke(value);
			}
		}


		public void ViewerElementClicked(Path clickedPath, IBaseEntity clickedPathEntity, int clickedEntitySourceLine)
        {
			ClickedPath = clickedPath;
			ClickedEntity = clickedPathEntity;
            ClickedEntityLineInfo = clickedEntitySourceLine;
        }

        public event Action ClickedPathChanged;
		public event Action ClickedEntityChanged;
		public event Action<int> ClickedEntityLineInfoChanged;


    }
}
