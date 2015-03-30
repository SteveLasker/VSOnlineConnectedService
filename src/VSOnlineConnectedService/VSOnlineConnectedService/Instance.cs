using Microsoft.VisualStudio.ConnectedServices;

namespace VSOnlineConnectedService
{
    public class Instance : ConnectedServiceInstance
    {
        //Purpose of this class is to marshall data between the Provider's ViewModel and the Handler

        public string VSOnlineUri { get; set; }

        public string TeamProjectCollectionName { get; set; }

        public string TeamProjectName { get; set; }

        public RuntimeAuthOptions RuntimeAuthOption { get; set; }
    }
}
