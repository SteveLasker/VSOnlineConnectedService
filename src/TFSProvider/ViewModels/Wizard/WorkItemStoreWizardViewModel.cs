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
    internal class WorkItemStoreWizard : ConnectedServiceWizard
    {
        ProjectSelectionPage projectSelection = new ProjectSelectionPage();
        RuntimeAuthSelectionPage authSelection = new RuntimeAuthSelectionPage();

        public WorkItemStoreWizard()
        {
            this.Pages.Add(projectSelection);
            this.Pages.Add(authSelection);

            foreach (var page in this.Pages)
            {
                page.PropertyChanged += this.PageViewModel_PropertyChanged;
            }
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            //TeamProjectSelection projectView = (TeamProjectSelection)projectSelection.View;

            TFSConnectedServiceInstance.Current.Name = "TFSWorkItemStoreService";

            return Task.FromResult<ConnectedServiceInstance>(TFSConnectedServiceInstance.Current);
        }

        private void PageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ProjectSelectionPage)
            {
                ProjectSelectionPage projSelectPage = (ProjectSelectionPage)sender;
                if (e.PropertyName == "AllowContinue")
                {
                    this.OnEnableNavigation(new NavigationEnabledState(false, true, false));
                    this.authSelection.IsEnabled = true;
                }
            }
            else if (sender is RuntimeAuthSelectionPage)
            {
                RuntimeAuthSelectionPage authSelectPage = (RuntimeAuthSelectionPage)sender;
                if (e.PropertyName == "AllowContinue")
                {
                    this.OnEnableNavigation(new NavigationEnabledState(true, false, true));
                }
            }
        }
    }
}
