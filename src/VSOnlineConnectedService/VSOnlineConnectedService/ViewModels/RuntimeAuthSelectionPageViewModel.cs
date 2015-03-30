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
    public class RuntimeAuthSelectionPageViewModel : ConnectedServiceWizardPage
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

        private RuntimeAuthOptions _runtimeAuthOptions;

        public RuntimeAuthOptions RuntimeAuthOptions
        {
            get { return _runtimeAuthOptions; }
            set
            {
                if (value != _runtimeAuthOptions)
                {
                    _runtimeAuthOptions = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}
