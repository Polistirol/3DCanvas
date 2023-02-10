using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WizardViewer.ViewModel
{
    public class Point3DCoordinatesViewModel : INotifyPropertyChanged
    {

        private string _pointName;

        public string PointName
        {
            get { return _pointName; }
            set { _pointName = value;
                OnPropertyChanged(nameof(PointName));
            }
        }

        private string _Xname;

        public string XName
        {
            get { return _Xname; }
            set { _Xname = value;
                OnPropertyChanged(nameof(XName));
            }
        } 
        private string _Yname;

        public string YName
        {
            get { return _Yname; }
            set {
                _Yname = value;
                OnPropertyChanged(nameof(YName));
            }
        }
        private string _Zname;

        public string ZName
        {
            get { return _Zname; }
            set {
                _Zname = value;
                OnPropertyChanged(nameof(ZName));
            }
        }
        private double? _x;

        public double? X
        {
            get { return _x; }
            set {
                _x = value;
                OnPropertyChanged(nameof(X));
            }
        }
        private double? _y;

        public double? Y
        {
            get { return _y; }
            set {
                _y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        private double? _z;

        public double? Z
        {
            get { return _z; }
            set
            {
                _z = value;
                OnPropertyChanged(nameof(Z));
            }
        }

        private bool _isEditable;

        public bool IsEditable
        {
            get { return _isEditable; }
            set {
                _isEditable = value;
                OnPropertyChanged(nameof(IsEditable));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
