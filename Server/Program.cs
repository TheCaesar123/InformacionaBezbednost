using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IEntitet> servis = new ChannelFactory<IEntitet>("Servis");

            IEntitet kanal = servis.CreateChannel();

            Console.WriteLine("SERVER POVEZAN NA LoadBalancer");

            using (ServiceHost host = new ServiceHost(typeof(EntitetServer))) 
            {
                host.Open();
                Console.WriteLine("Servis je uspesno pokrenut ");
                Console.ReadKey();
                host.Close();
            }
        }
    }
}
