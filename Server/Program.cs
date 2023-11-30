using Common;
using Manager;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            
            NetTcpBinding loadBalancerBinding = new NetTcpBinding();
            string loadBalancerAddress = "net.tcp://localhost:8002/IProsledi";

            loadBalancerBinding.Security.Mode = SecurityMode.Transport;
            loadBalancerBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            loadBalancerBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + WindowsIdentity.GetCurrent().Name);
            using (ServerProxy proxy = new ServerProxy(loadBalancerBinding, loadBalancerAddress))
            {
                Console.WriteLine("SERVER POVEZAN NA LoadBalancer");
                proxy.Prosledi();

            }

            string srvCertCN = "wcfservice";
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:8001/Servis";
            ServiceHost host = new ServiceHost(typeof(EntitetServer));
            host.AddServiceEndpoint(typeof(IEntitet), binding, address);

            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            //host.Authorization.ServiceAuthorizationManager = new MyAuthorizationManager();

            host.Open();
            Console.WriteLine("Servis je uspesno pokrenut ");
            Console.ReadKey();

            host.Close();



        }
    }
}

