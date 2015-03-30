using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSProvider
{
    public enum RuntimeAuthOptions
    {
        IntegratedAuth,
        BasicAuth,
        None
    };

    public class TFSConnectedServiceInstance : ConnectedServiceInstance
    {
        //Purpose of this class is to marshall data between the Provider's ViewModel and the Handler
        //Make this a singleton - is this a commmon pattern that we would recommend?
        static TFSConnectedServiceInstance()
        {
            TFSConnectedServiceInstance.Current = new TFSConnectedServiceInstance();
        }

        public static TFSConnectedServiceInstance Current { get; }

        public string TFSUri { get; set; }

        public string TeamProjectCollectionName { get; set; }

        public string TeamProjectName { get; set; }

        public RuntimeAuthOptions RuntimeAuthOption { get; set; }
    }
}
