using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IProsledi> servis = new ChannelFactory<IProsledi>("ServisLoad");

            IProsledi kanal = servis.CreateChannel();

            Console.WriteLine("WORKER POVEZAN NA LOADBALANCER");

            kanal.Prosledi();
            Console.ReadKey();
        }
    }
}
