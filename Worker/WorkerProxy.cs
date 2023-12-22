using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class WorkerProxy : ChannelFactory<IProsledi>, IProsledi, IDisposable
    {
        IProsledi factory;
   
        public WorkerProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
      
        public Entitet LBToWorker(Entitet e)
        {
          
            Entitet et = factory.LBToWorker(e);
           
            return et;
           
        }

        public void Prosledi(string idDogadjaja, string korisnik)
        {
            Console.WriteLine("WorkerProxy poziv.");
        }

        public Entitet WorkerToLB(Entitet e)
        {
            Console.WriteLine($"Worker pje poslao nove podatke za dogadjaj sa ID : {e.Id} -> {e.ToString()}");
            factory.WorkerToLB(e);
            return e;
        }
    }
}
