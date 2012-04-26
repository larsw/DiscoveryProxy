namespace DiscoveryProxy
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;
    using System.Xml;

    // Implement DiscoveryProxy by extending the DiscoveryProxy class and overriding the abstract methods
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DiscoveryProxyService : DiscoveryProxy
    {
        // Repository to store EndpointDiscoveryMetadata. A database or a flat file could also be used instead.
        private readonly Dictionary<EndpointAddress, EndpointDiscoveryMetadata> _onlineServices;

        public DiscoveryProxyService()
        {
            _onlineServices = new Dictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }

        // OnBeginOnlineAnnouncement method is called when a Hello message is received by the Proxy
        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                                  EndpointDiscoveryMetadata endpointDiscoveryMetadata,
                                                                  AsyncCallback callback, object state)
        {
            AddOnlineService(endpointDiscoveryMetadata);
            return new OnOnlineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            OnOnlineAnnouncementAsyncResult.End(result);
        }

        // OnBeginOfflineAnnouncement method is called when a Bye message is received by the Proxy
        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                                   EndpointDiscoveryMetadata endpointDiscoveryMetadata,
                                                                   AsyncCallback callback, object state)
        {
            RemoveOnlineService(endpointDiscoveryMetadata);
            return new OnOfflineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            OnOfflineAnnouncementAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Probe request message is received by the Proxy
        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback,
                                                    object state)
        {
            MatchFromOnlineService(findRequestContext);
            return new OnFindAsyncResult(callback, state);
        }

        protected override void OnEndFind(IAsyncResult result)
        {
            OnFindAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Resolve request message is received by the Proxy
        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback,
                                                       object state)
        {
            return new OnResolveAsyncResult(MatchFromOnlineService(resolveCriteria), callback, state);
        }

        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return OnResolveAsyncResult.End(result);
        }

        // The following are helper methods required by the Proxy implementation
        private void AddOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            lock (_onlineServices)
            {
                _onlineServices[endpointDiscoveryMetadata.Address] = endpointDiscoveryMetadata;
            }

            PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Adding");
        }

        private void RemoveOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            if (endpointDiscoveryMetadata != null)
            {
                lock (_onlineServices)
                {
                    _onlineServices.Remove(endpointDiscoveryMetadata.Address);
                }

                PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Removing");
            }
        }

        private void MatchFromOnlineService(FindRequestContext findRequestContext)
        {
            lock (_onlineServices)
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in _onlineServices.Values)
                {
                    if (findRequestContext.Criteria.IsMatch(endpointDiscoveryMetadata))
                    {
                        findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                    }
                }
            }
        }

        private EndpointDiscoveryMetadata MatchFromOnlineService(ResolveCriteria criteria)
        {
            EndpointDiscoveryMetadata matchingEndpoint = null;
            lock (_onlineServices)
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in _onlineServices.Values)
                {
                    if (criteria.Address == endpointDiscoveryMetadata.Address)
                    {
                        matchingEndpoint = endpointDiscoveryMetadata;
                    }
                }
            }
            return matchingEndpoint;
        }

        private void PrintDiscoveryMetadata(EndpointDiscoveryMetadata endpointDiscoveryMetadata, string verb)
        {
            Console.WriteLine("\n**** " + verb + " service of the following type from cache. ");
            foreach (XmlQualifiedName contractName in endpointDiscoveryMetadata.ContractTypeNames)
            {
                Console.WriteLine("** " + contractName);
                break;
            }
            Console.WriteLine("**** Operation Completed");
        }

        #region Nested type: OnFindAsyncResult

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

        #endregion

        #region Nested type: OnOfflineAnnouncementAsyncResult

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

        #endregion

        #region Nested type: OnOnlineAnnouncementAsyncResult

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

        #endregion

        #region Nested type: OnResolveAsyncResult

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