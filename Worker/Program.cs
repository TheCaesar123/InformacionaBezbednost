using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8003/IProsledi";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + WindowsIdentity.GetCurrent().Name);
            string poruka = "";
            string poruka2= "";
            ServiceHost host = new ServiceHost(typeof(WorkerToLB));
            host.AddServiceEndpoint(typeof(IProsledi), binding, address);
            host.Open();
            Entitet e = new Entitet();
            using (WorkerProxy proxy = new  WorkerProxy(binding, address))
            {
                Console.WriteLine("WORKER POVEZAN NA LOADBALANCER");
                proxy.Prosledi(poruka, poruka2);
               // proxy.LBToWorker(e);
                
            }
            Console.ReadKey();
            host.Close();

        }
    }
}
