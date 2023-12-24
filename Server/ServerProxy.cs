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

        public void LBtoServer(string Podaci)
        {
            factory.LBtoServer(Podaci);
            Console.WriteLine(Podaci);
        }

        public Entitet LBToWorker(Entitet e, int mod_del)
        {
            factory.LBToWorker(e, mod_del);
            return e;
        }

        public void Prosledi(string idDogadjaja, string korisnik)
        {
            Console.WriteLine("ServerProxy poziv.");
            factory.Prosledi(idDogadjaja, korisnik);
            
           
        }

      

        public Entitet WorkerToLB(Entitet e)
        {
            factory.WorkerToLB(e);
            Console.WriteLine(e.ToString() + "server");
            return e;
        }
    }
}
