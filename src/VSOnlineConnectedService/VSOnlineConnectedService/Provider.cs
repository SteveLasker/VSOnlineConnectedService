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
            this.CreatedBy = "Microsoft";
            this.Version = new Version(1, 0, 0);
            this.MoreInfoUri = new Uri("https://msdn.microsoft.com/en-us/library/bb130347.aspx");
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + Assembly.GetExecutingAssembly().ToString() + ";component/" + "Resources/Icon.png"));
        }

        public override IEnumerable<Tuple<string, Uri>> GetSupportedTechnologyLinks()
        {
            // A list of supported technolgoies, such as which services it supports
            yield return Tuple.Create("SomeURLName", new Uri("https://www.visualstudio.com/products/what-is-visual-studio-online-vs"));
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderContext context)
        {
            return Task.FromResult<ConnectedServiceConfigurator>(new ServiceWizard());
        }
    }
}