using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LBProxy : ChannelFactory<IProsledi>, IProsledi, IDisposable
    {
        IProsledi factory;
        public LBProxy(NetTcpBinding binding, string address) : base(binding, address)
        {

            factory = this.CreateChannel();

        }

        public Entitet LBToWorker(Entitet e)
        {
            Console.WriteLine("poslato ka workeru");
            factory.LBToWorker(e);
            return e;
          
        }

        public void Prosledi(string id, string korisnik)
        {
            factory.Prosledi(id, korisnik);
        }

        public Entitet WorkerToLB(Entitet e)
        {
            Console.WriteLine("Loadbalancer primio od workera");
            Entitet noviEnt = factory.WorkerToLB(e);
            Console.WriteLine(noviEnt.ToString() + "ovdeee");
            return noviEnt;
        }
    }
}
