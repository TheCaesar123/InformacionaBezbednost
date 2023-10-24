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
               
            }

        }
    }
}
