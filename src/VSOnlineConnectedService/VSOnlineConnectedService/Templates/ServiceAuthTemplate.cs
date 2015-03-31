using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace $rootnamespace$
{
    public class TFSWorkItemStoreConnector
    {
        public TfsConfigurationServer AuthenticateToTFSService()
        {
            Uri tfsUri = new Uri(GetConfigValue("$Instance.VSOnlineUri$"));
            string username = GetConfigValue("$Instance.Username$");
            string password = GetConfigValue("$Instance.Password$");
            NetworkCredential netCred = new NetworkCredential(username, password);
            BasicAuthCredential basicCred = new BasicAuthCredential(netCred);
            TfsClientCredentials tfsCred = new TfsClientCredentials(basicCred);
            tfsCred.AllowInteractive = false;

            TfsConfigurationServer configurationServer = new TfsConfigurationServer(tfsUri, tfsCred);

            configurationServer.Authenticate();
            return configurationServer;
        }

        public CatalogNode GetTeamProjectCollection(TfsConfigurationServer tfs)
        {
            ReadOnlyCollection<CatalogNode> teamProjectCatalogs = tfs.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);

            return teamProjectCatalogs.FirstOrDefault(p => p.Resource.DisplayName == GetConfigValue("$Instance.TeamProjectCollectionName$"));
        }

        public WorkItemStore GetWorkItemStore(TfsConfigurationServer tfs, CatalogNode teamProjectColCatalog)
        {
            Guid teamProjectCatalogId = new Guid(teamProjectColCatalog.Resource.Properties["InstanceId"]);
            TfsTeamProjectCollection teamProjectCollection = tfs.GetTeamProjectCollection(teamProjectCatalogId);
            return new WorkItemStore(teamProjectCollection);
        }

        public Project GetWorkItemStoreProject(WorkItemStore workItemStore)
        {
            return workItemStore.Projects[GetConfigValue("$Instance.TeamProjectName$")];
        }

        /// <summary>
        /// Retrieves values from app/web.config files
        /// </summary>
        /// <param name="keyName">The key within app/web.config</param>
        /// <returns>The value found</returns>
        /// <exception cref="InvalidOperationException">If the key isn't found</exception>
        private string GetConfigValue(string keyName)
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
