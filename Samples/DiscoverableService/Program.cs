namespace DiscoverableService
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;
    using Contracts;

    class Program
    {
        static void Main()
        {
            var dnsName = Dns.GetHostName();
            // Define the base address of the service
            var baseAddress = new Uri(string.Format("net.tcp://{0}:9002/CalculatorService/{1}", dnsName, Guid.NewGuid().ToString()));
            // Define the endpoint address where announcement messages will be sent
            var announcementEndpointAddress = new Uri(string.Format("net.tcp://{0}:9021/Announcement", dnsName));

            // Create the service host
            var serviceHost = new ServiceHost(typeof(CalculatorService), baseAddress);
            try
            {
                // Add a service endpoint
                var netTcpEndpoint = serviceHost.AddServiceEndpoint(typeof(ICalculatorService), new NetTcpBinding(), string.Empty);

                // Create an announcement endpoint, which points to the Announcement Endpoint hosted by the proxy service.
                var announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(), new EndpointAddress(announcementEndpointAddress));

                // Create a ServiceDiscoveryBehavior and add the announcement endpoint
                var serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
                serviceDiscoveryBehavior.AnnouncementEndpoints.Add(announcementEndpoint);

                // Add the ServiceDiscoveryBehavior to the service host to make the service discoverable
                serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior);

                // Start listening for messages
                serviceHost.Open();

                Console.WriteLine("Calculator Service started at {0}", baseAddress);
                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }

            if (serviceHost.State != CommunicationState.Closed)
            {
                Console.WriteLine("Aborting the service...");
                serviceHost.Abort();
            }
        }
    }
}
