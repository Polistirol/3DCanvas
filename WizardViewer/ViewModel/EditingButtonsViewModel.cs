using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WizardViewer.ViewModel
{
    public class EditingButtonsViewModel : INotifyPropertyChanged
    {
        private bool _isUnlocked;

        public bool IsUnlocked
        {
            get { return _isUnlocked; }
            set { _isUnlocked = value;
                OnPropertyChanged(nameof(IsUnlocked));

            }
        }
        private string _lockingName;
        public string LockingName
        {
            get { return _lockingName; }
            set { _lockingName = value;
            OnPropertyChanged(nameof(LockingName));}
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
