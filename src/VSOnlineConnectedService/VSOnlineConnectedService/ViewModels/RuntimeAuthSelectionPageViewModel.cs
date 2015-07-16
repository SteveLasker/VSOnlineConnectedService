using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using VSOnlineConnectedService.Views;

namespace VSOnlineConnectedService.ViewModels
{
    internal class RuntimeAuthSelectionPageViewModel : ConnectedServiceWizardPage
    {
        public RuntimeAuthSelectionPageViewModel()
        {
            this.Legend = Properties.Resources.RunttimeAuthPageLegend;
            this.Title = Properties.Resources.RuntimeAuthPageTitle;
            this.Description = Properties.Resources.RuntimeAuthPageDescription;
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

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
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
