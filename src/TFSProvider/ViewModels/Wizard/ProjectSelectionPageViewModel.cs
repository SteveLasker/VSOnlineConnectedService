using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Reflection;

using TFSProvider;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.TeamFoundation.Client;
using System.Diagnostics;
using System.Windows.Forms;

namespace TFSConnectedService
{
    public class ProjectSelectionPage : ConnectedServiceWizardPage
    {
        TeamProjectSelection tpsUserCtrl;

        public override Task<NavigationEnabledState> OnPageEnteringAsync(WizardEnteringArgs args)
        {
            NavigationEnabledState state;
            if (args.PreviousPage != null)
            {
                //If the Previous button was clicked, then leave the state of the wizard's buttons unchanged
                state = new NavigationEnabledState(null, null, null);
            }
            else
            {
                //The first time the page is loaded, make sure that the Next and Finished buttons are disabled; we'll enable the Next button when TFS is successfully connected to
                state = new NavigationEnabledState(false, false, false);
            }
            return Task.FromResult(state);
        }

        public ProjectSelectionPage()
        {
            //Why not have a Title property that can be applied to all pages within the wizard?  Right now you have to implement them for each individual page.

            this.Title = "Configure Team Foundation Server for consumption within .NET ";
            this.Legend = "TFS Connection Options";
            this.Description = "Select the Team Foundation Server, Team Project Collection, and Team Project.";

            this.tpsUserCtrl = new TeamProjectSelection(this);
            this.View = this.tpsUserCtrl;
        }

        public void GetTFSConfiguration()
        {
            //To keep things simple, only allow a single team project to be selected using the TFS picker control
            using (TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false))
            {
                var result = tpp.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //Store the TFS picker control's selections to be used later in scaffolding
                    TfsConfigurationServer tfs = tpp.SelectedTeamProjectCollection.ConfigurationServer;
                    TFSConnectedServiceInstance.Current.TFSUri = tfs.Uri.ToString();
                    TFSConnectedServiceInstance.Current.TeamProjectCollectionName = tpp.SelectedTeamProjectCollection.Name.Replace(tfs.Name + "\\", String.Empty);

                    //The picker control is configured to only allow one project to be selected
                    Debug.Assert(tpp.SelectedProjects.Length == 1, "User should only be able to select one team project.");
                    TFSConnectedServiceInstance.Current.TeamProjectName = tpp.SelectedProjects[0].Name;

                    //Set view's labels to values selected in the TFS picker control
                    this.tpsUserCtrl.TfsServerUriVal.Content = TFSConnectedServiceInstance.Current.TFSUri;
                    this.tpsUserCtrl.TeamProjecCollectionVal.Content = TFSConnectedServiceInstance.Current.TeamProjectCollectionName;
                    this.tpsUserCtrl.TeamProjectVal.Content = TFSConnectedServiceInstance.Current.TeamProjectName;
                }
            }

            //Ensure that a valid TFS server, project collection and project are selected before allowing the user to continue through the wizard
            if ((this.tpsUserCtrl.TfsServerUriVal.Content != null) &&
               (this.tpsUserCtrl.TeamProjecCollectionVal.Content != null) &&
               (this.tpsUserCtrl.TeamProjectVal.Content != null))
            {
                this.OnNotifyPropertyChanged("AllowContinue");
            }
        }
    }
}