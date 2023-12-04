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

        public void Prosledi()
        {
            Console.WriteLine("WorkerProxy poziv.");
        }

        public void ServerTOClient(Entitet entitet)
        {
            Console.WriteLine(entitet.Name+"nestooo");
        }
    }
}
