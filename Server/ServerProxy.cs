using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerProxy : ChannelFactory<IProsledi>, IProsledi, IDisposable
    {
        IProsledi factory;
       

        public ServerProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
           
            factory = this.CreateChannel();
            
        }

        public Entitet LBToWorker(Entitet e)
        {
            factory.LBToWorker(e);
            return e;
        }

        public void Prosledi(string idDogadjaja, string korisnik)
        {
          
            factory.Prosledi(idDogadjaja, korisnik);
            Console.WriteLine(idDogadjaja + korisnik + "serverproxy");
            Console.WriteLine("ServerProxy poziv.");
        }

      

        public Entitet WorkerToLB(Entitet e)
        {
            throw new  NotImplementedException();
        }
    }
}
