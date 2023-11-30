using Common;
using System;
using System.ServiceModel;

namespace LoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8002/IProsledi";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(ProslediServer));
            host.AddServiceEndpoint(typeof(IProsledi), binding, address);
            host.Open();
            Console.WriteLine("LoadBalancer je uspesno pokrenut ");
            Console.ReadKey();
            host.Close();



        }
    }
}
