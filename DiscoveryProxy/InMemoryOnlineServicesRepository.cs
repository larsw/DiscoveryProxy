namespace DiscoveryProxy
{
    using System.ServiceModel;
    using System.ServiceModel.Discovery;
    using System;
    using System.Collections.Concurrent;

    public class InMemoryOnlineServicesRepository : IOnlineServicesRepository
    {
        // Repository to store EndpointDiscoveryMetadata. A database or a flat file could also be used instead.
        private readonly ConcurrentDictionary<EndpointAddress, EndpointDiscoveryMetadata> _onlineServices;
        private ILogger _logger;

        public InMemoryOnlineServicesRepository()
            : this(new NullLogger())
        {

        }

        public InMemoryOnlineServicesRepository(ILogger logger)
        {
            if (_logger == null) throw new ArgumentNullException("logger");

            _logger = logger;
            _onlineServices = new ConcurrentDictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }

        // The following are helper methods required by the Proxy implementation
        public void Add(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            _onlineServices.TryAdd(endpointDiscoveryMetadata.Address, endpointDiscoveryMetadata);
            //PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Adding");
        }

        public void Remove(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            if (endpointDiscoveryMetadata != null)
            {
                EndpointDiscoveryMetadata tmp;
                _onlineServices.TryRemove(endpointDiscoveryMetadata.Address, out tmp);
                //PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Removing");
            }
        }

        public void Match(FindRequestContext findRequestContext)
        {
            foreach (var endpointDiscoveryMetadata in _onlineServices.Values)
            {
                if (findRequestContext.Criteria.IsMatch(endpointDiscoveryMetadata))
                {
                    findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                }
            }
        }

        public EndpointDiscoveryMetadata Match(ResolveCriteria criteria)
        {
            EndpointDiscoveryMetadata matchingEndpoint = null;
            foreach (var endpointDiscoveryMetadata in _onlineServices.Values)
            {
                if (criteria.Address == endpointDiscoveryMetadata.Address)
                {
                    matchingEndpoint = endpointDiscoveryMetadata;
                }
            }
            return matchingEndpoint;
        }
    }
}