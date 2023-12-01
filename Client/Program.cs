using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            string keyFile = "D:/Program Files (x86)/TEST/clientKey.txt";

            string key = SecretKey.GenerateKey();

            SecretKey.StoreKey(key, keyFile);
            string message = "12345678";
            byte[] toEncrypt = ASCIIEncoding.ASCII.GetBytes(message);
            byte[] encripted = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            //string putanja = "D:/Program Files (x86)/TEST/encripted.txt";
            byte[] decripted = _3DES_Algorithm.Decrypt(key, System.Security.Cryptography.CipherMode.ECB, encripted);
            //File.WriteAllText(putanja, encripted);
            Entitet send = new Entitet(13);
            using (ClientFactory proxy = new ClientFactory(binding, address))
            {
                proxy.TestConnection();
                proxy.Read();
                proxy.Modify();
                proxy.Supervise();
                Console.ReadLine();
            }
        }
    }
}
