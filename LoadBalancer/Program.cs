using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(ProslediServer)))
            {
                host.Open();
                Console.WriteLine("LoadBalancer je uspesno pokrenut ");
                Console.ReadKey();
                ChannelFactory<IEntitet> servis = new ChannelFactory<IEntitet>("Servis");

                IEntitet kanal = servis.CreateChannel();

                Console.WriteLine("LOADBALANCER POVEZAN NA SERVER");

                kanal.Read();
                kanal.Modify();
                kanal.Supervise();
                Console.ReadKey();
                host.Close();
            }

        }
    }
}
