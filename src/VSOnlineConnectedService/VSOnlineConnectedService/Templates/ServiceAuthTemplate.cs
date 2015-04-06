using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http.Headers;
using System.Configuration;

namespace $rootnamespace$
{
    public class VSOnlineService
    {
        public VSOnlineService()
        {
            this.Initialize();
        }

        #region Properties
        /// <summary>
        /// VSOnline API Version used for requests
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// The VSOnline base endpoint to query for your tenant eg: https://Contoso.VisualStudio.com 
        /// </summary>
        /// <remarks>
        /// The value will be read from app/web.config by default in the construction of this object
        /// </remarks>
        public string Endpoint { get; set; }

        /// <summary>
        /// The maximum number of rows to return
        /// </summary>
        /// <remarks>
        /// Default of 100
        /// </remarks>
        public int MaxResultRows;

        /// <summary>
        /// The list of fields to return from VSOnline for queries
        /// </summary>
        public List<string> ResultFields { get; set; }

        #endregion

        /// <summary>
        /// Returns a collection of WorkItems T, based on the query
        /// </summary>
        /// <param name="query">Select [System.Id] From WorkItems Where[System.WorkItemType] = 'Bug' order by [System.CreatedDate] desc</param>
        /// <returns>A collection of T WorkItems</returns>
        /// <remarks>
        /// In this version, the list of properties queried and populated in T is based on the ResultFields. 
        /// However, you must specificy [System.Id] in the query for the current REST API to work
        /// </remarks>
        public async Task<List<T>> GetWorkItems<T>(string query)
        {
            // The collection of workitems to return
            List<T> vsoWorkItems = new List<T>();
            using (var client = GetClient())
            {
                // Execute a query that returns work item IDs matching the specified criteria
                using (var request = new HttpRequestMessage(HttpMethod.Post, this.Endpoint + "wiql"))
                {
                    request.Headers.Add("Accept", "application/json;api-version=" + this.ApiVersion);

                    Dictionary<string, string> body = new Dictionary<string, string>
                        {
                            {
                                "query", query
                            }
                        };

                    request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                    using (var response = await client.SendAsync(request))
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var workItems = JObject.Parse(content)["workItems"] as JArray;

                        string[] ids = workItems.Select<JToken, string>(w => (w["id"] + "")).Take(this.MaxResultRows).ToArray<string>();
                        string idsString = String.Join(",", ids);
                        string fields = String.Join(",", ResultFields);

                        // Get details for the list of WorkItem Ids
                        using (var detailsRequest = new HttpRequestMessage(HttpMethod.Get,
                            this.Endpoint + "workitems?ids=" + idsString + "&fields=" + fields))
                        {
                            detailsRequest.Headers.Add("Accept", "application/json;api-version=" + this.ApiVersion);

                            using (var detailsResponse = await client.SendAsync(detailsRequest))
                            {
                                var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                                if (detailsResponse.IsSuccessStatusCode)
                                {
                                    var detailsWorkItems = JObject.Parse(detailsContent)["value"] as JArray;

                                    foreach (dynamic workItem in detailsWorkItems)
                                    {
                                        vsoWorkItems.Add(JsonConvert.DeserializeObject<T>(workItem.fields.ToString()));
                                    }
                                } // if detailsResponse.IsSuccessStatusCode
                            } // client.SendAsync
                        } // HttpRequestMessage
                    } // client.SendAsync
                } // HttpRequestMessage
            } // var client = GetClient()
            return vsoWorkItems;
        }

        #region internal utility methods

        /// <summary>
        /// Gets a configured HttpClient for VSOnline
        /// </summary>
        /// <returns>A configured HttpClient</returns>
        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            string username = GetConfigValue("$Instance:Username$");
            string password = GetConfigValue("$Instance:Password$");

            // using basic auth for Service Account Scenario
            //TODO: Add OAuth support
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password))));
            return client;
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

        private void Initialize()
        {
            // Build the REST API
            // cleanup any trailing / in the configured URL
            string configUrl = GetConfigValue("$Instance:Endpoint$");
            if (configUrl.Substring(configUrl.Length - 1, 1) == "/")
            {
                configUrl = configUrl.Substring(0, configUrl.Length - 1);
            }

            this.Endpoint = configUrl + "/defaultcollection/_apis/wit/";

            ResultFields = new List<string>();
            ResultFields.AddRange(new string[] { "System.Id", "System.Title", "System.AreaPath", "System.TeamProject", "System.IterationPath", "System.WorkItemType", "System.State", "System.Reason", "System.CreatedDate", "System.CreatedBy", "System.ChangedDate", "System.ChangedBy", "System.Title", "Microsoft.VSTS.Common.Severity" });

            this.ApiVersion = "1.0";
            this.MaxResultRows = 100;
        }
        #endregion
    }
}