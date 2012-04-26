namespace DiscoveryProxy.Console
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;

    internal class Program
    {
        public static void Main()
        {
            var probeEndpointAddress = new Uri("net.tcp://localhost:8001/Probe");
            var announcementEndpointAddress = new Uri("net.tcp://localhost:9021/Announcement");

            // Host the DiscoveryProxy service
            var proxyServiceHost = new ServiceHost(new DiscoveryProxyService());

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