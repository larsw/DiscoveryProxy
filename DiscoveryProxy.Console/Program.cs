namespace DiscoveryProxy.Console
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Discovery;

    internal class Program
    {
        public static void Main()
        {
            var dnsName = Dns.GetHostName();
            var probeEndpointAddress = new Uri(string.Format("http://{0}:8001/Probe", dnsName));
            var announcementEndpointAddress = new Uri(string.Format("http://{0}:8001/Announcement", dnsName));
            var managementEndpointAddress = new Uri(string.Format("http://{0}:8001/Management", dnsName));

            // Host the DiscoveryProxy service
            var logger = new ConsoleLogger();
            var repository = new InMemoryOnlineServicesRepository(logger);
            var proxyServiceHost = new ServiceHost(new DiscoveryProxyService(repository, logger));
            var managementServiceHost = new ServiceHost(new ManagementResource(repository), managementEndpointAddress);
            var ep = managementServiceHost.AddServiceEndpoint(typeof (ManagementResource), new WebHttpBinding(), string.Empty);
            ep.Behaviors.Add(new WebHttpBehavior());
            try
            {
                // Add DiscoveryEndpoint to receive Probe and Resolve messages
                var discoveryEndpoint = new DiscoveryEndpoint(new WSHttpBinding(SecurityMode.None), 
                                                              new EndpointAddress(probeEndpointAddress))

                                            {
                                                IsSystemEndpoint = false
                                            };

                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                var announcementEndpoint = new AnnouncementEndpoint(new WSHttpBinding(SecurityMode.None),
                                                                    new EndpointAddress(announcementEndpointAddress));

                proxyServiceHost.AddServiceEndpoint(discoveryEndpoint);
                proxyServiceHost.AddServiceEndpoint(announcementEndpoint);

                proxyServiceHost.Open();

                managementServiceHost.Open();

                Console.WriteLine("Proxy Service started.");
                Console.WriteLine("Probe endpoint: " + probeEndpointAddress);
                Console.WriteLine("Announcement endpoint: " + announcementEndpointAddress);
                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();

                managementServiceHost.Close();

                proxyServiceHost.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }

            if (proxyServiceHost.State != CommunicationState.Closed)
            {
                Console.WriteLine("Aborting the service...");
                proxyServiceHost.Abort();
            }
        }
    }
}