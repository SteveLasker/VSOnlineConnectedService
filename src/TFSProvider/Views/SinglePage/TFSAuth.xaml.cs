using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

using Microsoft.TeamFoundation.Client;
using TFSConnectedService;

namespace TFSProvider
{
    /// <summary>
    /// Interaction logic for Test2.xaml
    /// </summary>
    public partial class TFSAuth : System.Windows.Controls.UserControl
    {
        public TFSAuth(TFSAuthentication dataContext)
        {
            InitializeComponent();

            //Used to invoke methods on the the view model (e.g. the provider) so that we can keep the View layer separate from the ViewModel.  
            //This data context is different than the TFSConnectedServiceInstance context because it's specific to marshalling data between the View\ViewModel.
            this.DataContext = dataContext;
        }

        /// <summary>
        /// Displays the TFS picker control to allow the user to select the team foundation server, team project collection, and team project.
        /// The picker control automatically prompts the user for authentication as needed.  
        /// </summary>
        private void selectTeamProject_Click(object sender, RoutedEventArgs e)
        {
            ((TFSAuthentication)this.DataContext).GetTFSConfiguration();
        }
    }
}
