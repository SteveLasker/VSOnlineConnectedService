using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VSOnlineConnectedService
{
    [ConnectedServiceHandlerExport(
        "Microsoft.Samples.VSOnline",
        AppliesTo = "CSharp")]
    public class Handler : ConnectedServiceHandler
    {
        private const string CONFIGKEY_TFSURI = "_TFSUri";
        private const string CONFIGKEY_TEAMPROJECTNAME = "_TeamProjectName";
        private const string CONFIGKEY_TEAMPROJECTCOLLECTIONNAME = "_TeamProjectCollectionName";
        private const string CONFIGKEY_USERNAME = "_UserName";
        private const string CONFIGKEY_PASSWORD = "_Password";
        public override async Task<AddServiceInstanceResult> AddServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            //TODO: need to revisit my singleton approach and how TFSEConnectedServiceInstances are passed around.  
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding References");
            this.AddAssemblyReferences(context);

            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Config values");
            this.UpdateConfig(context);

            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Scaffolding Code");
            await this.GenerateScaffolding(context);

            return new AddServiceInstanceResult(context.ServiceInstance.Name, new System.Uri("https://msdn.microsoft.com/en-us/library/bb130347.aspx"));
        }

        private void AddAssemblyReferences(ConnectedServiceHandlerContext context)
        {
            //Add required assemblies for connecting to TFS WorkItemStore.
            //NOTE: In Dev 14 Preview, the TFS assemblies aren't in the GAC, so we need to reference them by their full path.
            //This handler assumes that VS is installed under Program Files for either 32-bit or 64-bit machine; we could update this code to look in the registry, but this is simpler for now.
            string x86Dir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string rootDir = String.IsNullOrEmpty(x86Dir) ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) : x86Dir;
            rootDir += @"\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\";

            context.HandlerHelper.AddAssemblyReference(rootDir + "Microsoft.TeamFoundation.Client.dll");
            context.HandlerHelper.AddAssemblyReference(rootDir + "Microsoft.TeamFoundation.Common.dll");
            context.HandlerHelper.AddAssemblyReference(rootDir + "Microsoft.TeamFoundation.WorkItemTracking.Client.dll");
            context.HandlerHelper.AddAssemblyReference(rootDir + "Microsoft.VisualStudio.Services.Common.dll");

            //TODO: This code isn't correct - it's copying the assemblies to the runtime dir of this project instead of
            //of the runtime dir of the target project.
            //Need to copy the following assemblies to the local runtime folder; this is because these assemblies aren't in the GAC

            // TODO: Files should be CopyToOutput = True
            // hoping there's an updated NuGet soon to avoid having to do this, or eliminate these native dlls alltogether
            context.HandlerHelper.AddFileAsync(rootDir + "Microsoft.WITDataStore32.dll", "Microsoft.WITDataStore32.dll").Wait();
            context.HandlerHelper.AddFileAsync(rootDir + "Microsoft.WITDataStore64.dll", "Microsoft.WITDataStore64.dll").Wait();

            //Need to add this reference for reading values from the .config file
            context.HandlerHelper.AddAssemblyReference("System.Configuration");
        }

        private void UpdateConfig(ConnectedServiceHandlerContext context)
        {
            Instance tfsContext = (Instance)context.ServiceInstance;

            using (EditableXmlConfigHelper configHelper = context.CreateEditableXmlConfigHelper())
            {
                configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_TFSURI, tfsContext.VSOnlineUri, comment:"VSOnline");
                configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_TEAMPROJECTNAME, tfsContext.TeamProjectName);
                configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_TEAMPROJECTCOLLECTIONNAME, tfsContext.TeamProjectCollectionName);
                if (tfsContext.RuntimeAuthOption == RuntimeAuthOptions.BasicAuth)
                {
                    configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_USERNAME, "RequiredValue");
                    configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_PASSWORD, "RequiredValue");
                }
                configHelper.Save();
            }
        }

        private async Task GenerateScaffolding(ConnectedServiceHandlerContext context)
        {
            Instance tfsContext = (Instance)context.ServiceInstance;

            //Instance vsOnlineInstance = (Instance)context.ServiceInstance;
            string templateResourceUri = null;
            switch (tfsContext.RuntimeAuthOption)
            {
                case RuntimeAuthOptions.IntegratedAuth:
                    templateResourceUri = "pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Templates/IntegratedAuthTemplate.cs";
                    break;
                case RuntimeAuthOptions.BasicAuth:
                    templateResourceUri = "pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Templates/BasicAuthTemplate.cs";
                    break;
                case RuntimeAuthOptions.UsernamePasswordServiceAuth:
                    templateResourceUri = "pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Templates/ServiceAuthTemplate.cs";
                    break;
                default:
                    throw new InvalidOperationException("Unsupported runtime authentication option:" + tfsContext.RuntimeAuthOption);
            }

            // Generate a code file into the user's project from a template. 
            // The tokens in the template will be replaced by the HandlerHelper. 
            // Place service specific scaffolded code under the service folder 
            string serviceFolder = string.Format("Service References\\{0}\\", tfsContext.Name);
            context.HandlerHelper.TokenReplacementValues.Add("Instance.VSOnlineUri", tfsContext.Name + CONFIGKEY_TFSURI);
            context.HandlerHelper.TokenReplacementValues.Add("Instance.TeamProjectName", tfsContext.Name + CONFIGKEY_TEAMPROJECTNAME);
            context.HandlerHelper.TokenReplacementValues.Add("Instance.TeamProjectCollectionName", tfsContext.Name + CONFIGKEY_TEAMPROJECTCOLLECTIONNAME);

            await context.HandlerHelper.AddFileAsync(templateResourceUri, 
                Path.Combine(context.HandlerHelper.GetServiceArtifactsRootFolder(), 
                             context.ServiceInstance.Name, 
                             "TFSWorkItemStoreConnector.cs"));
        }
    }
}