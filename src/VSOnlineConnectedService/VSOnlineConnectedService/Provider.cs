using VSOnlineConnectedService.ViewModels;
using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VSOnlineConnectedService
{
    [ConnectedServiceProviderExport("Microsoft.Samples.VSOnline")]
    public class Provider : ConnectedServiceProvider
    {
        public Provider()
        {
            this.Category = "Microsoft";
            this.Name = "VSOnline - Work Item Tracking";
            this.Description = "Provides connection to Visual Studio Online WorkItemStore service used for tracking work items in a Team Project.";
            this.Version = new Version(1, 0, 0);
            this.MoreInfoUri = new Uri("https://github.com/SteveLasker/VSOnlineConnectedServiceProvider");
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + Assembly.GetExecutingAssembly().ToString() + ";component/" + "Resources/Icon.png"));
        }

        public override IEnumerable<Tuple<string, Uri>> GetSupportedTechnologyLinks()
        {
            // A list of supported technolgoies, such as which services it supports
            yield return Tuple.Create("Visual Studio Online", new Uri("https://www.visualstudio.com/products/what-is-visual-studio-online-vs"));
            //https://msdn.microsoft.com/en-us/library/bb130347.aspx
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderContext context)
        {
            ServiceWizard wizard = new ServiceWizard();
            wizard.Context = context;
            wizard.Initialize();
            return Task.FromResult<ConnectedServiceConfigurator>(wizard);
        }
    }
}