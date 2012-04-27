namespace DiscoveryProxy.Console
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;

    internal class Program
    {
        public static void Main()
        {
            var dnsName = Dns.GetHostName();
            var probeEndpointAddress = new Uri(string.Format("net.tcp://{0}:8001/Probe", dnsName));
            var announcementEndpointAddress = new Uri(string.Format("net.tcp://{0}:9021/Announcement", dnsName));

            // Host the DiscoveryProxy service
            var logger = new ConsoleLogger();
            var proxyServiceHost = new ServiceHost(new DiscoveryProxyService(new InMemoryOnlineServicesRepository(logger), logger));

            try
            {
                // Add DiscoveryEndpoint to receive Probe and Resolve messages
                var discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(),
                                                              new EndpointAddress(probeEndpointAddress))

                                            {
                                                IsSystemEndpoint = false
                                            };

                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                var announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(),
                                                                    new EndpointAddress(announcementEndpointAddress));

                proxyServiceHost.AddServiceEndpoint(discoveryEndpoint);
                proxyServiceHost.AddServiceEndpoint(announcementEndpoint);

                proxyServiceHost.Open();

                Console.WriteLine("Proxy Service started.");
                Console.WriteLine("Probe endpoint: " + probeEndpointAddress);
                Console.WriteLine("Announcement endpoint: " + announcementEndpointAddress);
                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();

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