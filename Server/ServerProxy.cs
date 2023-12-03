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

        public void Prosledi()
        {
            //factory.Prosledi();
            Console.WriteLine("ServerProxy poziv.");
        }
      
    }
}
