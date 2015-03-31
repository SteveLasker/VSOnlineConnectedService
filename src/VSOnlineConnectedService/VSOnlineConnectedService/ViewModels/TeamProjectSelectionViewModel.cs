using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.TeamFoundation.Client;
using VSOnlineConnectedService;
using System.Diagnostics;
using System.Windows.Forms;
using VSOnlineConnectedService.Views;

namespace VSOnlineConnectedService.ViewModels
{
    public class TeamProjectSelectionViewModel : ConnectedServiceWizardPage
    {
        TeamProjectSelectionView _tpsUserCtrl;
        public TeamProjectSelectionViewModel()
        {
            this.Legend = "Connection";
            this.Title = "VSOnline: Configure the Connection";
            this.Description = "Select the Team Server";

            _tpsUserCtrl = new TeamProjectSelectionView();

            this.View = this._tpsUserCtrl;
            this.View.DataContext = this;
        }
        private string _vsOnlineURI;
        public string VSOnlineURI
        {
            get { return _vsOnlineURI; }
            set
            {
                if (value != _vsOnlineURI)
                {
                    _vsOnlineURI = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _teamProjectCollectionName;
        public string TeamProjectCollectionName
        {
            get { return _teamProjectCollectionName; }
            set
            {
                if(value!= _teamProjectCollectionName)
                {
                    _teamProjectCollectionName = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string  _teamProjectName;
        public string  TeamProjectName
        {
            get { return _teamProjectName; }
            set
            {
                if (value != _teamProjectName)
                {
                    _teamProjectName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private RuntimeAuthOptions _runTimeAuthOptions;
        internal RuntimeAuthOptions RuntimeAuthOptions
        {
            get { return _runTimeAuthOptions; }
            set
            {
                if (value != _runTimeAuthOptions)
                {
                    _runTimeAuthOptions = value;
                    this.OnPropertyChanged();
                }
            }
        }


        public void GetTFSConfiguration()
        {
            //To keep things simple, only allow a single team project to be selected using the TFS picker control
            using (TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false))
            {
                var result = tpp.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //Store the TFS picker control's selections to be used later in scaffolding
                    TfsConfigurationServer tfs = tpp.SelectedTeamProjectCollection.ConfigurationServer;

                    this.VSOnlineURI = tfs.Uri.ToString();
                    this.TeamProjectCollectionName = tpp.SelectedTeamProjectCollection.Name.Replace(tfs.Name + "\\", String.Empty);

                    //The picker control is configured to only allow one project to be selected
                    Debug.Assert(tpp.SelectedProjects.Length == 1, "User should only be able to select one team project.");
                    this.TeamProjectName = tpp.SelectedProjects[0].Name;
                }
            }
            // After configuration, trigger state validation to set the buttons
            //((ServiceWizard)(this.Wizard)).ValidateState();
        }
    }
}