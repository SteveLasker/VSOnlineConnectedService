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
    [Export(typeof(ConnectedServiceProvider))]
    [ExportMetadata("ProviderId", "TFSWorkStoreItemWizard.Provider")]
    internal class WorkItemStoreWizardProvider : ConnectedServiceProvider
    {
        public WorkItemStoreWizardProvider()
        {
            this.Category = "Microsoft";
            this.Name = "Team Foundation Server Wizard - Work Item Tracking";
            this.Description = "Provides connection to Microsoft Team Foundation Server's WorkItemStore service used for tracking work items in a Team Project.";
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + Assembly.GetAssembly(this.GetType()).ToString() + ";component/Resources/TFSImage.png"));
            this.CreatedBy = "Microsoft";
            this.Version = new Version(1, 0, 0);
            this.MoreInfoUri = new Uri("https://msdn.microsoft.com/en-us/library/bb130347.aspx");
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderHost host)
        {
            ConnectedServiceConfigurator configurator = new WorkItemStoreWizard();
            return Task.FromResult(configurator);
        }
    }
}