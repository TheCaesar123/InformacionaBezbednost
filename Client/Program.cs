using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {

        public static void Main(string[] args)
        {
            
            string srvCertCN = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:8001/Servis"), new X509CertificateEndpointIdentity(srvCert));

            string keyFile = "D:/Program Files (x86)/InformacionaBezbednost/clientKey.txt";

            string key = SecretKey.GenerateKey();

            SecretKey.StoreKey(key, keyFile);
            byte[] sdf = null;
            using (ClientFactory proxy = new ClientFactory(binding, address))
            {
                proxy.TestConnection();
                proxy.Read(sdf);
                proxy.Modify();
                proxy.Supervise();
                Console.ReadLine();
            }
        }
    }
}
