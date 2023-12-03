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

            string keyFile = "C:/Bezbednost/clientKey.txt";

            string key = SecretKey.GenerateKey();

            SecretKey.StoreKey(key, keyFile);
            //string message = "12345678";
            Console.WriteLine("Unesite poruku : ");
            string message = Console.ReadLine();
            while (message.Length == 0)
            {
                Console.WriteLine("Nste uneli poruku : ");
                message = Console.ReadLine();
                
            }
            message = MessageForSend(message);
            byte[] toEncrypt = ASCIIEncoding.ASCII.GetBytes(message);
            byte[] encripted = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            //string putanja = "D:/Program Files (x86)/TEST/encripted.txt";
            byte[] decripted = _3DES_Algorithm.Decrypt(key, System.Security.Cryptography.CipherMode.ECB, encripted);
            //File.WriteAllText(putanja, encripted);
            Entitet entitet = new Entitet();
            entitet.Id = 13;
            string Korisnik = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
         
            using (ClientFactory proxy = new ClientFactory(binding, address))
            {


                proxy.TestConnection(Korisnik);
                proxy.Read(Korisnik, entitet);
                proxy.Modify(Korisnik, entitet);
                proxy.Supervise(Korisnik, entitet);

                Console.ReadLine();
            }
        }

        static string MessageForSend(string message)
        {


            string retVal;
            char[] m = message.ToCharArray();
            if (m.Length % 8 != 0)
            {
                int n = 8 - (m.Length % 8);
                Array.Resize(ref m, m.Length + n);
                for (int i = m.Length; i < 7; i++)
                {
                    m[i] = ' ';
                }
            }
            retVal = new string(m);
          
            return retVal;

        }
    }
}
