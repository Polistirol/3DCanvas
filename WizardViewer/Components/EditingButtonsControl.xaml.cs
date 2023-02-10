using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WizardViewer.ViewModel;

namespace WizardViewer.Components
{
    /// <summary>
    /// Logica di interazione per EditingButtonsControl.xaml
    /// </summary>
    public partial class EditingButtonsControl : UserControl
    {
        public EditingButtonsViewModel vM { get; set; }
        public EditingButtonsControl()
        {
            InitializeComponent();
            vM= new EditingButtonsViewModel();
            this.DataContext= vM;
            vM.LockingName = "Unlock Editing";
            vM.IsUnlocked= false;
        }

        public Action PreviewBtn_Clicked;
        public Action SaveBtn_Clicked;
        public Action RevertBtn_Clicked;
        public Action UnlockBtn_Clicked;

        private void UnlockBtn_Click(object sender, RoutedEventArgs e)
        {
            vM.IsUnlocked = !vM.IsUnlocked;
            if (vM.IsUnlocked)
                vM.LockingName = "Disable Editing";
            else
                vM.LockingName = "Unlock Editing";
            UnlockBtn_Clicked.Invoke();

        }

        public void ResetButtons()
        {
            vM.IsUnlocked = false;
            vM.LockingName = "Unlock Editing";
        }

        private void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            PreviewBtn_Clicked?.Invoke();
        }       
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveBtn_Clicked.Invoke();
        }

        private void RevertBtn_Click(object sender, RoutedEventArgs e)
        {
            RevertBtn_Clicked.Invoke();
        }



 
    }
}
