using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
{
    public class WorkerToLB : IProsledi
    {
      
        public Entitet LBToWorker(Entitet e)
        {
            List<worker> workers = new List<worker>();
            Random random = new Random();
            int cost1 = random.Next(0, 11);
            int cost2 = random.Next(0, 11);
            int cost3 = random.Next(0, 11);
            
            worker w1 = new worker(cost1);
            worker w2 = new worker(cost2);
            worker w3 = new worker(cost3);
            Console.WriteLine("w1" + w1.CostFactor);
            Console.WriteLine("w2" + w2.CostFactor);
            Console.WriteLine("w3" + w3.CostFactor);
            workers.Add(w1);
            workers.Add(w2);
            workers.Add(w3);
            worker workerWithMinCost = workers.OrderBy(x => x.CostFactor).FirstOrDefault();
            Console.WriteLine("Entitet za izmenu -> " + e.ToString());
           

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8002/IProsledi";

            binding.Security.Mode = SecurityMode.Transport;
            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            //binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            using (WorkerProxy proxy = new WorkerProxy(binding, address))
            {
                ObradaZahteva(workerWithMinCost, e);
                Console.WriteLine("odabrani" + workerWithMinCost.CostFactor);
                proxy.WorkerToLB(e);


            }

            return e;
        }

        public void ObradaZahteva(worker w, Entitet e)
        {
            Random random = new Random();
            int newCost = random.Next(0, 5);
            Entitet newData = new Entitet();
            Dictionary<int, Entitet> AllEntities = ReadFromDatabase();
            Console.WriteLine($"Podatak obradjuje worker ID: {w.Id}");
            Console.WriteLine("Unesite nove podatke : ");
            Console.WriteLine("Novo ime : ");
            e.Name = Console.ReadLine();
            e.Time = DateTime.Now;
            newData = e;

            Console.WriteLine("Entitet se obradjuje...");
            AllEntities[e.Id] = newData;
            string toWrite = "";
            foreach (Entitet en in AllEntities.Values)
            {
                toWrite += en.ToString() + "\n";
            }
            File.WriteAllText("C:/Bezbednost/Baza.txt", toWrite);
            Thread.Sleep(2000);
            Console.WriteLine("Entitet je izmenjen");
            w.CostFactor += newCost;

          

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
        public void Prosledi(string id, string korisnik)
        {
            throw new NotImplementedException();
        }

        Entitet IProsledi.WorkerToLB(Entitet e)
        {
            throw new NotImplementedException();
        }
    }
}
