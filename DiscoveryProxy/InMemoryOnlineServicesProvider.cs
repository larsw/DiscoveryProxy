namespace DiscoveryProxy
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;

    public class InMemoryOnlineServicesProvider : IOnlineServicesProvider
    {
        // Repository to store EndpointDiscoveryMetadata. A database or a flat file could also be used instead.
        private readonly Dictionary<EndpointAddress, EndpointDiscoveryMetadata> _onlineServices;

        public InMemoryOnlineServicesProvider()
        {
            _onlineServices = new Dictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }

        // The following are helper methods required by the Proxy implementation
        public void AddOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            lock (_onlineServices)
            {
                _onlineServices[endpointDiscoveryMetadata.Address] = endpointDiscoveryMetadata;
            }
            // Replace with log4net
            //PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Adding");
        }

        public void RemoveOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            if (endpointDiscoveryMetadata != null)
            {
                lock (_onlineServices)
                {
                    _onlineServices.Remove(endpointDiscoveryMetadata.Address);
                }
                // Replace with log4net
                //PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Removing");
            }
        }

        public void MatchFromOnlineService(FindRequestContext findRequestContext)
        {
            lock (_onlineServices)
            {
                foreach (var endpointDiscoveryMetadata in _onlineServices.Values)
                {
                    if (findRequestContext.Criteria.IsMatch(endpointDiscoveryMetadata))
                    {
                        findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                    }
                }
            }
        }

        public EndpointDiscoveryMetadata MatchFromOnlineService(ResolveCriteria criteria)
        {
            EndpointDiscoveryMetadata matchingEndpoint = null;
            lock (_onlineServices)
            {
                foreach (var endpointDiscoveryMetadata in _onlineServices.Values)
                {
                    if (criteria.Address == endpointDiscoveryMetadata.Address)
                    {
                        matchingEndpoint = endpointDiscoveryMetadata;
                    }
                }
            }
            return matchingEndpoint;
        }
    }
}