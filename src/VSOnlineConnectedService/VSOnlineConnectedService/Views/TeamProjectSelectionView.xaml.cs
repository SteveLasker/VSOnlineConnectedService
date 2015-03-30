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
using VSOnlineConnectedService.ViewModels;

namespace VSOnlineConnectedService.Views
{
    /// <summary>
    /// Interaction logic for TeamProjectSelectionView.xaml
    /// </summary>
    public partial class TeamProjectSelectionView : UserControl
    {
        public TeamProjectSelectionView()
        {
            InitializeComponent();
            //Used to invoke methods on the the view model (e.g. the provider) so that we can keep the View layer separate from the ViewModel.  
            //This data context is different than the Instance context because it's specific to marshalling data between the View\ViewModel.
            //this.DataContext = dataContext;

        }
        /// <summary>
        /// Displays the TFS picker control to allow the user to select the team foundation server, team project collection, and team project.
        /// The picker control automatically prompts the user for authentication as needed.  
        /// </summary>
        private void selectTeamProject_Click(object sender, RoutedEventArgs e)
        {
            ((TeamProjectSelectionViewModel)this.DataContext).GetTFSConfiguration();
        }

    }
}
