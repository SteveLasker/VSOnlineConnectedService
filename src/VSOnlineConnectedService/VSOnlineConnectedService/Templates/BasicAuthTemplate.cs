using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace $rootnamespace$
{
    public class TFSWorkItemStoreConnector
{
        public static TfsConfigurationServer AuthenticateToTFSService()
        {
            Uri tfsUri = new Uri(GetConfigValue("$Instance.VSOnlineUri$"));

            //If you specify a provider, the user will be provided with a prompt to provide non-default credentials
            ICredentialsProvider provider = new UICredentialsProvider();
            TfsConfigurationServer tfs = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri, provider);

            try
            {
                //Prompts the user for credentials if they have not already been authenticated
                tfs.EnsureAuthenticated();
            }
            catch (TeamFoundationServerUnauthorizedException)
            {
                //Handle the TeamFoundationServerUnauthorizedException that is thrown when the user clicks 'Cancel'
                //in the authentication prompt or if they are otherwise unable to be successfully authenticated
                tfs = null;
            }

            return tfs;
        }

        public static CatalogNode GetTeamProjectCollection(TfsConfigurationServer tfs)
        {
            ReadOnlyCollection<CatalogNode> teamProjectCatalogs = tfs.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);

            return teamProjectCatalogs.FirstOrDefault(p => p.Resource.DisplayName == GetConfigValue("$Instance.TeamProjectCollectionName$"));
        }

        public static WorkItemStore GetWorkItemStore(TfsConfigurationServer tfs, CatalogNode teamProjectColCatalog)
        {
            Guid teamProjectCatalogId = new Guid(teamProjectColCatalog.Resource.Properties["InstanceId"]);
            TfsTeamProjectCollection teamProjectCollection = tfs.GetTeamProjectCollection(teamProjectCatalogId);
            return new WorkItemStore(teamProjectCollection);
        }

        public static Project GetWorkItemStoreProject(WorkItemStore workItemStore)
        {
            return workItemStore.Projects[GetConfigValue("$Instance.TeamProjectName$")];
        }

        private static string GetConfigValue(string keyName)
        {
            string[] values = ConfigurationManager.AppSettings.GetValues(keyName);

            if (values.Length != 1)
            {
                throw new InvalidOperationException("Invalid .config values for the specified key: " + keyName);
            }

            return values[0];
        }
    }
}
