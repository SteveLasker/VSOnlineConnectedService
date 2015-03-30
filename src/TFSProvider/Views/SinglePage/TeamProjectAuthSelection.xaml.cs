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
using TFSConnectedService;

namespace TFSProvider
{
    /// <summary>
    /// Interaction logic for TeamProjectAuthSelection.xaml
    /// </summary>
    public partial class TeamProjectAuthSelection : UserControl
    {
        public TeamProjectAuthSelection(WorkItemConnectionPage dataContext)
        {
            InitializeComponent();

            this.DataContext = dataContext;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button.IsChecked ?? true)
            {
                ((WorkItemConnectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.IntegratedAuth);
            }
            else
            {
                ((WorkItemConnectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.None);
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button.IsChecked ?? true)
            {
                ((WorkItemConnectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.BasicAuth);
            }
            else
            {
                ((WorkItemConnectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.None);
            }
        }

        ///// <summary>
        ///// Displays the TFS picker control to allow the user to select the team foundation server, team project collection, and team project.
        ///// The picker control automatically prompts the user for authentication as needed.  
        ///// </summary>
        //private void selectTeamProject_Click(object sender, RoutedEventArgs e)
        //{
        //    ((WorkItemConnectionPage)this.DataContext).GetTFSConfiguration();
        //}
    }
}
