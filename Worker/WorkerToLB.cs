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
      
        public Entitet LBToWorker(Entitet e, int mod_del)
        {
            List<worker> workers = new List<worker>();
            Random random = new Random();
            int cost1 = random.Next(0, 11);
            int cost2 = random.Next(0, 11);
            int cost3 = random.Next(0, 11);
            
            worker w1 = new worker(cost1);
            worker w2 = new worker(cost2);
            worker w3 = new worker(cost3);
            Console.WriteLine("Lista Workera :");
            Console.WriteLine($"Worker {w1.Id}" + " Cost factor : " + w1.CostFactor);
            Console.WriteLine($"Worker {w2.Id}" + " Cost factor : " + w2.CostFactor);
            Console.WriteLine($"Worker {w3.Id}" + " Cost factor : " + w3.CostFactor);
        
            workers.Add(w1);
            workers.Add(w2);
            workers.Add(w3);

            worker workerWithMinCost = workers.OrderBy(x => x.CostFactor).FirstOrDefault();

           

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8002/IProsledi";

            binding.Security.Mode = SecurityMode.Transport;
            using (WorkerProxy proxy = new WorkerProxy(binding, address))
            {
                ObradaZahteva(workerWithMinCost, e, mod_del);
                
                proxy.WorkerToLB(e);


            }

            return e;
        }

        public void ObradaZahteva(worker w, Entitet e, int mod_del)
        {
            Random random = new Random();
            int newCost = random.Next(0, 11);
           
            Dictionary<int, Entitet> AllEntities = ReadFromDatabase();
            if (mod_del == 0)
            {
                Console.WriteLine($"Podatak obradjuje worker ID: {w.Id}");


                Console.WriteLine("Entitet se obradjuje...");
                AllEntities[e.Id] = e;
                string toWrite = "";
                foreach (Entitet en in AllEntities.Values)
                {
                    toWrite += en.ToString() + "\n";
                }
                File.WriteAllText("C:/Bezbednost/Baza.txt", toWrite);
                Thread.Sleep(2000);
                Console.WriteLine("*******************");
                Console.WriteLine("Entitet je izmenjen");
                Console.WriteLine("*******************");

                w.CostFactor += newCost;
            }
            else
            {
                Console.WriteLine($"Podatak ce obrisati worker ID: {w.Id}");


                Console.WriteLine("Entitet se brise...");
                AllEntities.Remove(e.Id);
                string toWrite = "";
                foreach (Entitet en in AllEntities.Values)
                {
                    toWrite += en.ToString() + "\n";
                }
                File.WriteAllText("C:/Bezbednost/Baza.txt", toWrite);
                Thread.Sleep(2000);
                Console.WriteLine("*******************");
                Console.WriteLine("Entitet je obrisan");
                Console.WriteLine("*******************");

                w.CostFactor += newCost;
            }


            Console.WriteLine($"Novi cost factor workera sa ID : {w.Id} ->  {w.CostFactor}");

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

        public void LBtoServer(string Podaci)
        {
            throw new NotImplementedException();
        }
    }
}
