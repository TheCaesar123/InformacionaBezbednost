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
        public static Dictionary<int, Entitet> entiteti { get; set; } = new Dictionary<int, Entitet>();
        public static void Main(string[] args)
        {

            string srvCertCN = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:8001/Servis"), new X509CertificateEndpointIdentity(srvCert));


            //  string signCertCN = "wcfreader_sign";
            //string keyFile = "C:/Bezbednost/clientKey.txt";

            //string key = SecretKey.GenerateKey();

            //SecretKey.StoreKey(key, keyFile);
            //string message = "12345678";

            DateTime t = DateTime.Now;
            Console.WriteLine(t);
            string allEntities = File.ReadAllText("C:/Bezbednost/Baza.txt");
            string[] entitites = allEntities.Split(';');
            for (int i = 0; i < entitites.Length-1; i++)
            {
                string[] entityParts = entitites[i].Split('|');
                int id = Int32.Parse(entityParts[0]);
                string name = entityParts[1];
                string SID = entityParts[2];
                DateTime time = DateTime.Parse(entityParts[3]);
                Entitet e = new Entitet { Id = id, Name = name, SID = SID, Time = time };
                entiteti.Add(e.Id, e);
            }

            //  byte[] toEncrypt = ASCIIEncoding.ASCII.GetBytes(message);
            /*byte[] encripted = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            string potpisanaPoruka = ASCIIEncoding.ASCII.GetString(encripted);*/
            Entitet entitet = new Entitet();
            
            
          // string Korisnik =  "wcfreader";
         
            using (ClientFactory proxy = new ClientFactory(binding, address))
            {
                //  string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
                // X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);
                //byte[] signature = DigitalSignature.Create(potpisanaPoruka, "SHA1", certificateSign);


                /* proxy.TestConnection(entitet.SID);
                 proxy.Read(entitet, encripted, signature);
                 proxy.Modify(entitet, encripted, signature);
                 proxy.Supervise(entitet, encripted, signature);
                 proxy.DataFromServerToCLientDecripted();*/
                Menu(proxy);
                Console.ReadLine();
            }

        }
       
        static void Menu(ClientFactory proxy)
        {

            int opcija = 0;
            string korisnik = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            Console.WriteLine("1 - Napravi dogadjaj");
            Console.WriteLine("2 - READ");
            Console.WriteLine("3 - MODIFY");
            Console.WriteLine("4 - SUPERVISE");
            Console.WriteLine("5 - SUBSCRIBE");
            
           
            do
            {
                Console.WriteLine("Odaberite opciju : ");

                opcija = int.Parse(Console.ReadLine());

                switch (opcija)
                {
                    case 1:
                        Create();
                        break;
                    case 2:

                        MenuRead(proxy, korisnik)
                            ;
                        break;
                   /* case 3:
                        proxy.Modify(korinik, signature);
                        break;
                    case 4:
                        proxy.Supervise(korinik, signature);
                        break;*/
                    default:
                        Console.WriteLine("Pogresna opcija");
                        break;
                }
            } while (opcija != 6);


        }
        static void MenuRead(ClientFactory proxy, string korisnik)
        {

            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);
            
            string key = SecretKey.GenerateKey();
            
            string keyFile = "C:/Bezbednost/clientKey.txt";
            SecretKey.StoreKey(key, keyFile);
            Console.WriteLine("Unesite ID koji zlite da procitate : ");
            string id = Console.ReadLine();
            string message = MessageForSend(id);
            byte[] encripted = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            string EnkriptovanaPoruka = ASCIIEncoding.ASCII.GetString(encripted);
           
            byte[] signature = DigitalSignature.Create(EnkriptovanaPoruka, "SHA1", certificateSign);
            proxy.Read(encripted, korisnik, signature);
            proxy.DataFromServerToCLientDecripted();
        }
        static void Create()
        {
            Console.WriteLine("Unesite ime entitetta: ");
            string ime = Console.ReadLine();
            int id = 0;
            do
            {
                Console.WriteLine("Unesite ID:");
                id = int.Parse(Console.ReadLine());
                if (entiteti.ContainsKey(id))
                {
                    Console.WriteLine("Uneti Id postoji!");
                    id = 0;
                }
            } while (id == 0);
            string SID = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            DateTime time = DateTime.Now;
            Entitet e = new Entitet { Id = id, Name = ime, SID = SID, Time = time };
            entiteti.Add(e.Id, e);
            string toWrite = "";
            foreach (Entitet en in entiteti.Values)
            {
                toWrite += en.ToString() + "\n";
            }
            File.WriteAllText("C:/Bezbednost/Baza.txt", toWrite);
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
