using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using VSOnlineConnectedService.Views;
using EnvDTE;
using VSOnlineConnectedService.Utilities;

namespace VSOnlineConnectedService.ViewModels
{
    public class OptionsViewModel : ConnectedServiceWizardPage
    {
        public OptionsViewModel()
        {
            this.Legend = "Options";
            this.Title = "VSOnline: Options";
            this.Description = "Additional Configuration Options";
            this.IsEnabled = true;

            this.View = new OptionsView();
            this.View.DataContext = this;
        }
        private string _serviceName;

        public string ServiceName
        {
            get { return _serviceName; }
            set
            {
                if (value != _serviceName)
                {
                    _serviceName = GetValidServiceName(value);
                    this.OnPropertyChanged();
                }
            }
        }
        private string GetValidServiceName(string name)
        {
            //TODO: Do real validation for what folders/namespaces are already in the project
            //if (name == "VSOnline")
            //{
            //    name = name + "1";
            //}
            return name;
        }

    }
}
