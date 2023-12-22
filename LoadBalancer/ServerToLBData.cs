using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class ServerToLBData : IProsledi
    {
        public Dictionary<int, Entitet> entiteti = new Dictionary<int, Entitet>();

        public Entitet LBToWorker(Entitet e)
        {
            Console.WriteLine("kkkk");
            Console.WriteLine(e.ToString());
            return e;
        }

        public void Prosledi(string idDogadjaja, string korisnik)
        {

            Console.WriteLine(idDogadjaja + "test" + korisnik);
            Console.WriteLine("PROSLEDI");
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
            Entitet entitetForSend = new Entitet();
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8003/IProsledi";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            if (korisnik == entiteti[Int32.Parse(idDogadjaja)].SID)
            {
                Console.WriteLine("Korisnik ima pravo da izmeni dogadjaj");
                Console.WriteLine("Zahtev za izmenu se prosledjuje slobodnoj WORKER komponenti");
                entitetForSend = entiteti[Int32.Parse(idDogadjaja)];
                
                using (LBProxy proxy = new LBProxy(binding, address))
                {
                    
                    proxy.LBToWorker(entitetForSend);
                }
             
            }
            else
            {
                Console.WriteLine("Korisnik nema pravo da izmeni dogadjaj");

            }
            
            
        }

        public Entitet WorkerToLB(Entitet e)
        {
            Console.WriteLine("Obradjeni dogadjaj od workera");
            Console.WriteLine(e.ToString());
            return e;
        }
    }
}
