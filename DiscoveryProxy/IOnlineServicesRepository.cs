namespace DiscoveryProxy
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Discovery;
    using System.Collections;

    public interface IOnlineServicesRepository
    {
        void Add(EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        void Remove(EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        void Match(FindRequestContext findRequestContext);
        EndpointDiscoveryMetadata Match(ResolveCriteria criteria);
        IEnumerable<OnlineService> GetAllOnlineServices();
    }

    public class OnlineService
    {
        public EndpointDiscoveryMetadata Metadata { get; set; }
        public DateTime Added { get; set; }

        public OnlineService(EndpointDiscoveryMetadata metadata, DateTime added)
        {
            Metadata = metadata;
            Added = added;
        }
    }
}