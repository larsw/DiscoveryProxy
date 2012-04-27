namespace DiscoveryProxy
{
    using System.ServiceModel.Discovery;

    public interface IOnlineServicesRepository
    {
        void Add(EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        void Remove(EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        void Match(FindRequestContext findRequestContext);
        EndpointDiscoveryMetadata Match(ResolveCriteria criteria);
    }
}