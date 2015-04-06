using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace $rootnamespace$
{
    public class WorkItem
    {
        [JsonProperty("System.Id")]
        public string Id { get; set; }

        [JsonProperty("System.Title")]
        public string Title { get; set; }

        [JsonProperty("System.AreaPath")]
        [Display(Name = "Area Path")]
        public string AreaPath { get; set; }

        [JsonProperty("System.TeamProject")]
        [Display(Name = "Team Project")]
        public string TeamProject { get; set; }

        [JsonProperty("System.IterationPath")]
        [Display(Name = "Iteration Path")]
        public string IterationPath { get; set; }

        [JsonProperty("System.WorkItemType")]
        [Display(Name = "WorkItem Type")]
        public string WorkItemType { get; set; }

        [JsonProperty("System.State")]
        public string State { get; set; }

        [JsonProperty("System.Reason")]
        public string Reason { get; set; }

        [JsonProperty("System.CreatedDate")]
        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("System.CreatedBy")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [JsonProperty("System.ChangedDate")]
        [Display(Name = "Changed")]
        public DateTime ChangedDate { get; set; }

        [JsonProperty("System.ChangedBy")]
        [Display(Name = "Changed By")]
        public string ChangedBy { get; set; }

        [JsonProperty("Microsoft.VSTS.Common.Severity")]
        public string Severity { get; set; }
    }
}