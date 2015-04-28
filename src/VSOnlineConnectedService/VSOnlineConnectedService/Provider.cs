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
            Category = "Microsoft";
            Name = "Visual Studio Online - Work Item Tracking Sample";
            Description = "Manage Work Items in Visual Studio Online."; this.CreatedBy = "Steve Lasker";
            Version = new Version(1, 0, 0, 7);
            MoreInfoUri = new Uri("https://github.com/SteveLasker/VSOnlineConnectedServiceProvider");
            Icon = new BitmapImage(new Uri("pack://application:,,/" + Assembly.GetExecutingAssembly().ToString() + ";component/" + "Resources/Icon.png"));
        }

        public override IEnumerable<Tuple<string, Uri>> GetSupportedTechnologyLinks()
        {
            // A list of supported technolgoies, such as which services it supports
            yield return Tuple.Create("Visual Studio Online", new Uri("https://www.visualstudio.com/products/what-is-visual-studio-online-vs"));
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