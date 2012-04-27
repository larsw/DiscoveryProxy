namespace DiscoveryProxy
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;
    using System.ServiceModel.Web;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [ServiceContract(Namespace = "")]
    public class ManagementResource
    {
        private readonly IOnlineServicesRepository _repository;

        public ManagementResource(IOnlineServicesRepository repository)
        {
            _repository = repository;
        }

        [WebGet(UriTemplate = "", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public IEnumerable<RegisteredService> GetAllRegisteredServices()
        {
            return _repository.GetAllOnlineServices().Select(x => new RegisteredService
                                                                      {
                                                                          Address = x.Metadata.Address.ToString(),
                                                                          Added = x.Added,
                                                                          Metadata = new RegisteredServiceMetadata(x.Metadata)
                                                                      });
        }

        [DataContract(Name = "service")]
        public class RegisteredService
        {
            [DataMember(Name = "address")]
            public string Address { get; set; }

            [DataMember(Name = "added")]
            public System.DateTime Added { get; set; }

            [DataMember(Name = "metadata")]
            internal RegisteredServiceMetadata Metadata { get; set; }
        }
    }

    [DataContract]
    public class RegisteredServiceMetadata
    {
        [DataMember(Name = "version")]
        public int Version { get; set; }

        [DataMember(Name = "scopes")]
        public IEnumerable<string> Scopes { get; set; }

        [DataMember(Name = "extensions")]
        public IEnumerable<string> Extensions { get; set; }

        [DataMember(Name = "listen-uris")]
        public IEnumerable<string> ListenUris { get; set; }

        [DataMember(Name = "contract-type-names")]
        public IEnumerable<string> ContractTypeNames { get; set; } 

        public RegisteredServiceMetadata()
        {

        }

        public RegisteredServiceMetadata(EndpointDiscoveryMetadata metadata)
        {
            Version = metadata.Version;
            Scopes = metadata.Scopes.Select(x => x.ToString());
            Extensions = metadata
                            .Extensions
                                .Select(x => string.Format("<{0}:{1}>{2}</{0}>", 
                                                           x.GetPrefixOfNamespace(x.GetDefaultNamespace()), 
                                                           x.Name, 
                                                           x.Value));
            ListenUris = metadata.ListenUris.Select(x => x.ToString());
            ContractTypeNames = metadata.ContractTypeNames.Select(x => string.Format("{0}/{1}", x.Namespace, x.Name));
        }
    }
}