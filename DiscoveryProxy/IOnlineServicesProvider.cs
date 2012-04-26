namespace DiscoveryProxy
{
    using System.ServiceModel.Discovery;

    public interface IOnlineServicesProvider
    {
        void AddOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        void RemoveOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        void MatchFromOnlineService(FindRequestContext findRequestContext);
        EndpointDiscoveryMetadata MatchFromOnlineService(ResolveCriteria criteria);
    }
}