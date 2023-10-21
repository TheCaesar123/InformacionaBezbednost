using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class EntitetServer : IEntitet
    {
        public void Modify()
        {
            Console.WriteLine("MODIFY"); 
        }

        public void Read()
        {
            Console.WriteLine("READ");
        }

        public void Supervise()
        {
            Console.WriteLine("SUPERVISE");
        }
    }
}
