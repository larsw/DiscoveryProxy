namespace DiscoveryProxy
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;
    using System;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Text;

    public class InMemoryOnlineServicesRepository : IOnlineServicesRepository
    {
        // Repository to store EndpointDiscoveryMetadata. A database or a flat file could also be used instead.
        private readonly ConcurrentDictionary<EndpointAddress, OnlineService> _onlineServices;
        private readonly ILogger _logger;

        public InMemoryOnlineServicesRepository()
            : this(new NullLogger())
        {
        }

        public InMemoryOnlineServicesRepository(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");

            _logger = logger;
            _onlineServices = new ConcurrentDictionary<EndpointAddress, OnlineService>();
        }

        // The following are helper methods required by the Proxy implementation
        public void Add(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            _onlineServices.TryAdd(endpointDiscoveryMetadata.Address, new OnlineService(endpointDiscoveryMetadata, DateTime.Now));
            _logger.Log("Adding endpoint:\r\n" + endpointDiscoveryMetadata.ToLog(), LogLevel.Debug);
        }

        public void Remove(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            if (endpointDiscoveryMetadata != null)
            {
                OnlineService tmp;
                _onlineServices.TryRemove(endpointDiscoveryMetadata.Address, out tmp);
                _logger.Log("Removing " + endpointDiscoveryMetadata.Address, LogLevel.Debug);
            }
        }

        public void Match(FindRequestContext findRequestContext)
        {
            _logger.Log("Matching " + findRequestContext.Criteria, LogLevel.Debug);

            foreach (var endpointDiscoveryMetadata in _onlineServices.Values.Where(x => findRequestContext.Criteria.IsMatch(x.Metadata)))
            {
                _logger.Log("...found " + endpointDiscoveryMetadata, LogLevel.Debug);
                findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata.Metadata);
            }
        }

        public EndpointDiscoveryMetadata Match(ResolveCriteria criteria)
        {
            EndpointDiscoveryMetadata matchingEndpoint = null;
            _logger.Log("Matching " + criteria, LogLevel.Debug);
            foreach (var onlineService in _onlineServices.Values.Where(x => criteria.Address == x.Metadata.Address))
            {
                _logger.Log("...found " + criteria.Address, LogLevel.Debug);
                matchingEndpoint = onlineService.Metadata;
            }
            return matchingEndpoint;
        }

        public IEnumerable<OnlineService> GetAllOnlineServices()
        {
            return _onlineServices.Values;
        }
    }

    internal static class EndpointDiscoveryMetadataExtensions
    {
        public static string ToLog(this EndpointDiscoveryMetadata metadata)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\tAddress: " + metadata.Address);
            sb.AppendLine("\tVersion: " + metadata.Version);
            sb.AppendLine("\tContract Type Names:");
            foreach (var name in metadata.ContractTypeNames)
            {
                sb.AppendLine("\t\t" + name);
            }
            sb.AppendLine("\tExtensions:");
            foreach (var extension in metadata.Extensions)
            {
                sb.AppendLine("\t\t" + extension);
            }
            sb.AppendLine("\tListen URIs:");
            foreach (var listenUri in metadata.ListenUris)
            {
                sb.AppendLine("\t\t" + listenUri);
            }
            sb.AppendLine("\tScopes:");
            foreach (var scope in metadata.Scopes)
            {
                sb.AppendLine("\t\t" + scope);
            }
            return sb.ToString();
        }
    }
}