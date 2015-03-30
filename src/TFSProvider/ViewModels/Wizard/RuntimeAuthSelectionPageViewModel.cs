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
    public class RuntimeAuthSelectionPage : ConnectedServiceWizardPage
    {
        public override Task<NavigationEnabledState> OnPageEnteringAsync(WizardEnteringArgs args)
        {
            //The first time the page is loaded, make sure that the Next and Finished buttons are disabled; we'll enable the Next button when TFS is successfully connected to
            NavigationEnabledState state = new NavigationEnabledState(true, false, false);
            return Task.FromResult(state);
        }

        public RuntimeAuthSelectionPage()
        {

            this.Title = "Configure Team Foundation Server for consumption within .NET ";
            this.Legend = "Runtime Authentication Scaffolding";
            this.Description = "Select the type of runtime authentication to use for connecting to Team Foundation Server.";
            this.IsEnabled = false;

            this.View = new RuntimeAuthSelection(this);
        }

        public void GetAuthSelection(RuntimeAuthOptions selectedAuthOption)
        {
            TFSConnectedServiceInstance.Current.RuntimeAuthOption = selectedAuthOption;

            if (selectedAuthOption != RuntimeAuthOptions.None)
            {
                this.OnNotifyPropertyChanged("AllowContinue");
            }

        }
    }
}
