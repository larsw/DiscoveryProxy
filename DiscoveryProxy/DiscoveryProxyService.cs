namespace DiscoveryProxy
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;

    // Implement DiscoveryProxy by extending the DiscoveryProxy class and overriding the abstract methods
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DiscoveryProxyService : DiscoveryProxy
    {
        private readonly IOnlineServicesRepository _provider;
        private readonly ILogger _logger;

        public DiscoveryProxyService()
            :this(new InMemoryOnlineServicesRepository(), new NullLogger())
        {
            
        }

        public DiscoveryProxyService(IOnlineServicesRepository provider, ILogger logger)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (logger == null) throw new ArgumentNullException("logger");

            _provider = provider;
            _logger = logger;
        }

        // OnBeginOnlineAnnouncement method is called when a Hello message is received by the Proxy
        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                                  EndpointDiscoveryMetadata endpointDiscoveryMetadata,
                                                                  AsyncCallback callback, object state)
        {
            _logger.Log("OnBeginOnlineAnnouncement()", LogLevel.Debug);
            _provider.Add(endpointDiscoveryMetadata);
            return new OnOnlineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            _logger.Log("OnEndOnlineAnnouncement()", LogLevel.Debug);
            OnOnlineAnnouncementAsyncResult.End(result);
        }

        // OnBeginOfflineAnnouncement method is called when a Bye message is received by the Proxy
        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                                   EndpointDiscoveryMetadata endpointDiscoveryMetadata,
                                                                   AsyncCallback callback, object state)
        {
            _logger.Log("OnBeginOfflineAnnouncement()", LogLevel.Debug);
            _provider.Remove(endpointDiscoveryMetadata);
            return new OnOfflineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            _logger.Log("OnEndOfflineAnnouncement()", LogLevel.Debug);
            OnOfflineAnnouncementAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Probe request message is received by the Proxy
        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback,
                                                    object state)
        {
            _logger.Log("OnBeginFind()", LogLevel.Debug);
            _provider.Match(findRequestContext);
            return new OnFindAsyncResult(callback, state);
        }

        protected override void OnEndFind(IAsyncResult result)
        {
            _logger.Log("OnEndFind()", LogLevel.Debug);
            OnFindAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Resolve request message is received by the Proxy
        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback,
                                                       object state)
        {
            _logger.Log("OnBeginResolve()", LogLevel.Debug);
            return new OnResolveAsyncResult(_provider.Match(resolveCriteria), callback, state);
        }

        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            _logger.Log("OnEndResolve()", LogLevel.Debug);
            return OnResolveAsyncResult.End(result);
        }

        //private void PrintDiscoveryMetadata(EndpointDiscoveryMetadata endpointDiscoveryMetadata, string verb)
        //{
        //    Trace.WriteLine("\n**** " + verb + " service of the following type from cache. ");
        //    foreach (var contractName in endpointDiscoveryMetadata.ContractTypeNames)
        //    {
        //        Trace.WriteLine("** " + contractName);
        //        break;
        //    }
        //    Trace.WriteLine("**** Operation Completed");
        //}

        #region Nested AsyncResult types

        private sealed class OnFindAsyncResult : AsyncResult
        {
            public OnFindAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                End<OnFindAsyncResult>(result);
            }
        }

        private sealed class OnOfflineAnnouncementAsyncResult : AsyncResult
        {
            public OnOfflineAnnouncementAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                End<OnOfflineAnnouncementAsyncResult>(result);
            }
        }

        private sealed class OnOnlineAnnouncementAsyncResult : AsyncResult
        {
            public OnOnlineAnnouncementAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                End<OnOnlineAnnouncementAsyncResult>(result);
            }
        }

        private sealed class OnResolveAsyncResult : AsyncResult
        {
            private readonly EndpointDiscoveryMetadata _matchingEndpoint;

            public OnResolveAsyncResult(EndpointDiscoveryMetadata matchingEndpoint, AsyncCallback callback, object state)
                : base(callback, state)
            {
                _matchingEndpoint = matchingEndpoint;
                Complete(true);
            }

            public static EndpointDiscoveryMetadata End(IAsyncResult result)
            {
                var thisPtr = End<OnResolveAsyncResult>(result);
                return thisPtr._matchingEndpoint;
            }
        }

        #endregion
    }
}