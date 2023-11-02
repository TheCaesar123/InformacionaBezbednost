using Common;
using System;
using System.Security.Principal;
using System.ServiceModel;

namespace Server
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

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + WindowsIdentity.GetCurrent().Name);
            using (ServerProxy proxy = new ServerProxy(binding, address))
            {
                Console.WriteLine("SERVER POVEZAN NA LoadBalancer");
                proxy.Prosledi();
                
            }
            using (ServiceHost host = new ServiceHost(typeof(EntitetServer)))
            {
                host.Open();
                Console.WriteLine("Servis je uspesno pokrenut ");
                Console.ReadKey();
                host.Close();
            }
        }
    }
}
