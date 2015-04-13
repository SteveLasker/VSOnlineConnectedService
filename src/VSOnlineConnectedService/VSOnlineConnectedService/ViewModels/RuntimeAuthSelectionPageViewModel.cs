using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.TeamFoundation.Client;
using VSOnlineConnectedService.Views;

namespace VSOnlineConnectedService.ViewModels
{
    internal class RuntimeAuthSelectionPageViewModel : ConnectedServiceWizardPage
    {
        public RuntimeAuthSelectionPageViewModel()
        {
            this.Legend = "Runtime Auth";
            this.Title = "VSOnline: Configure Runtime Authentication";
            this.Description = "How will you authenticate TFS at runtime?";
            this.IsEnabled = false;
            this.View = new RuntimeAuthSelectionView();
            this.View.DataContext = this;
        }

        private RuntimeAuthOptions _runtimeAuthOptions = RuntimeAuthOptions.UsernamePasswordServiceAuth;

        public RuntimeAuthOptions RuntimeAuthOptions
        {
            get { return _runtimeAuthOptions; }
            set
            {
                if (value != _runtimeAuthOptions)
                {
                    _runtimeAuthOptions = value;
                    // if the RuntimeAuthOption has been cleared from None, clear any error state
                    if (_runtimeAuthOptions != RuntimeAuthOptions.None)
                    {
                        this.HasErrors = false;
                    }
                    this.OnPropertyChanged();
                }
            }
        }

        public override Task<WizardNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            // If the user has entered and left this page, validate the RuntimeAuthOption has been set to a valid value
            if (this.RuntimeAuthOptions == RuntimeAuthOptions.None)
            {
                this.HasErrors = true;
            }
            return base.OnPageLeavingAsync(args);
        }
    }
}
