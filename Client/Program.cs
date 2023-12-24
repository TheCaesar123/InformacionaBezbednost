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



            string allEntities = File.ReadAllText("C:/Bezbednost/Baza.txt");
            string[] entitites = allEntities.Split(';');
            for (int i = 0; i < entitites.Length - 1; i++)
            {
                string[] entityParts = entitites[i].Split('|');
                int id = Int32.Parse(entityParts[0]);
                string name = entityParts[1];                                                        // Citanje pdataka iz baze
                string SID = entityParts[2];
                DateTime time = DateTime.Parse(entityParts[3]);
                Entitet e = new Entitet { Id = id, Name = name, SID = SID, Time = time };
                entiteti.Add(e.Id, e);
            }


            using (ClientFactory proxy = new ClientFactory(binding, address))
            {
                Menu(proxy);
                Console.ReadLine();
            }

        }

        static void Menu(ClientFactory proxy)
        {

            int opcija = 0;
            string korisnik = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);



            do
            {
                Console.WriteLine("1 - Napravi dogadjaj");
                Console.WriteLine("2 - READ");
                Console.WriteLine("3 - MODIFY");
                Console.WriteLine("4 - SUPERVISE");
          
                Console.WriteLine("Odaberite opciju : ");

                opcija = int.Parse(Console.ReadLine());

                switch (opcija)
                {
                    case 1:
                        Create(proxy);
                        break;
                    case 2:

                        MenuRead(opcija.ToString(), proxy, korisnik);
                        break;
                    case 3:
                        MenuModify(opcija.ToString(), proxy, korisnik);
                        break;
                    case 4:
                        MenuSupervise(opcija.ToString(), proxy, korisnik);
                        break;
                    default:
                        Console.WriteLine("Pogresna opcija");
                        break;
                }
            } while (opcija != 6);


        }
        static void MenuRead(string operation, ClientFactory proxy, string korisnik)
        {

            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);

            string key = SecretKey.GenerateKey();

            string keyFile = "C:/Bezbednost/clientKey.txt";
            SecretKey.StoreKey(key, keyFile);

            string message = MessageForSend(operation);
            byte[] encripted = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            string EnkriptovanaPoruka = ASCIIEncoding.ASCII.GetString(encripted);

            byte[] signature = DigitalSignature.Create(EnkriptovanaPoruka, "SHA1", certificateSign);
            proxy.Read(encripted, korisnik, signature);
            proxy.DataFromServerToCLientDecripted();
        }
        static void MenuSupervise(string operation, ClientFactory proxy, string korisnik)
        {
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);
            string key = SecretKey.GenerateKey();

            string keyFile = "C:/Bezbednost/clientKey.txt";
            SecretKey.StoreKey(key, keyFile);
            string message = MessageForSend(operation);
            byte[] encripted = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            string EnkriptovanaPoruka = ASCIIEncoding.ASCII.GetString(encripted);

            byte[] signature = DigitalSignature.Create(EnkriptovanaPoruka, "SHA1", certificateSign);
            proxy.Supervise(encripted, korisnik, signature);
            proxy.DataFromServerToCLientDecripted();
        }


        static void MenuModify(string operation, ClientFactory proxy, string korisnik)
        {
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);
            string key = SecretKey.GenerateKey();
            string keyFile = "C:/Bezbednost/clientKey.txt";
            SecretKey.StoreKey(key, keyFile);
            string message = MessageForSend(operation);
            byte[] encripted = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            string EnkriptovanaPoruka = ASCIIEncoding.ASCII.GetString(encripted);

            byte[] signature = DigitalSignature.Create(EnkriptovanaPoruka, "SHA1", certificateSign);




            Dictionary<int, Entitet> EntitiesMadeByModifier = new Dictionary<int, Entitet>();
            entiteti = ReadFromDatabase();
            Console.WriteLine("Entiteti koje ste vi napravili : ");
            Console.WriteLine();
            foreach (var item in entiteti.Values)
            {
                if (item.SID == korisnik)
                {
                    EntitiesMadeByModifier.Add(item.Id, item);
                    Console.WriteLine("\t"+item.ToString().Replace(';', '\n'));
                }
            }
            
            bool Pronadjen = false;
            Console.WriteLine("Da li zelite da modifikujete ili da obrisete entitet");
            Console.WriteLine("Modifikacija : 1");
            Console.WriteLine("Brisanje : 2");
            Console.WriteLine("exit : Pritisnite bilo sta");
            string md = Console.ReadLine();
            if (EntitiesMadeByModifier.Count == 0)
            {

                Console.WriteLine("******************************************************************************");
                Console.WriteLine("Morate prvo da napravite dogadjaj, da biste izvrsili modifikaciju ili brisanje");
                Console.WriteLine("******************************************************************************");
                return;
            }
            int Mod_Del = Int32.Parse(md); // modifikacija/brisanje
          
            switch (Mod_Del)
            {
                case 1:
                    do
                    {
                        Console.WriteLine("Unesite ID entiteta koji zelite da izmenite");
                        string id = Console.ReadLine();
                        if (!EntitiesMadeByModifier.ContainsKey(Int32.Parse(id)))
                        {
                            Console.WriteLine("Entitet sa tim ID ne postoji u bazi");
                            Pronadjen = false;
                        }
                        else
                        {
                            Entitet newEntity = entiteti[Int32.Parse(id)];
                            Pronadjen = true;
                            Console.WriteLine($"Unesite nove podatke za odabrani entitet -> {newEntity.ToString()}");
                            Console.WriteLine("Ime : ");
                            newEntity.Name = Console.ReadLine();
                            newEntity.Time = DateTime.Now;
                            string newEntityForSend = MessageForSend(newEntity.ToString());
                            byte[] encriptedEntity = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, newEntityForSend);
                            proxy.Modify(encriptedEntity, encripted, korisnik, signature);
                            if (proxy.Subscriber)
                            {
                                string allEntities = File.ReadAllText("C:/Bezbednost/Baza.txt");
                                string[] entitites = allEntities.Split(';');
                                entiteti = new Dictionary<int, Entitet>();
                                for (int i = 0; i < entitites.Length - 1; i++)
                                {
                                    string[] entityParts = entitites[i].Split('|');
                                    int Id = Int32.Parse(entityParts[0]);
                                    string name = entityParts[1];                                                        // Citanje pdataka iz baze
                                    string SID = entityParts[2];
                                    DateTime time = DateTime.Parse(entityParts[3]);
                                    Entitet e = new Entitet { Id = Id, Name = name, SID = SID, Time = time };
                                    entiteti.Add(e.Id, e);
                                }
                                Console.WriteLine("***************UPDATE-ovani podaci iz baze***************");
                                Console.WriteLine("---------------------------------------------------------");
                                foreach (var item in entiteti.Values)
                                {
                                    Console.WriteLine(item.ToString());
                                }
                                Console.WriteLine("---------------------------------------------------------");

                            }
                        }
                    } while (Pronadjen == false);
                    break;
                case 2:
                    do
                    {
                        Console.WriteLine("Unesite ID entiteta koji zelite da obrisete");
                        string id = Console.ReadLine();
                        if (!EntitiesMadeByModifier.ContainsKey(Int32.Parse(id)))
                        {
                            Console.WriteLine("Entitet sa tim ID ne postoji u bazi");
                            Pronadjen = false;
                        }
                        else
                        {
                            Entitet EntityForRemove = entiteti[Int32.Parse(id)];
                            Pronadjen = true;

                            string idForRemove = MessageForSend(EntityForRemove.Id.ToString() + "|brisanje");
                            byte[] encriptedIDForRemove = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, idForRemove);
                            proxy.Modify(encriptedIDForRemove, encripted, korisnik, signature);

                            if (proxy.Subscriber)
                            {
                                string allEntities = File.ReadAllText("C:/Bezbednost/Baza.txt");
                                string[] entitites = allEntities.Split(';');
                                entiteti = new Dictionary<int, Entitet>();
                                for (int i = 0; i < entitites.Length - 1; i++)
                                {
                                    string[] entityParts = entitites[i].Split('|');
                                    int Id = Int32.Parse(entityParts[0]);
                                    string name = entityParts[1];                                                        // Citanje pdataka iz baze
                                    string SID = entityParts[2];
                                    DateTime time = DateTime.Parse(entityParts[3]);
                                    Entitet e = new Entitet { Id = Id, Name = name, SID = SID, Time = time };
                                    entiteti.Add(e.Id, e);
                                }
                                Console.WriteLine("***************UPDATE-ovani podaci iz baze***************");
                                Console.WriteLine("---------------------------------------------------------");

                                foreach (var item in entiteti.Values)
                                {
                                    Console.WriteLine(item.ToString());
                                }
                                Console.WriteLine("---------------------------------------------------------");
                            }
                        }
                    } while (Pronadjen == false);break;

                default:
                    Console.WriteLine("");
                break;
            }
            
        }
    static void Create(ClientFactory proxy)
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
        if (proxy.Subscriber)
        {
            Console.WriteLine("***************UPDATE-ovani podaci iz baze***************");
            Console.WriteLine("---------------------------------------------------------");
            foreach (var item in entiteti.Values)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("---------------------------------------------------------");

        }
        string toWrite = "";
        foreach (Entitet en in entiteti.Values)
        {
            toWrite += en.ToString() + "\n";
        }
        File.WriteAllText("C:/Bezbednost/Baza.txt", toWrite);
    }

    static Dictionary<int, Entitet> ReadFromDatabase()
    {
        string allEntities = File.ReadAllText("C:/Bezbednost/Baza.txt");
        string[] entitites = allEntities.Split(';');
        entiteti = new Dictionary<int, Entitet>();
        for (int i = 0; i < entitites.Length - 1; i++)
        {
            string[] entityParts = entitites[i].Split('|');
            int Id = Int32.Parse(entityParts[0]);
            string name = entityParts[1];                                                        // Citanje pdataka iz baze
            string SID = entityParts[2];
            DateTime time = DateTime.Parse(entityParts[3]);
            Entitet e = new Entitet { Id = Id, Name = name, SID = SID, Time = time };
            entiteti.Add(e.Id, e);
        }
        return entiteti;
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
