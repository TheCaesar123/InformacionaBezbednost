using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Debugger.Launch();
            string srvCertCN = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:8001/Servis"), new X509CertificateEndpointIdentity(srvCert));

            using (ClientFactory proxy = new ClientFactory(binding, address))
            {
                proxy.Read();
                proxy.Modify();
                proxy.Supervise();
                Console.ReadLine();
            }
        }
    }
}
