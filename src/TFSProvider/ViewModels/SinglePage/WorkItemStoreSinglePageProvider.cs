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
    //[Export(typeof(ConnectedServiceProvider))]
    //[ExportMetadata("ProviderId", "TFSWorkStoreItemSinglePage.Provider")]
    public class WorkItemStoreSinglePageProvider : ConnectedServiceProvider
    {
        public WorkItemStoreSinglePageProvider()
        {
            this.Category = "Microsoft";
            this.Name = "Team Foundation Server Single Page - Work Item Tracking";
            this.Description = "Provides connection to Microsoft Team Foundation Server's WorkItemStore service used for tracking work items in a Team Project.";
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + Assembly.GetAssembly(this.GetType()).ToString() + ";component/Resources/TFSImage.png"));
            this.CreatedBy = "Microsoft";
            this.Version = new Version(1, 0, 0);
            this.MoreInfoUri = new Uri("https://msdn.microsoft.com/en-us/library/bb130347.aspx");
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderHost host)
        {
            ConnectedServiceConfigurator configurator = new WorkItemConnectionPage();
            return Task.FromResult(configurator);
        }
    }

    public class TFSAuthentication : ConnectedServiceAuthenticator
    {
        TFSAuth view;
        bool isTFSConfigured;

        public string TfsServerUri { get; set;  }

        public string TeamProjectCollection { get; set; }

        public string TeamProject { get; set; }


        public TFSAuthentication()
        {
            this.NeedToAuthenticateText = "Authenticate to TFS";

            this.view = new TFSAuth(this);
            this.View = this.view;
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
                    this.TfsServerUri = TFSConnectedServiceInstance.Current.TFSUri;
                    this.TeamProjectCollection = TFSConnectedServiceInstance.Current.TeamProjectCollectionName;
                    this.TeamProject = TFSConnectedServiceInstance.Current.TeamProjectName;

                    //Ensure that a valid TFS server, project collection and project are selected before allowing the user to continue through the wizard
                    //this.isTFSConfigured = (this.view.TfsServerUriVal.Content != null) &&
                    //                       (this.view.TeamProjecCollectionVal.Content != null) &&
                    //                       (this.view.TeamProjectVal.Content != null);

                    this.OnNotifyPropertyChanged(nameof(TfsServerUri));
                    this.OnNotifyPropertyChanged(nameof(TeamProjectCollection));
                    this.OnNotifyPropertyChanged(nameof(TeamProject));

                }
            }
        }

    }

    public class WorkItemConnectionPage : ConnectedServiceSinglePage
    {
        TeamProjectAuthSelection view;
        bool isAuthSelected;
        public ConnectedServiceAuthenticator Authenticator { get; set; }

        public WorkItemConnectionPage()
        {
            this.Description = "Connect to TFS's Work Item Store and generate scaffolding.";

            this.view = new TeamProjectAuthSelection(this);
            this.View = view;

            this.IsFinishEnabled = true;
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            TFSConnectedServiceInstance.Current.Name = "TFSWorkItemStoreService";
            return Task.FromResult<ConnectedServiceInstance>(TFSConnectedServiceInstance.Current);
        }

        public override Task<ConnectedServiceAuthenticator> CreateAuthenticatorAsync()
        {
            this.Authenticator = new TFSAuthentication();
            return Task.FromResult(this.Authenticator);
        }

        public void GetAuthSelection(RuntimeAuthOptions selectedAuthOption)
        {
            TFSConnectedServiceInstance.Current.RuntimeAuthOption = selectedAuthOption;

            if (selectedAuthOption != RuntimeAuthOptions.None)
            {
                this.isAuthSelected = true;
            }
        }

    }
}