using Common;
using Server;
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

        public void LBtoServer(string Podaci)
        {
            throw new NotImplementedException();
        }

        public Entitet LBToWorker(Entitet e, int mod_del)
        {
           
            return e;
        }

        public void Prosledi(string izmenjeniEntitet, string korisnik)
        {

            
        
            entiteti = ReadFromDatabase();


            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8003/IProsledi";
      

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Entitet entitetForSend = new Entitet();
            if (izmenjeniEntitet.Contains("brisanje"))
            {
                string[] part = izmenjeniEntitet.Split('|');
                int idForRemove = Int32.Parse(part[0]);
                if (korisnik == entiteti[idForRemove].SID)
                {
                    Console.WriteLine("Korisnik ima pravo da obrise entitet");
                    Console.WriteLine("Zahtev za izmenu se prosledjuje slobodnoj WORKER komponenti");

                    entitetForSend = entiteti[idForRemove];
                    using (LBProxy proxy = new LBProxy(binding, address))
                    {

                        proxy.LBToWorker(entitetForSend, 1);
                    }

                }
                else
                {
                    Console.WriteLine("Korisnik nema pravo da obrise entitet");
                   
                }
            }
            else
            {
                string[] parts = izmenjeniEntitet.Split('|');
                entitetForSend.Id = Int32.Parse(parts[0]);
                entitetForSend.Name = parts[1];
                entitetForSend.SID = parts[2];
                entitetForSend.Time = DateTime.Now;
                if (korisnik == entiteti[entitetForSend.Id].SID)
                {
                    Console.WriteLine("************************************");
                    Console.WriteLine("Korisnik ima pravo da izmeni entitet");
                    Console.WriteLine("************************************");
                    Console.WriteLine("Zahtev ce se proslediti slobodnoj WORKER komponenti");


                    using (LBProxy proxy = new LBProxy(binding, address))
                    {

                        proxy.LBToWorker(entitetForSend, 0);
                    }

                }
                else
                {
                    Console.WriteLine("Korisnik nema pravo da izmeni dogadjaj");

                }
            }
           
            
            
        }
        public Dictionary<int, Entitet> ReadFromDatabase()
        {
            Dictionary<int, Entitet> entiteti = new Dictionary<int, Entitet>();
            string allEntities = File.ReadAllText("C:/Bezbednost/Baza.txt");
            string[] entitites = allEntities.Split(';');
            for (int i = 0; i < entitites.Length - 1; i++)
            {
                string[] entityParts = entitites[i].Split('|');
                int id = Int32.Parse(entityParts[0]);
                string name = entityParts[1];
                string SID = entityParts[2];
                DateTime time = DateTime.Parse(entityParts[3]);
                Entitet e = new Entitet { Id = id, Name = name, SID = SID, Time = time };
                entiteti.Add(e.Id, e);
            }
            return entiteti;
        }
        public Entitet WorkerToLB(Entitet e)
        {
            Console.WriteLine("Obradjeni dogadjaj od workera :");
            Console.WriteLine("----------------------------------");
            Console.WriteLine(e.ToString());
            Console.WriteLine("----------------------------------");

            return e;
        }
    }
}
