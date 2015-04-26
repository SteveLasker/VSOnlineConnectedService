using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.Threading;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shell = Microsoft.VisualStudio.Shell;

using VSOnlineConnectedService.Utilities;
namespace VSOnlineConnectedService
{
    [ConnectedServiceHandlerExport(
        "Microsoft.Samples.VSOnline",
        AppliesTo = "CSharp")]
    public class Handler : ConnectedServiceHandler
    {
        private const string CONFIGKEY_TFSURI = ":Endpoint";
        private const string CONFIGKEY_TEAMPROJECTNAME = ":TeamProjectName";
        private const string CONFIGKEY_TEAMPROJECTCOLLECTIONNAME = ":TeamProjectCollectionName";
        private const string CONFIGKEY_USERNAME = ":UserName";
        private const string CONFIGKEY_PASSWORD = ":Password";

        [Import]
        internal IVsPackageInstaller PackageInstaller { get; set; }

        public override async Task<AddServiceInstanceResult> AddServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            Project project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);

            await this.AddAssemblyReferences(context);
            await this.AddNuGetPackagesAsync(context, project);
            await this.UpdateConfig(context);
            await this.GenerateScaffolding(context);

            return new AddServiceInstanceResult(context.ServiceInstance.Name, 
                new System.Uri(Properties.Resources.GettingStartedURL));
        }

        private async Task AddAssemblyReferences(ConnectedServiceHandlerContext context)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding References");
            //Need to add this reference for reading values from the .config file
            context.HandlerHelper.AddAssemblyReference("System.Configuration");
            context.HandlerHelper.AddAssemblyReference("System.ComponentModel.DataAnnotations");
        }

        private async Task AddNuGetPackagesAsync(ConnectedServiceHandlerContext context, Project project)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages");
            this.PackageInstaller.InstallPackagesFromVSExtensionRepository(
                "VSOnlineConnectedService.e30335d6-f9c7-4d08-b422-f011f3f18477",
                false,
                false,
                project,
                new Dictionary<string, string> {
                    { "Newtonsoft.Json", "6.0.8" }
                });
        }

        private async Task UpdateConfig(ConnectedServiceHandlerContext context)
        {
            
            await Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); // The EditableConfigHelper must run on the UI thread.
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Config values");
            Instance tfsContext = (Instance)context.ServiceInstance;

            using (EditableXmlConfigHelper configHelper = context.CreateEditableXmlConfigHelper())
            {
                configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_TFSURI, tfsContext.VSOnlineUri, comment:"VSOnline");
                configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_TEAMPROJECTNAME, tfsContext.TeamProjectName);
                configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_TEAMPROJECTCOLLECTIONNAME, tfsContext.TeamProjectCollectionName);
                if (tfsContext.RuntimeAuthOption == RuntimeAuthOptions.UsernamePasswordServiceAuth)
                {
                    configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_USERNAME, "RequiredValue");
                    configHelper.SetAppSetting(tfsContext.Name + CONFIGKEY_PASSWORD, "RequiredValue");
                }
                configHelper.Save();
            }
        }

        private async Task GenerateScaffolding(ConnectedServiceHandlerContext context)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Scaffolding Code");
            Instance tfsContext = (Instance)context.ServiceInstance;

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
                    context.HandlerHelper.TokenReplacementValues.Add("Instance:Username", tfsContext.Name +  CONFIGKEY_USERNAME);
                    context.HandlerHelper.TokenReplacementValues.Add("Instance:Password", tfsContext.Name + CONFIGKEY_PASSWORD);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported runtime authentication option:" + tfsContext.RuntimeAuthOption);
            }

            // Generate a code file into the user's project from a template. 
            // The tokens in the template will be replaced by the HandlerHelper. 
            // Place service specific scaffolded code under the service folder 
            context.HandlerHelper.TokenReplacementValues.Add("Instance:Endpoint", tfsContext.Name + CONFIGKEY_TFSURI);
            context.HandlerHelper.TokenReplacementValues.Add("Instance:TeamProjectName", tfsContext.Name + CONFIGKEY_TEAMPROJECTNAME);
            context.HandlerHelper.TokenReplacementValues.Add("Instance:TeamProjectCollectionName", tfsContext.Name + CONFIGKEY_TEAMPROJECTCOLLECTIONNAME);

            string serviceFolder = string.Format("Service References\\{0}\\", tfsContext.Name);

            await context.HandlerHelper.AddFileAsync(templateResourceUri, 
                Path.Combine(context.HandlerHelper.GetServiceArtifactsRootFolder(),
                             context.ServiceInstance.Name,
                             "VSOnlineService.cs"));

            templateResourceUri = "pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Templates/WorkItem.cs";
            await context.HandlerHelper.AddFileAsync(templateResourceUri,
                Path.Combine("Models",
                                    context.ServiceInstance.Name,
                                    "WorkItem.cs"));
                                    

        }
    }
}