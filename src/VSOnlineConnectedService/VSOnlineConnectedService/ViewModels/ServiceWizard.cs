using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Microsoft.VisualStudio.ConnectedServices;

using Microsoft.TeamFoundation.Client;
using VSOnlineConnectedService.ViewModels;
namespace VSOnlineConnectedService.ViewModels
{
    internal class ServiceWizard : ConnectedServiceWizard
    {
        TeamProjectSelectionViewModel _projectSelection = new TeamProjectSelectionViewModel();
        RuntimeAuthSelectionPageViewModel _authSelection = new RuntimeAuthSelectionPageViewModel();
        OptionsViewModel _options = new OptionsViewModel();

        public ServiceWizard()
        {
            // Add the ViewModels that represent the pages of the wizard
            this.Pages.Add(_projectSelection);
            this.Pages.Add(_authSelection);
            this.Pages.Add(_options);
            

            foreach (var page in this.Pages)
            {
                page.PropertyChanged += this.PageViewModel_PropertyChangd;
            }
        }
        public ConnectedServiceProviderContext Context { get; set; }
        private void PageViewModel_PropertyChangd(object sender, PropertyChangedEventArgs e)
        {
            // Is the Server configured?
            //Ensure that a valid TFS server, project collection and project are selected before allowing the user to continue through the wizard
            if (!string.IsNullOrEmpty(this._projectSelection.VSOnlineURI) &&
               !string.IsNullOrEmpty(this._projectSelection.TeamProjectCollectionName) &&
               !string.IsNullOrEmpty(this._projectSelection.TeamProjectName))
            {
                this._authSelection.IsEnabled = true;
            }

            // Should the Finish button be enabled?
            if (this._projectSelection.IsEnabled &&
                this._authSelection.RuntimeAuthOptions != RuntimeAuthOptions.None)
            {
                this.IsFinishEnabled = true;
            }
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            Instance instance = new Instance();
            instance.VSOnlineUri = this._projectSelection.VSOnlineURI;
            instance.TeamProjectName = this._projectSelection.TeamProjectName;
            instance.TeamProjectCollectionName = this._projectSelection.TeamProjectCollectionName;
            instance.Name = this._options.ServiceName;
            instance.RuntimeAuthOption = this._authSelection.RuntimeAuthOptions;
            return Task.FromResult<ConnectedServiceInstance>(instance);
        }

        /// <summary>
        /// Cleanup object references
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // free up the property changed events
            foreach (var page in this.Pages)
            {
                page.PropertyChanged -= this.PageViewModel_PropertyChangd;
            }
            base.Dispose(disposing);
        }
    }
}