using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TFSProvider;

namespace TFSConnectedService
{
    [ExportMetadata("ProviderId", "TFSWorkStoreItemWizard.Provider")]
    [Export(typeof(ConnectedServiceHandler))]
    [ExportMetadata("AppliesTo", "CSharp")]
    internal class WizardHandler : Handler
    {

    }

    //Temporarily commented out for demo purposes of only showing the wizard
    //[ExportMetadata("ProviderId", "TFSWorkStoreItemSinglePage.Provider")]
    //[Export(typeof(ConnectedServiceHandler))]
    [ExportMetadata("AppliesTo", "CSharp")]
    internal class SinglePageHandler : Handler
    {
        //Note: The single page handler isn't fully implemented; only did partial implemtnation to compare this
        //experience against the wizard handler
    }
  
    internal class Handler : ConnectedServiceHandler
    {

        public override async Task AddServiceInstanceAsync(ConnectedServiceInstanceContext context, CancellationToken ct)
        {
            //TODO: need to revisit my singleton approach and how TFSEConnectedServiceInstances are passed around.  

            // return Task.FromResult(true);

            this.ConfigStuff(context);

            this.AddAssemblyReferences(context);

            await this.GenerateScaffolding(context);

            // TODO: Why does this take the context?  Seems like this allows me to do some extra stuff besides just open the uri?
            await HandlerHelper.AddGettingStartedAsync(context, context.ServiceInstance.Name, new Uri("https://msdn.microsoft.com/en-us/library/bb130347.aspx"));
        }

        private void AddAssemblyReferences(ConnectedServiceInstanceContext context)
        {
            //Add required assemblies for connecting to TFS WorkItemStore.
            //NOTE: In Dev 14 Preview, the TFS assemblies aren't in the GAC, so we need to reference them by their full path.
            //This handler assumes that VS is installed under Program Files for either 32-bit or 64-bit machine; we could update this code to look in the registry, but this is simpler for now.
            string x86Dir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string rootDir = String.IsNullOrEmpty(x86Dir) ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) : x86Dir;
            rootDir += @"\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\";

            HandlerHelper.AddAssemblyReference(context, rootDir + "Microsoft.TeamFoundation.Client.dll");
            HandlerHelper.AddAssemblyReference(context, rootDir + "Microsoft.TeamFoundation.Common.dll");
            HandlerHelper.AddAssemblyReference(context, rootDir + "Microsoft.TeamFoundation.WorkItemTracking.Client.dll");
            HandlerHelper.AddAssemblyReference(context, rootDir + "Microsoft.VisualStudio.Services.Common.dll");

            //TODO: This code isn't correct - it's copying the assemblies to the runtime dir of this project instead of
            //of the runtime dir of the target project.
            //Need to copy the following assemblies to the local runtime folder; this is because these assemblies aren't in the GAC
            File.Copy(rootDir + "Microsoft.WITDataStore32.dll", Environment.CurrentDirectory + @"\Microsoft.WITDataStore32.dll", true);
            File.Copy(rootDir + "Microsoft.WITDataStore64.dll", Environment.CurrentDirectory + @"\Microsoft.WITDataStore64.dll", true);

            //Need to add this reference for reading values from the .config file
            HandlerHelper.AddAssemblyReference(context, "System.Configuration");
        }

        private void ConfigStuff(ConnectedServiceInstanceContext context)
        {
            TFSConnectedServiceInstance tfsContext = (TFSConnectedServiceInstance)context.ServiceInstance;

            using (EditableConfigHelper configHelper = new EditableConfigHelper(context.ProjectHierarchy))
            {
                configHelper.SetAppSetting("TFSUri", tfsContext.TFSUri);
                configHelper.SetAppSetting("TeamProjectCollectionName", tfsContext.TeamProjectCollectionName);
                configHelper.SetAppSetting("TeamProjectName", tfsContext.TeamProjectName);
                configHelper.Save();
            }
        }

        private async Task GenerateScaffolding(ConnectedServiceInstanceContext context)
        {
            TFSConnectedServiceInstance tfsContext = (TFSConnectedServiceInstance)context.ServiceInstance;

            string templateResourceUri = null;

            if (tfsContext.RuntimeAuthOption == RuntimeAuthOptions.BasicAuth)
            {
                templateResourceUri = "pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Templates/BasicAuthTemplate.cs";
            }
            else if (tfsContext.RuntimeAuthOption == RuntimeAuthOptions.IntegratedAuth)
            {
                templateResourceUri = "pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Templates/IntegratedAuthTemplate.cs";
            }
            else
            {
                throw new InvalidOperationException("Unsupported runtime authentication option:" + tfsContext.RuntimeAuthOption);
            }

            // Generate a code file into the user's project from a template. 
            // The tokens in the template will be replaced by the HandlerHelper. 
            // Place service specific scaffolded code under the service folder 
            string serviceFolder = string.Format("Service References\\{0}\\", tfsContext.Name);
            Dictionary<string, string> tokenReplacements = new Dictionary<string, string>();
            tokenReplacements.Add("ServiceInstance.TFSUri", "TFSUri");
            tokenReplacements.Add("ServiceInstance.TeamProjectCollectionName", "TeamProjectCollectionName");
            tokenReplacements.Add("ServiceInstance.TeamProjectName", "TeamProjectName");

            await HandlerHelper.AddFileAsync(context, templateResourceUri, serviceFolder + "TFSWorkItemStoreConnector.cs", tokenReplacements);
        }
    }
}
