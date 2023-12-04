using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class ProslediServer : IProsledi
    {
        public void Prosledi()
        {
            Console.WriteLine("PROSLEDI");
        }

        public void ServerTOClient(Entitet e)
        {
            Console.WriteLine(e.Name+"nesto");
        }
    }
}
